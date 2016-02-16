using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BF2ScriptingEngine.Scripting.Interfaces;

namespace BF2ScriptingEngine.Scripting
{
    public struct Point3D : ICastable
    {
        private decimal x;
        private decimal y;
        private decimal z;

        /// <summary>
        /// Gets or Sets the Right and Left position
        /// </summary>
        public decimal X
        {
            get { return x; }
            set { x = value; }
        }

        /// <summary>
        /// Gets or Sets the Up and Down position
        /// </summary>
        public decimal Y
        {
            get { return y; }
            set { y = value; }
        }

        /// <summary>
        /// Gets or Sets the forward and reverse direction
        /// </summary>
        public decimal Z
        {
            get { return z; }
            set { z = value; }
        }

        public override string ToString()
        {
            return $"{X}/{Y}/{Z}";
        }

        /// <summary>
        /// Convert from a String to a Point3D
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator Point3D(string value)
        {
            // Ensure we have all 3 coordinates
            string[] parts = value.Split('/');
            if (parts.Length != 3)
            {
                int len = parts.Length;
                throw new Exception(
                    $"Cannot convert to a Point3D because there are only {len} coordinates"
                );
            }

            return new Point3D()
            {
                X = Decimal.Parse(parts[0], CultureInfo.InvariantCulture),
                Y = Decimal.Parse(parts[1], CultureInfo.InvariantCulture),
                Z = Decimal.Parse(parts[2], CultureInfo.InvariantCulture),
            };
        }

        /// <summary>
        /// Convert from a Point3D to a string
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator String(Point3D value)
        {
            return value.ToString();
        }
    }
}
