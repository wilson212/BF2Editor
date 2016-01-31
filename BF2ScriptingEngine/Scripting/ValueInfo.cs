using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF2ScriptingEngine.Scripting
{
    public enum ValType
    {
        Literal,
        Variable
    }

    /// <summary>
    /// This object contains information about a specific property argument
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ValueInfo<T>
    {
        /// <summary>
        /// The token that belongs to the <see cref="ObjectProperty{T}"/> 
        /// that owns this property
        /// </summary>
        public Token Token { get; protected set; }

        /// <summary>
        /// The absolute value of this property argument.
        /// </summary>
        /// <remarks>
        /// If the Expression is not null, then setting this value will not affect 
        /// the value thats saved in the <see cref="ConFile"/> when ToFileFormat()
        /// is called
        /// </remarks>
        public T Value { get; set; }

        /// <summary>
        /// Indicates whether this Value is a Literal value, or bound to an Expression
        /// </summary>
        public ValType Type
        {
            get
            {
                return Expression == null ? ValType.Literal : ValType.Variable;
            }
        }

        /// <summary>
        /// The Assignable (variable or constant) in which this value is stored, if any
        /// </summary>
        public Expression Expression { get; protected set; }

        /// <summary>
        /// Creates a new instance of <see cref="ValueInfo{T}"/>
        /// </summary>
        /// <param name="item"></param>
        /// <param name="expression"></param>
        public ValueInfo(T item, Expression expression = null)
        {
            this.Value = item;
            this.Expression = expression;
        }

        /// <summary>
        /// Sets the reference to this property value to the specified assingable. When bound,
        /// the value shares the value with the expression.
        /// </summary>
        /// <param name="expression"></param>
        public void Reference(Expression expression)
        {
            // Ensure that this value AND expression are saved in the same file
            if (expression.Token.File.FilePath != Token.File.FilePath)
                throw new Exception("Expression file is not equal to the file of this Value");

            // TYPE CHECK
            Value = Converter.ConvertValue<T>(expression.Value, typeof(T));

            // Set expression now that we know the types can convert
            this.Expression = expression;
        }

        /// <summary>
        /// Removes the expression this property references if any
        /// </summary>
        public void UnReference()
        {
            this.Expression = null;
        }

        /// <summary>
        /// Converts this <see cref="ValueInfo{T}.Value"/> to file format
        /// </summary>
        /// <returns></returns>
        public string ToFileFormat()
        {
            // If we are bound to an assignable, then we return its name
            if (Type == ValType.Variable)
            {
                // Ensure we can cast to it, since Expression.Value is a string
                Value = Converter.ConvertValue<T>(Expression.Value, typeof(T));
                return Expression.Name;
            }

            // Cast our Value to a plain object
            Type propertyType = typeof(T);
            object value = Value;

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
    }
}
