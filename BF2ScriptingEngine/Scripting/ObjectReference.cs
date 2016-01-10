using BF2ScriptingEngine.Scripting;

namespace BF2ScriptingEngine.Scripting
{
    /// <summary>
    /// This object contains information about a reference
    /// to the supplied <see cref="ConFileObject"/>.
    /// </summary>
    public class ObjectReference
    {
        /// <summary>
        /// Gets the <see cref="Token"/> object that was created when
        /// this object was referenced
        /// </summary>
        public Token Token { get; set; }

        /// <summary>
        /// Gets the Object reference name (Ex: ObjectTemplate, weaponTempalte etc etc)
        /// </summary>
        public string ReferenceName
        {
            get
            {
                string[] parts = Token.Value.Split(new char[] { '.' }, 2);
                return parts[0];
            }
        }

        /// <summary>
        /// Gets the <see cref="ConFileObject"/> that this reference points to.
        /// </summary>
        public ConFileObject Object { get; set; }
    }
}