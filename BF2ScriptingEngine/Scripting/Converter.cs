using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF2ScriptingEngine.Scripting
{
    /// <summary>
    /// This class provides methods to convert values from within a bf2 Script
    /// file into the specifed value type.
    /// </summary>
    public static class Converter
    { 
        /// <summary>
        /// Converts the object value into the Typed variant of the Specified type parameter,
        /// and returns the <see cref="ValueInfo{T}"/> object for this token property.
        /// This method also sets the value of the <see cref="ValueInfo{T}.Expression"/> field 
        /// if this property value is a variable/constant reference.
        /// </summary>
        /// <param name="Value">The current property index value</param>
        /// <returns></returns>
        public static ValueInfo<K> CreateValueInfo<K>(ObjectProperty property, object Value)
        {
            // Grab the type of K
            Type PropertyType = typeof(K);
            string strValue = Value.ToString();
            Expression exp = null;

            // Check for variable or constant... names must begin with v_ or c_
            if (strValue.StartsWithAny("v_", "c_"))
            {
                // We use the File's expression reference and NOT the Scope, 
                // because this object property could be created AFTER the inital 
                // parse of the file
                exp = property.Owner.File.GetExpressionReference(strValue, property);
                Value = exp.Value;
            }

            K newValue = ConvertValue<K>(Value, PropertyType);
            return new ValueInfo<K>(newValue, exp);
        }

        /// <summary>
        /// Converts the object value into the Typed variant of this <see cref="Value"/>
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static K ConvertValue<K>(object Value, Type PropertyType)
        {
            Type ValueType = Value.GetType();

            // No need to change type if types match
            if (ValueType == PropertyType)
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

            if (PropertyType.IsClass || PropertyType.IsSealed)
            {
                var converter = TypeDescriptor.GetConverter(PropertyType);
                if (converter?.CanConvertFrom(ValueType) ?? false)
                {
                    object newObject = converter.ConvertFrom(Value);
                    return (K)newObject;
                }
                else
                {
                    throw new Exception(
                        $"Invalid Object TypeConversion from {ValueType.Name} -> {PropertyType.Name}"
                    );
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
        public static object ConvertArray(Object[] values, Type propertyType)
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
    }
}
