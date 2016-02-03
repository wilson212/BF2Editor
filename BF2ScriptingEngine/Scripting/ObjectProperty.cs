using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using BF2ScriptingEngine.Scripting;
using BF2ScriptingEngine.Scripting.Attributes;

namespace BF2ScriptingEngine
{
    /// <summary>
    /// Represents a property to a ConFile object
    /// </summary>
    /// <typeparam name="T">The expected value of this property</typeparam>
    public class ObjectProperty<T> : ObjectProperty
    {
        /// <summary>
        /// Gets the argument information that provided the Value of this property
        /// </summary>
        public ValueInfo<T> Argument { get; protected set; }

        /// <summary>
        /// Gets or Sets the value for this property
        /// </summary>
        public T Value
        {
            get
            {
                if (Argument == null)
                    return default(T);

                return Argument.Value;
            }
            set { Argument.Value = value; }
        }

        /// <summary>
        /// Creates a new instance of <see cref="ObjectProperty{T}"/>
        /// </summary>
        /// <param name="name">the property name</param>
        /// <param name="token">the token information</param>
        /// <param name="property">The PropertyInfo object for this instance</param>
        /// <param name="owner">the ConFileObject that owns this property instance</param>
        public ObjectProperty(string name, Token token, PropertyInfo property, ConFileObject owner)
        {
            Name = name;
            Token = token;
            Property = property;
            Owner = owner;
        }

        /// <summary>
        /// Takes an array of string values, and converts it to the proper value type for
        /// this instance's Generic Type
        /// </summary>
        /// <param name="ValueParams">The string value's to convert, and set the 
        /// Value of this instance to.
        /// </param>
        public override void SetValue(Token token, int objectLevel = 0)
        {
            Type PropertyType = typeof(T);

            // ===
            // DONOT use this ObjectProperties TOKEN! breaks collections!
            TokenArgs tokenArgs = token.TokenArgs;
            // ===

            // Check for ConFileObjects
            if (typeof(ConFileObject).IsAssignableFrom(PropertyType))
            {
                // define vars
                ConFileObject obj = (ConFileObject)(object)Value;

                // Load a new object if we are undefined
                if (obj == null)
                {
                    // Is this a defined object?
                    string name = tokenArgs.Arguments.Last();
                    bool existing = Property.GetCustomAttribute(typeof(ExistingObject)) != null;
                    var type = ObjectManager.GetObjectType(PropertyType);

                    if (existing && ObjectManager.ContainsObject(name, type))
                    {
                        obj = ObjectManager.GetObject(name, type);
                        Argument = new ValueInfo<T>((T)(object)obj);
                    }
                    else
                    {
                        string error = $"ObjectProperty \"{Name}\" requires an existing object that is not defined!";
                        throw new Exception(error);
                    }
                }
                else
                {
                    // Let the child class parse itself
                    obj.Parse(token, ++objectLevel);
                    Argument = new ValueInfo<T>((T)(object)obj);
                }
            }

            // Check for Generic types, as they are handled differently
            else if (PropertyType.IsGenericType)
            {
                // Grab our types interfaces and generic types
                Type[] interfaces = PropertyType.GetInterfaces();
                Type[] types = PropertyType.GetGenericArguments();

                // Check for List<T>
                if (interfaces.Any(i => i.Name == "IList"))
                {
                    // Grab our current list... if the Value isnt created yet, make it
                    IList obj = (IList)Value ?? CreateList(types);

                    // If we are indexed, then skip the first argument which is the index identifier
                    if (Property.GetCustomAttribute(typeof(IndexedList)) != null)
                        tokenArgs.Arguments = tokenArgs.Arguments.Skip(1).ToArray();

                    // Add our value to the list
                    if (typeof(ConFileObject).IsAssignableFrom(types[0]))
                        obj.Add(CreateObject(types[0], tokenArgs.Arguments.Last(), token));
                    else if (types[0].IsArray)
                        obj.Add(ConvertArray(tokenArgs.Arguments, types[0]));
                    else
                        obj.Add(ConvertValue<object>(tokenArgs.Arguments[0], types[0]));

                    // Set internal value
                    Argument = new ValueInfo<T>((T)obj);
                }

                // Check for Dictionary<TKey, TVal>
                else if (interfaces.Any(i => i.Name == "IDictionary"))
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
                    //Value = (T)obj;
                    Argument = new ValueInfo<T>((T)obj);
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
                Argument = new ValueInfo<T>(ConvertValue<T>(tokenArgs.Arguments[0], PropertyType));
            }
        }

        /// <summary>
        /// Converts the value of this property to file format
        /// </summary>
        /// <param name="obj">The confile object that contains this property</param>
        /// <param name="field">This object property's field info</param>
        /// <returns></returns>
        public override string ToFileFormat()
        {
            // Get our attributes
            Type PropertyType = typeof(T);
            PropertyName propertyInfo = Property.GetCustomAttribute(typeof(PropertyName)) as PropertyName;
            StringBuilder builder = new StringBuilder();

            // Check for ConFileObjects
            if (typeof(ConFileObject).IsAssignableFrom(PropertyType))
            {
                var subObj = (ConFileObject)(object)Value;
                builder.AppendLine(subObj.ToFileFormat(Token));
            }
            else
            {
                // Create reference name. 
                string referenceName = $"{Token.TokenArgs.ReferenceName}.{Token.TokenArgs.PropertyName}";

                // Append comment if we have one
                //if (!String.IsNullOrEmpty(Token.Comment?.Value))
                    //builder.AppendLine(Token.Comment.Value.TrimEnd());

                // Check for Generic types, as they are handled differently
                if (PropertyType.IsGenericType)
                {
                    // Grab our types interfaces and generic types
                    Type[] interfaces = PropertyType.GetInterfaces();
                    Type[] types = PropertyType.GetGenericArguments();

                    // Check for List<T>
                    if (interfaces.Any(i => i.Name == "IList"))
                    {
                        // Grab our current list... if the Value isnt created yet, make it
                        IList list = (IList)Value;
                        bool indexed = Property.GetCustomAttribute(typeof(IndexedList)) != null;
                        bool isObj = typeof(ConFileObject).IsAssignableFrom(types[0]);
                        int i = 0;

                        // Add a new line in the string builder for each item
                        foreach (object item in list)
                        {

                            // CON File objects
                            if (isObj)
                            {
                                var subObj = (ConFileObject)item;
                                builder.AppendLine(subObj.ToFileFormat());
                            }
                            else
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
                    }

                    // Check for Dictionary<TKey, TVal>
                    else if (interfaces.Any(i => i.Name == "IDictionary"))
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
            }

            // return our built string
            return builder.ToString().TrimEnd();
        }

        public override void SetValues(object[] values, Token token = null)
        {
            Token tkn = token ?? Token;

            // Ensure that we have the correct number of arguments
            if (values.Length != 1)
            {
                string error = $"Invalid values count for {tkn.TokenArgs.ReferenceName}; Got {values.Length}, Expecting 1.";
                Logger.Error(error, token?.File, token?.Position ?? 0);
                throw new Exception(error);
            }

            // Create ValueInfo<T> object
            Argument = ConvertValue<T>(values[0]);
        }
    }

    public class ObjectProperty<T1, T2> : ObjectProperty
    {
        /// <summary>
        /// Gets the property's first argument information
        /// </summary>
        public ValueInfo<T1> Argument1 { get; protected set; }

        /// <summary>
        /// Gets the property's second argument information
        /// </summary>
        public ValueInfo<T2> Argument2 { get; protected set; }

        /// <summary>
        /// Gets or Sets the first value for this property
        /// </summary>
        public T1 Value1
        {
            get { return Argument1.Value; }
            set { Argument1.Value = value; }
        }

        /// <summary>
        /// Gets or Sets the second value for this property
        /// </summary>
        public T2 Value2
        {
            get { return Argument2.Value; }
            set { Argument2.Value = value; }
        }

        /// <summary>
        /// Creates a new instance of <see cref="ObjectProperty{T1,T2}"/>
        /// </summary>
        /// <param name="name">the property name</param>
        /// <param name="token">the token information</param>
        /// <param name="property">The PropertyInfo object for this instance</param>
        /// <param name="owner">the ConFileObject that owns this property instance</param>
        public ObjectProperty(string name, Token token, PropertyInfo property, ConFileObject owner)
        {
            Name = name;
            Token = token;
            Property = property;
            Owner = owner;
        }

        public override void SetValues(object[] values, Token token = null)
        {
            Token tkn = token ?? Token;

            // Ensure that we have the correct number of arguments
            if (values.Length != 2)
            {
                string error = $"Invalid values count for {tkn.TokenArgs.ReferenceName}; Got {values.Length}, Expecting 2.";
                Logger.Error(error, token?.File, token?.Position ?? 0);
                throw new Exception(error);
            }

            // Set parent Token, and convert values
            Argument1 = ConvertValue<T1>(values[0]);
            Argument2 = ConvertValue<T2>(values[1]);
        }

        public override string ToFileFormat()
        {
            // Create reference name. 
            string referenceName = $"{Token.TokenArgs.ReferenceName}.{Token.TokenArgs.PropertyName}";
            StringBuilder sb = new StringBuilder(referenceName);

            // Add Items
            sb.Append($" {Argument1.ToFileFormat()}");
            sb.Append($" {Argument2.ToFileFormat()}");
            return sb.ToString();
        }
    }

    public class ObjectProperty<T1, T2, T3> : ObjectProperty
    {
        /// <summary>
        /// Gets the property's first argument information
        /// </summary>
        public ValueInfo<T1> Argument1 { get; protected set; }

        /// <summary>
        /// Gets the property's second argument information
        /// </summary>
        public ValueInfo<T2> Argument2 { get; protected set; }

        /// <summary>
        /// Gets the property's third argument information
        /// </summary>
        public ValueInfo<T3> Argument3 { get; protected set; }

        /// <summary>
        /// Gets or Sets the first value for this property
        /// </summary>
        public T1 Value1
        {
            get { return Argument1.Value; }
            set { Argument1.Value = value; }
        }

        /// <summary>
        /// Gets or Sets the second value for this property
        /// </summary>
        public T2 Value2
        {
            get { return Argument2.Value; }
            set { Argument2.Value = value; }
        }

        /// <summary>
        /// Gets or Sets the third value for this property
        /// </summary>
        public T3 Value3
        {
            get { return Argument3.Value; }
            set { Argument3.Value = value; }
        }

        /// <summary>
        /// Creates a new instance of <see cref="ObjectProperty{T1,T2,T3}"/>
        /// </summary>
        /// <param name="name">the property name</param>
        /// <param name="token">the token information</param>
        /// <param name="property">The PropertyInfo object for this instance</param>
        /// <param name="owner">the ConFileObject that owns this property instance</param>
        public ObjectProperty(string name, Token token, PropertyInfo property, ConFileObject owner)
        {
            Name = name;
            Token = token;
            Property = property;
            Owner = owner;
        }

        public override void SetValues(object[] values, Token token = null)
        {
            Token tkn = token ?? Token;

            // Ensure that we have the correct number of arguments
            if (values.Length != 3)
            {
                string error = $"Invalid values count for {tkn.TokenArgs.ReferenceName}; Got {values.Length}, Expecting 3.";
                Logger.Error(error, token?.File, token?.Position ?? 0);
                throw new Exception(error);
            }

            // Set parent Token, and convert values
            Argument1 = ConvertValue<T1>(values[0]);
            Argument2 = ConvertValue<T2>(values[1]);
            Argument3 = ConvertValue<T3>(values[2]);
        }

        public override string ToFileFormat()
        {
            // Create reference name. 
            string referenceName = $"{Token.TokenArgs.ReferenceName}.{Token.TokenArgs.PropertyName}";
            StringBuilder sb = new StringBuilder(referenceName);

            // Add Items
            sb.Append($" {Argument1.ToFileFormat()}");
            sb.Append($" {Argument2.ToFileFormat()}");
            sb.Append($" {Argument3.ToFileFormat()}");
            return sb.ToString();
        }
    }

    public class ObjectProperty<T1, T2, T3, T4> : ObjectProperty
    {
        /// <summary>
        /// Gets the property's first argument information
        /// </summary>
        public ValueInfo<T1> Argument1 { get; protected set; }

        /// <summary>
        /// Gets the property's second argument information
        /// </summary>
        public ValueInfo<T2> Argument2 { get; protected set; }

        /// <summary>
        /// Gets the property's third argument information
        /// </summary>
        public ValueInfo<T3> Argument3 { get; protected set; }

        /// <summary>
        /// Gets the property's fourth argument information
        /// </summary>
        public ValueInfo<T4> Argument4 { get; protected set; }

        /// <summary>
        /// Gets or Sets the first value for this property
        /// </summary>
        public T1 Value1
        {
            get { return Argument1.Value; }
            set { Argument1.Value = value; }
        }

        /// <summary>
        /// Gets or Sets the second value for this property
        /// </summary>
        public T2 Value2
        {
            get { return Argument2.Value; }
            set { Argument2.Value = value; }
        }

        /// <summary>
        /// Gets or Sets the third value for this property
        /// </summary>
        public T3 Value3
        {
            get { return Argument3.Value; }
            set { Argument3.Value = value; }
        }

        /// <summary>
        /// Gets or Sets the fourth value for this property
        /// </summary>
        public T4 Value4
        {
            get { return Argument4.Value; }
            set { Argument4.Value = value; }
        }

        /// <summary>
        /// Creates a new instance of <see cref="ObjectProperty{T1,T2,T3,T4}"/>
        /// </summary>
        /// <param name="name">the property name</param>
        /// <param name="token">the token information</param>
        /// <param name="property">The PropertyInfo object for this instance</param>
        /// <param name="owner">the ConFileObject that owns this property instance</param>
        public ObjectProperty(string name, Token token, PropertyInfo property, ConFileObject owner)
        {
            Name = name;
            Token = token;
            Property = property;
            Owner = owner;
        }

        public override void SetValues(object[] values, Token token = null)
        {
            Token tkn = token ?? Token;

            // Ensure that we have the correct number of arguments
            if (values.Length != 4)
            {
                string error = $"Invalid values count for {tkn.TokenArgs.ReferenceName}; Got {values.Length}, Expecting 4.";
                Logger.Error(error, token?.File, token?.Position ?? 0);
                throw new Exception(error);
            }

            // Set parent Token, and convert values
            Argument1 = ConvertValue<T1>(values[0]);
            Argument2 = ConvertValue<T2>(values[1]);
            Argument3 = ConvertValue<T3>(values[2]);
            Argument4 = ConvertValue<T4>(values[3]);
        }

        public override string ToFileFormat()
        {
            // Create reference name. 
            string referenceName = $"{Token.TokenArgs.ReferenceName}.{Token.TokenArgs.PropertyName}";
            StringBuilder sb = new StringBuilder(referenceName);

            // Add Items
            sb.Append($" {Argument1.ToFileFormat()}");
            sb.Append($" {Argument2.ToFileFormat()}");
            sb.Append($" {Argument3.ToFileFormat()}");
            sb.Append($" {Argument4.ToFileFormat()}");
            return sb.ToString();
        }
    }
}
