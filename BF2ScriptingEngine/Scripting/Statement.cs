using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF2ScriptingEngine.Scripting
{
    /// <summary>
    /// This object represents a statement found within a <see cref="ConFile"/>.
    /// </summary>
    /// <example>
    /// - run object.tweak
    /// - include object.tweak
    /// - if v_variable == 20
    ///       ....
    ///   endIf
    /// </example>
    public class Statement : ConFileEntry
    {
        public Statement(Token token)
        {
            base.Token = token;
        }
    }
}
