using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF2ScriptingEngine
{
    /// <summary>
    /// Provides an instruction to a <see cref="Scope"/> to perform if the said
    /// Scope does not have a named <see cref="ConFileObject"/> defined.
    /// </summary>
    public enum MissingObjectHandling
    {
        /// <summary>
        /// Tells the Scope to just create a new local instance of the
        /// object if we cannot locate a reference in this <see cref="Scope"/>
        /// </summary>
        CreateNew,

        /// <summary>
        /// Tells the Scope to ask the parent <see cref="Scope"/> for
        /// the object if we cannot locate a reference in this <see cref="Scope"/>
        /// </summary>
        /// <remarks>
        /// Be sure to check the <see cref="ScopeType"/> of the current scope.
        /// If the ScopeType is Detached, then then the object we get from our
        /// parent Scope will be deeply cloned, and any changes made to that object
        /// inside this Scope will not affect the parents object reference and 
        /// vise-versa
        /// </remarks>
        CheckParent,

        /// <summary>
        /// Tells the Scope to throw an <see cref="Exception"/> if we
        /// cannot locate the requested object
        /// </summary>
        ThrowError
    }
}
