using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF2ScriptingEngine.Scripting
{
    /// <summary>
    /// This enumeration is used to instruct the script engine
    /// how the value of this token should be parsed
    /// </summary>
    public enum TokenType
    {
        /// <summary>
        /// Represents an unparsable token
        /// </summary>
        None,

        /// <summary>
        /// Represents the start of an object (Object.Create)
        /// </summary>
        ObjectStart,

        /// <summary>
        /// Represents a property of an object (Object.property)
        /// </summary>
        ObjectProperty,

        /// <summary>
        /// Represents an object reference switch (Object.Active(?:Safe))
        /// </summary>
        ActiveSwitch,

        /// <summary>
        /// Represents a single line comment
        /// </summary>
        RemComment,

        /// <summary>
        /// Represents a multiline comment block
        /// </summary>
        BeginRem,

        /// <summary>
        /// Represents a closing tag for a rem comment
        /// </summary>
        EndRem,

        /// <summary>
        /// Represents a var
        /// </summary>
        Variable,

        /// <summary>
        /// Represents a const
        /// </summary>
        Constant,

        /// <summary>
        /// Represents an IF expression
        /// </summary>
        IfStart,

        /// <summary>
        /// Represents a closing IF tag
        /// </summary>
        EndIf,

        /// <summary>
        /// Represents an include command
        /// </summary>
        Include,

        /// <summary>
        /// Represents a Run command
        /// </summary>
        Run
    }
}
