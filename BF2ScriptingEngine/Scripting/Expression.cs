using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF2ScriptingEngine.Scripting
{
    /// <summary>
    /// This object represents a variable or constant expression
    /// defined in a <see cref="ConFile"/>
    /// </summary>
    /// <example>var v_butts = 20</example>
    public class Expression : ConFileEntry
    {
        public Expression(Token token)
        {
            base.Token = token;
        }
    }
}
