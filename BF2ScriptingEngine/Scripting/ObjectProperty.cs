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
        public ValueInfo<T> Argument { get; internal set; }

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
                    //var type = ScriptEngine.GetTemplateType(PropertyType);

                    // Check for the object in this properties owner's Scope
                    if (Token.File.Scope.ContainsObject(name, PropertyType))
                    {
                        obj = Token.File.Scope.GetObject(name, PropertyType, Token);
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
            else if (PropertyType.IsGenericType)
            {
                // Check for Generic types, we don't support these!
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
                string referenceName = $"{Token.TokenArgs.ReferenceType.Name}.{Token.TokenArgs.PropertyName}";

                // Append comment if we have one
                //if (!String.IsNullOrEmpty(Token.Comment?.Value))
                    //builder.AppendLine(Token.Comment.Value.TrimEnd());

                // Append reference call and property name
                builder.AppendLine($"{referenceName} {ValueToString(Value, PropertyType)}");
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
                string error = $"Invalid values count for {tkn.TokenArgs.ReferenceType.Name}; Got {values.Length}, Expecting 1.";
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
                string error = $"Invalid values count for {tkn.TokenArgs.ReferenceType.Name}; Got {values.Length}, Expecting 2.";
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
            string referenceName = $"{Token.TokenArgs.ReferenceType.Name}.{Token.TokenArgs.PropertyName}";
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
                string error = $"Invalid values count for {tkn.TokenArgs.ReferenceType.Name}; Got {values.Length}, Expecting 3.";
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
            string referenceName = $"{Token.TokenArgs.ReferenceType.Name}.{Token.TokenArgs.PropertyName}";
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
                string error = $"Invalid values count for {tkn.TokenArgs.ReferenceType.Name}; Got {values.Length}, Expecting 4.";
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
            string referenceName = $"{Token.TokenArgs.ReferenceType.Name}.{Token.TokenArgs.PropertyName}";
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
