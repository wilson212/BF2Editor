using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF2ScriptingEngine.Scripting
{
    public class TokenArgs
    {
        public string ReferenceName;

        public string PropertyName;

        public string[] Arguments;

        private string[] _properties;

        public string[] PropertyNames
        {
            get
            {
                if (_properties == null)
                    _properties = PropertyName.Split(new char[] { ' ', '\t' });

                return _properties;
            }
        }
    }
}
