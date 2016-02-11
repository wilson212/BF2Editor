using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF2ScriptingEngine.Scripting
{
    /// <summary>
    /// An object representation of the parsed line from inside a con file
    /// </summary>
    /// <remarks>
    /// Only Objects and Property accesors get TokenArgs made.
    /// </remarks>
    public class TokenArgs
    {
        /// <summary>
        /// Gets or Sets the the Object Template name that owns this token
        /// </summary>
        /// <example>ObjectTemplate, weaponTemplate, aiTemplate etc etc</example>
        public string TemplateName { get; set; }

        /// <summary>
        /// Gets or Sets the property name or function name
        /// </summary>
        /// <remarks>
        /// If this property is nested, such as a component accessor, then this
        /// property name contains the full path to the property (all periods)
        /// </remarks>
        public string PropertyName { get; set; }

        /// <summary>
        /// Gets or Sets the arguments for this function or property value
        /// </summary>
        public string[] Arguments { get; set; }

        /// <summary>
        /// Gets an array of nested property names for this line.
        /// </summary>
        public string[] PropertyNames
        {
            get
            {
                if (_properties == null)
                    _properties = PropertyName.Split(new char[] { '.' });

                return _properties;
            }
        }

        private string[] _properties;
    }
}
