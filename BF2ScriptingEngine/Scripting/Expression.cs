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
        public string Name { get; protected set; }

        public string Value { get; protected set; }

        public Expression(Token token)
        {
            base.Token = token;
            this.Name = token.Match.Groups["name"].Value;
            if (token.Match.Groups["value"].Success)
                this.Value = token.Match.Groups["value"].Value;
        }
    }
}
