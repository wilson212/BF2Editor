using BF2ScriptingEngine.Scripting;

namespace BF2ScriptingEngine.Scripting
{
    /// <summary>
    /// This object contains information about a reference
    /// to the supplied <see cref="ConFileObject"/>.
    /// </summary>
    public class ObjectReference : ConFileEntry
    {
        /// <summary>
        /// Gets the Object reference name (Ex: ObjectTemplate, weaponTempalte etc etc)
        /// </summary>
        public string ReferenceName => Token.TokenArgs.ReferenceName;

        /// <summary>
        /// Gets the <see cref="ConFileObject"/> that this reference points to.
        /// </summary>
        public ConFileObject Object { get; set; }

        public override string ToFileFormat(Token token = null)
        {
            Token tkn = token ?? Token;
            return Object.ToFileFormat(tkn);
        }
    }
}