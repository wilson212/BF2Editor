using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF2ScriptingEngine.Scripting
{
    public abstract class ConFileEntry
    {
        /// <summary>
        /// Gets the <see cref="Token"/> object that was created when
        /// this object was referenced
        /// </summary>
        public Token Token { get; set; }

        public virtual string ToFileFormat(Token token = null)
        {
            return Token?.Value;
        }

        public override string ToString()
        {
            return Token?.Value ?? base.ToString();
        }
    }
}
