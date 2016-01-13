using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BF2ScriptingEngine.Scripting
{
    public abstract class ObjectPropertyBase
    {
        /// <summary>
        /// The name of this property tag
        /// </summary>
        public string Name;

        /// <summary>
        /// If there was a comment proceeding this object, its set here
        /// </summary>
        public RemComment Comment;

        /// <summary>
        /// The line number in the <see cref="ConFile"/> this object is located
        /// </summary>
        public Token Token;

        /// <summary>
        /// Takes an array of string values, and converts it to the proper value type for
        /// this instance's Generic Type
        /// </summary>
        public abstract void SetValueFromParams(Token token, int objectLevel = 0);

        /// <summary>
        /// Converts the value of this property to file format
        /// </summary>
        /// <param name="referenceName">The object type reference name used to call upon this property</param>
        /// <param name="field">This object property's field info</param>
        public abstract string ToFileFormat(Token token, FieldInfo field);

        /// <summary>
        ///     Using reflection, this method creates a new instance of the <paramref name="field"/>
        ///     type, and returns it.
        /// </summary>
        /// <remarks>
        ///     This method does NOT set the field value, since hey... we dont have an instance to work on
        /// </remarks>
        /// <param name="field">The field we are creating an instance for. </param>
        /// <param name="token">The ConFile token for the ObjectProperty ctor</param>
        /// <param name="comment">The RemComment for the ObjectProperty ctor</param>
        /// <exception cref="System.Exception">
        ///     Thrown if the Field provided does not contain the <see cref="PropertyName"/> attribute
        /// </exception>
        public static ObjectPropertyBase Create(FieldInfo field, Token token, RemComment comment)
        {
            // If the Custom attribute exists, we add it to the Mapping
            Attribute attribute = Attribute.GetCustomAttribute(field, typeof(PropertyName));
            if (attribute == null)
                throw new Exception($"Internal property \"{field.Name}\" does not contain a PropertyName attribute!");

            // Get our constructor
            PropertyName fieldAttr = attribute as PropertyName;
            return (ObjectPropertyBase)Activator.CreateInstance(
                field.FieldType,
                new object[] { fieldAttr.Name, token, comment }
            );
        }

        /// <summary>
        /// Converts the object value into the Typed variant of this <see cref="Value"/>
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        protected K ConvertValue<K>(object Value, Type PropertyType)
        {
            // No need to change type if types match
            if (Value.GetType() == PropertyType)
            {
                return (K)Value;
            }

            // Enums need special care
            if (PropertyType.IsEnum)
            {
                return (K)Enum.Parse(PropertyType, Value.ToString(), true);
            }

            // Bools need special care, since a string of "1" or "0" fails to
            // parse in bool.TryParse, but a 1 or 0 parses fine (as an Int)
            if (PropertyType == typeof(bool))
            {
                bool boolValue;
                if (bool.TryParse(Value.ToString(), out boolValue))
                {
                    return (K)(boolValue as object);
                }
                else
                {
                    int intValue;
                    if (Int32.TryParse(Value.ToString(), out intValue))
                        return (K)Convert.ChangeType(intValue, PropertyType);
                }
            }

            return (K)Convert.ChangeType(Value, PropertyType);
        }

        /// <summary>
        /// Converts an object array to the specified type.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="propertyType"></param>
        /// <returns></returns>
        protected object ConvertArray(Object[] values, Type propertyType)
        {
            // Set the value to the instanced object
            switch (propertyType.Name.ToLowerInvariant())
            {
                case "int[]":
                case "int32[]":
                    return Array.ConvertAll(values, Convert.ToInt32);
                case "string[]":
                    return Array.ConvertAll(values, Convert.ToString);
                case "double[]":
                    return Array.ConvertAll(values, Convert.ToDouble);
                case "decimal[]":
                    return Array.ConvertAll(values, Convert.ToDecimal);
                default:
                    throw new Exception("Invalid property type: " + propertyType.Name);
            }
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
        protected static ConFileObject CreateObject(Type type)
        {
            return (ConFileObject)Activator.CreateInstance(type);
        }
    }
}
