using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BF2ScriptingEngine.Scripting.Attributes;

namespace BF2ScriptingEngine.Scripting
{
    public abstract class ObjectProperty
    {
        /// <summary>
        /// The name of this property tag
        /// </summary>
        public string Name;

        /// <summary>
        /// The <see cref="Token"/> for which this property is first called
        /// </summary>
        public Token Token;

        /// <summary>
        /// Gets the <see cref="PropertyInfo"/> object that represents the holder of
        /// this object
        /// </summary>
        public PropertyInfo Property { get; protected set; }

        /// <summary>
        /// Takes an array of arguments, and attempts to set the values
        /// of this object property
        /// </summary>
        /// <param name="values"></param>
        /// <param name="token"></param>
        public abstract void SetValues(object[] values, Token token = null);

        /// <summary>
        /// Converts the value of this property to file format
        /// </summary>
        public abstract string ToFileFormat();

        /// <summary>
        /// Takes an array of string values, and converts it to the proper value type for
        /// this instance's Generic Type
        /// </summary>
        public virtual void SetValue(Token token, int objectLevel = 0)
        {
            this.SetValues(token.TokenArgs.Arguments, token);
        }

        /// <summary>
        ///     Using reflection, this method creates a new instance of the <paramref name="property"/>
        ///     type, and returns it.
        /// </summary>
        /// <remarks>
        ///     This method does NOT set the field value, since hey... we dont have an instance to work on
        /// </remarks>
        /// <param name="property">The field we are creating an instance for. </param>
        /// <param name="token">The ConFile token for the ObjectProperty ctor</param>
        /// <exception cref="System.Exception">
        ///     Thrown if the Field provided does not contain the <see cref="PropertyName"/> attribute
        /// </exception>
        public static ObjectProperty Create(PropertyInfo property, Token token)
        {
            // If the Custom attribute exists, we add it to the Mapping
            Attribute attribute = Attribute.GetCustomAttribute(property, typeof(PropertyName));
            if (attribute == null)
                throw new Exception($"Internal property \"{property.Name}\" does not contain a PropertyName attribute!");

            // Get our constructor
            PropertyName fieldAttr = attribute as PropertyName;
            return (ObjectProperty)Activator.CreateInstance(
                property.PropertyType,
                new object[] { fieldAttr.Name, token, property }
            );
        }

        /// <summary>
        /// Converts the object value into the Typed variant of this <see cref="Value"/>
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        protected ValueInfo<K> ConvertValue<K>(object Value)
        {
            return Converter.CreateValueInfo<K>(Token, Value);
        }

        /// <summary>
        /// Converts the object value into the Typed variant of this <see cref="Value"/>
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        protected K ConvertValue<K>(object Value, Type PropertyType)
        {
            return Converter.ConvertValue<K>(Value, PropertyType);
        }

        /// <summary>
        /// Converts an object array to the specified type.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="propertyType"></param>
        /// <returns></returns>
        protected object ConvertArray(Object[] values, Type propertyType)
        {
            return Converter.ConvertArray(values, propertyType);
        }

        /// <summary>
        /// Converts the object supplied into the correct format for con/Ai script files
        /// </summary>
        /// <param name="value"></param>
        /// <param name="propertyType"></param>
        /// <returns></returns>
        protected string ValueToString(object value, Type propertyType)
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
            return (val.Any(x => Char.IsWhiteSpace(x))) ? $"\"{val}\"" : val;
        }

        /// <summary>
        /// Creates a new Dictionary with the provided types
        /// </summary>
        /// <param name="genericTypes"></param>
        /// <returns></returns>
        protected IDictionary CreateCollection(Type[] genericTypes)
        {
            Type obj = typeof(Dictionary<,>).MakeGenericType(genericTypes);
            return (IDictionary)Activator.CreateInstance(obj);
        }

        /// <summary>
        /// Creates a new List{T} with the specified types
        /// </summary>
        /// <param name="genericTypes"></param>
        /// <returns></returns>
        protected static IList CreateList(Type[] genericTypes)
        {
            Type obj = typeof(List<>).MakeGenericType(genericTypes);
            return (IList)Activator.CreateInstance(obj);
        }

        /// <summary>
        /// Creates a new confile of the specified type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected object CreateObject(Type type, string name, Token token)
        {
            // Check for abstract
            if (type.IsAbstract)
                throw new Exception($"Cannot create object \"{name}\" from Abstract type \"{type}\"");

            return Activator.CreateInstance(type, new object[] { name, token });
        }
    }
}
