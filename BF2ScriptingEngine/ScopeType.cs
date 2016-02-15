using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF2ScriptingEngine
{
    /// <summary>
    /// Determines how a <see cref="Scope"/> will handle <see cref="ConFileObject"/>
    /// references from other scopes.
    /// </summary>
    public enum ScopeType
    {
        /// <summary>
        /// Indicates that this <see cref="Scope"/> will keep References to Objects
        /// obtained from the parent <see cref="Scope"/>.
        /// </summary>
        /// <remarks>
        /// With this option, objects fetched from other scopes will retain their
        /// reference in memory, meaning that any changes to that shared object 
        /// will reflect on all other scopes that use that object.
        /// </remarks>
        Attached,

        /// <summary>
        /// Indicates that any Objects fetched from a Parent <see cref="Scope"/> will 
        /// be Deeply cloned, and NOT retain their Reference
        /// </summary>
        /// <remarks>
        /// This is the recommended option if you plan on saving changes back into a
        /// script file via <see cref="ConFile.Save()"/>. This way, changes made outside of
        /// this local scope donnot cause changes to other objects in other scopes
        /// that share the parent scope, thus having undesired results.
        /// </remarks>
        Detached
    }
}
