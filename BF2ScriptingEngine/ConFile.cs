using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BF2ScriptingEngine.Scripting;

namespace BF2ScriptingEngine
{
    /// <summary>
    /// Represents a battlefield 2 .con or .ai script file.
    /// </summary>
    /// <remarks>
    /// Con files are what brings everything together in Battlefield 2. 
    /// Con files link together the objects, models, sounds, maps, AI, and 
    /// settings in the game. These are text files, sometimes called script files, 
    /// with each line defining a property or performing a directive.
    /// </remarks>
    /// <seealso cref="http://bfmods.com/mdt/scripting/Intro.html"/>
    public class ConFile
    {
        /// <summary>
        /// Relative file path from the Objects_server root folder
        /// </summary>
        public string FilePath { get; protected set; }

        /// <summary>
        /// A list of found objects that were created in this Con file
        /// </summary>
        /// <remarks>Only objects created with the ".create" method are stored here!</remarks>
        public List<ConFileObject> Objects { get; protected set; }

        /// <summary>
        /// A list of objects and their reference points found in this con file
        /// </summary>
        /// <remarks>
        /// Contains all objects in the <see cref="Objects"/> List, as well as any referenced
        /// objects (.Active and .activeSafe) that were created elsewhere.
        /// </remarks>
        public List<ObjectReference> References { get; protected set; }

        /// <summary>
        /// Creates a new instance of ConFile
        /// </summary>
        /// <param name="filePath">The complete file path to this con file.</param>
        public ConFile(string filePath)
        {
            FilePath = filePath;
            Objects = new List<ConFileObject>();
            References = new List<ObjectReference>();
        }

        /// <summary>
        /// Adds a new object to this confile, and returns it's reference
        /// </summary>
        /// <param name="conObject"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public ObjectReference AddObject(ConFileObject conObject, Token token)
        {
            // Only add created objects to the Objects list
            if (token.Kind == TokenType.ObjectStart)
                Objects.Add(conObject);

            // Always add the reference, no matter the type
            return CreateReference(conObject, token);
        }

        /// <summary>
        /// Creates a new reference point for the specied object.
        /// </summary>
        /// <param name="conObject"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        protected ObjectReference CreateReference(ConFileObject conObject, Token token)
        {
            // Create a new reference and add it
            var reference = new ObjectReference() { Token = token, Object = conObject };
            References.Add(reference);
            return reference;
        }

        /// <summary>
        /// Converts all the referenced objects and thier properties found in
        /// this confile into script (con) file format.
        /// </summary>
        /// <returns></returns>
        public string ToFileFormat()
        {
            StringBuilder builder = new StringBuilder();

            // Add defined properties
            foreach (var obj in References.OrderBy(x => x.Token.Position))
            {
                builder.AppendLine(obj.Object.ToFileFormat(obj));
                builder.AppendLine();
            }

            return builder.ToString().TrimEnd();
        }

        /// <summary>
        /// Saves the changes made to the objects contained in this fileobject
        /// </summary>
        public void Save()
        {
            // Convert our objects to script format
            string lines = ToFileFormat();

            // Open the file and write all the contents
            using (FileStream stream = new FileStream(FilePath, FileMode.Create))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                writer.Write(lines);
            }
        }
    }
}
