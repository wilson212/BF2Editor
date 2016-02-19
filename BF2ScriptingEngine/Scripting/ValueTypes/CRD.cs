using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF2ScriptingEngine.Scripting
{
    /// <summary>
    /// This class represents a "Continuous Random Distribution" object, 
    /// and is used to assign a variable argument.
    /// </summary>
    /// <remarks>
    /// The argument's value is not fixed, but is determined at the time the property 
    /// of an instance of an object first needs it. This allows  effects to have a controlled 
    /// randomness to them: a variable speed and direction for where various bits fly, etc.
    /// 
    /// A CRD always has four sub-arguments, e.g.: CRD_NONE/3/0/0
    /// 
    /// So these first three arguments compute a random number. The final, fourth argument is a 
    /// boolean, and can affect the sign of this computed number. Specifically, if this argument 
    /// is 1 (true), then there is a 50/50 chance that the computed number will be negated. 
    /// If 0 (false), no negation "coin-flip" takes place.
    /// </remarks>
    /// <seealso cref="http://bfmods.com/mdt/scripting/CRD.html"/>
    [TypeConverter(typeof(CRDConverter))]
    public class CRD
    {
        /// <summary>
        /// Gets or Sets what sort of distribution will happen with 
        /// the numbers in this CRD
        /// </summary>
        public CrdType CrdType { get; set; }

        /// <summary>
        /// Definition varies depending on the type of <see cref="CrdType"/> specified
        /// </summary>
        public double Mean { get; set; }

        /// <summary>
        /// Definition varies depending on the type of <see cref="CrdType"/> specified
        /// </summary>
        public double Variance { get; set; }

        /// <summary>
        /// Gets or Sets whether there is a 50/50 chance that the computed number 
        /// will be negated. If false, no negation "coin-flip" takes place.
        /// </summary>
        public bool Negation { get; set; }

        public override string ToString()
        {
            // If we are None, then just return the Mean
            /*if (CrdType == CrdType.CRD_NONE)
            {
                return Mean.ToString("0.####", CultureInfo.InvariantCulture);
            }
            */
            
            int val = Negation ? 1 : 0;
            return $"{CrdType}/{Mean}/{Variance}/{val}";
        }

        /// <summary>
        /// Convert from a String to a Point3D
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator CRD(string value)
        {
            // Ensure we have all 4 coordinates
            string[] parts = value.Split('/');
            if (parts.Length != 4)
            {
                if (parts.Length != 1)
                {
                    int len = parts.Length;
                    throw new Exception(
                        $"Cannot convert to a CRD because there are only {len} arguments; Expecting 4"
                    );
                }
                else
                {
                    return new CRD()
                    {
                        CrdType = CrdType.CRD_NONE,
                        Mean = Converter.ConvertValue<double>(parts[0], typeof(double)),
                        Variance = 0,
                        Negation = false
                    };
                }
            }

            return new CRD()
            {
                CrdType = Converter.ConvertValue<CrdType>(parts[0], typeof(CrdType)),
                Mean = Converter.ConvertValue<double>(parts[1], typeof(double)),
                Variance = Converter.ConvertValue<double>(parts[2], typeof(double)),
                Negation = Converter.ConvertValue<bool>(parts[1], typeof(bool))
            };
        }

        /// <summary>
        /// Convert from a Point3D to a string
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator String(CRD value)
        {
            return value.ToString();
        }
    }

    internal class CRDConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return (sourceType == typeof(string));
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var val = value as string;
            if (val != null)
                return (CRD)val;
            else
                return base.ConvertFrom(context, culture, value);
        }
    }
}
