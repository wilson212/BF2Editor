namespace BF2ScriptingEngine.Scripting
{
    /// <summary>
    /// Represents a parsable entry in a <see cref="ConFile"/>. This class acts
    /// as a base for every object, statement and expression that comes from, 
    /// and can be put into a confile.
    /// </summary>
    /// <remarks>
    /// Objects are responsible for handling their own properties, therfor object
    /// property expressions do not extend from <see cref="ConFileEntry"/>
    /// </remarks>
    public abstract class ConFileEntry
    {
        /// <summary>
        /// Gets the <see cref="Token"/> object that was created when
        /// this object was referenced
        /// </summary>
        public Token Token { get; set; }

        /// <summary>
        /// Converts this object, statement or expression back into
        /// con file format
        /// </summary>
        /// <param name="token">
        /// If not null, this token will be used as a reference for building this
        /// entry's formatted string.
        /// </param>
        /// <returns></returns>
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
