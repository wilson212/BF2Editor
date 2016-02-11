using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF2ScriptingEngine
{
    /// <summary>
    /// Provides an instruction to the <see cref="ScriptEngine"/> to perform if a
    /// <see cref="Scripting.TokenType.Include"/> or <see cref="Scripting.TokenType.Run"/> 
    /// instruction is encountered within a parsing a <see cref="ConFile"/>
    /// </summary>
    public enum ExecuteInstruction
    {
        /// <summary>
        /// Tells the <see cref="ScriptEngine"/> to ignore the instruction
        /// </summary>
        Skip,

        /// <summary>
        /// Tells the <see cref="ScriptEngine"/> to execute the command using
        /// the current active Scope
        /// </summary>
        ExecuteInScope,

        /// <summary>
        /// Tells the <see cref="ScriptEngine"/> to spawn a new Child <see cref="Scope"/>, and
        /// execute the command in that new Child Scope
        /// </summary>
        ExecuteInNewScope
    }
}
