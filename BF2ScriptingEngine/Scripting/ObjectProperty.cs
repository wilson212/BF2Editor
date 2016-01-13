using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using BF2ScriptingEngine.Scripting;

namespace BF2ScriptingEngine
{
    /// <summary>
    /// Represents a property to a ConFile object
    /// </summary>
    /// <typeparam name="T">The expected value of this property</typeparam>
    public class ObjectProperty<T> : ObjectPropertyBase
    {
        /// <summary>
        /// Gets or Sets the value for this property
        /// </summary>
        public T Value;

        /// <summary>
        /// Creates a new instance of <see cref="ObjectProperty{T}"/>
        /// </summary>
        /// <param name="name">the property name</param>
        /// <param name="token">the token information</param>
        /// <param name="comment">the rem comment if any</param>
        public ObjectProperty(string name, Token token, RemComment comment)
        {
            Name = name;
            Comment = comment;
            Token = token;
        }

        /// <summary>
        /// Takes an array of string values, and converts it to the proper value type for
        /// this instance's Generic Type
        /// </summary>
        /// <param name="ValueParams">The string value's to convert, and set the 
        /// Value of this instance to.
        /// </param>
        public override void SetValueFromParams(Token token, int objectLevel = 0)
        {
            Type PropertyType = typeof(T);

            // ===
            // DONOT use this ObjectProperties TOKEN! breaks collections!
            TokenArgs tokenArgs = token.TokenArgs;
            // ===

            // Check for ConFileObjects
            if (typeof(ConFileObject).IsAssignableFrom(PropertyType))
            {
                // Let the child class parse itself
                var obj = CreateObject(PropertyType);
                obj.Parse(token, Comment, ++objectLevel);
                Value = (T)(object)obj;
            }

            // Check for array's, as they are handled differently
            else if (PropertyType.IsArray)
            {
                // Since we are an array property, warn the user if we have
                // less values then expected (array should always be > 1)
                if (tokenArgs.Arguments.Length == 1)
                    Logger.Warning($"Expecting value Array for \"{Name}\", but got a single value", Token.File, Token.Position);

                // Set the value to the instanced object
                Value = (T)ConvertArray(tokenArgs.Arguments, PropertyType);
            }
            else if (PropertyType.IsGenericType)
            {
                // Grab our types interfaces and generic types
                Type[] interfaces = PropertyType.GetInterfaces();
                Type[] types = PropertyType.GetGenericArguments();

                // Check for List<T>
                if (interfaces.Any(i => i == typeof(IList)))
                {
                    // Grab our current list... if the Value isnt created yet, make it
                    IList obj = (IList)Value ?? CreateList(types);

                    // Add our value to the list
                    if (types[0].IsArray)
                        obj.Add(ConvertArray(tokenArgs.Arguments, types[0]));
                    else
                        obj.Add(ConvertValue<object>(tokenArgs.Arguments[0], types[0]));

                    // Set internal value
                    Value = (T)obj;
                }

                // Check for Dictionary<TKey, TVal>
                else if (interfaces.Any(i => i == typeof(IDictionary)))
                {
                    // Grab our current Dictionary... if the Value isnt created yet, make it
                    IDictionary obj = (IDictionary)Value ?? CreateCollection(types);

                    // Grab our key
                    object key = ConvertValue<object>(tokenArgs.Arguments[0], types[0]);

                    // Add our value to the list
                    if (types[1].IsArray)
                        obj[key] = ConvertArray(tokenArgs.Arguments.Skip(1).ToArray(), types[1]);
                    else
                        obj[key] = ConvertValue<object>(tokenArgs.Arguments[1], types[1]);

                    // Set internal value
                    Value = (T)obj;
                }
                else
                    throw new Exception($"Invalid Generic Type found \"{PropertyType}\"");
            }
            else
            {
                // Since we are not an array property, make sure we didnt get 
                // an array passed in the con file
                if (tokenArgs.Arguments.Length > 1)
                    throw new Exception($"Expecting single value, but got an Array for \"{Name}\"");

                // Since we are not an array, extract our only value
                Value = ConvertValue<T>(tokenArgs.Arguments[0], PropertyType);
            }
        }

        /// <summary>
        /// Converts the value of this property to file format
        /// </summary>
        /// <param name="obj">The confile object that contains this property</param>
        /// <param name="field">This object property's field info</param>
        /// <returns></returns>
        public override string ToFileFormat(Token token, FieldInfo field)
        {
            // Get our attributes
            Type PropertyType = typeof(T);
            PropertyName propertyInfo = field.GetCustomAttribute(typeof(PropertyName)) as PropertyName;
            StringBuilder builder = new StringBuilder();

            //Create reference name. 
            string referenceName = $"{token.TokenArgs.ReferenceName}.{Token.TokenArgs.PropertyName}";

            // Append comment if we have one
            if (!String.IsNullOrEmpty(Comment?.Value))
                builder.AppendLine(Comment.Value.TrimEnd());

            // Check for ConFileObjects
            if (typeof(ConFileObject).IsAssignableFrom(PropertyType))
            {
                var subObj = (ConFileObject)(object)Value;
                return subObj.ToFileFormat(token);
            }

            // Check for array's, as they are handled differently
            else if (PropertyType.IsArray)
            {
                // Append reference call
                builder.Append(referenceName);

                // Implode the array into a spaced string
                // Thanks to the handy dynamic keyword, this is simplified
                foreach (object val in (dynamic)Value)
                    builder.Append($" {ValueToString(val, val.GetType())}");

                // Close line
                builder.AppendLine();
            }
            else if (PropertyType.IsGenericType)
            {
                // Grab our types interfaces and generic types
                Type[] interfaces = PropertyType.GetInterfaces();
                Type[] types = PropertyType.GetGenericArguments();

                // Check for List<T>
                if (interfaces.Any(i => i == typeof(IList)))
                {
                    // Grab our current list... if the Value isnt created yet, make it
                    IList list = (IList)Value;
                    bool indexed = field.GetCustomAttribute(typeof(IndexedList)) != null;
                    int i = 0;

                    // Add a new line in the string builder for each item
                    foreach (object item in list)
                    {
                        // Append this reference and property name
                        builder.Append(referenceName);
                        if (indexed)
                            builder.Append($" {i++}");
                    
                        // Implode arrays into a string
                        if (types[0].IsArray)
                        {
                            // Thanks to the handy dynamic keyword, this is simplified
                            foreach (object val in (dynamic)item)
                                builder.Append($" {ValueToString(val, val.GetType())}");

                            // Close line
                            builder.AppendLine();
                        }
                        else
                        {
                            // Add formated value
                            builder.AppendLine($" {ValueToString(item, types[0])}");
                        }
                    }
                }

                // Check for Dictionary<TKey, TVal>
                else if (interfaces.Any(i => i == typeof(IDictionary)))
                {
                    // Grab our current Dictionary... if the Value isnt created yet, make it
                    IDictionary dic = (IDictionary)Value;

                    // Add a new line in the string builder for each item
                    foreach (dynamic item in dic)
                    {
                        // Append this reference and property name
                        builder.Append($"{referenceName} {ValueToString(item.Key, types[0])}");

                        // Implode arrays into a string
                        if (types[1].IsArray)
                        {
                            // Thanks to the handy dynamic keyword, this is simplified
                            foreach (object val in (dynamic)item)
                                builder.Append($" {ValueToString(val, val.GetType())}");

                            // Close line
                            builder.AppendLine();
                        }
                        else
                        {
                            // Add formated value
                            builder.AppendLine($" {ValueToString(item, types[1])}");
                        }
                    }
                }
            }
            else
            {
                // Append reference call and property name
                builder.AppendLine($"{referenceName} {ValueToString(Value, PropertyType)}");
            }


            // return our built string
            return builder.ToString().TrimEnd();
        }

        /// <summary>
        /// Converts the object supplied into the correct format for con/Ai script files
        /// </summary>
        /// <param name="value"></param>
        /// <param name="propertyType"></param>
        /// <returns></returns>
        private string ValueToString(object value, Type propertyType)
        {
            // Bool format is 1 and 0, not "true" and "false"
            if (propertyType == typeof(bool))
            {
                return (bool)value ? "1" : "0";
            }
            else if (propertyType == typeof(Double))
            {
                double dVal = (double)Convert.ChangeType(value, TypeCode.Double);
                return dVal.ToString("0.0###", CultureInfo.InvariantCulture);
            }
            else if (propertyType == typeof(Decimal))
            {
                Decimal dVal = (Decimal)Convert.ChangeType(value, TypeCode.Decimal);
                return dVal.ToString(CultureInfo.InvariantCulture);
            }

                // wrap value in quotes if we detect whitespace
            string val = value.ToString();
            return (val.Any(x => Char.IsWhiteSpace(x))) ?  $"\"{val}\"" : val;
        }
    }
}
