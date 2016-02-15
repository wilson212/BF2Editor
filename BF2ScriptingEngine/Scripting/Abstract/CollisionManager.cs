using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF2ScriptingEngine.Scripting
{
    public class CollisionManager : ConFileObject
    {
        public CollisionManager(string Name, Token Token) : base(Name, Token) { }

        public static ConFileObject Create(Token token)
        {
            return new CollisionManager(token.TokenArgs.Arguments.Last(), token);
        }
    }
}
