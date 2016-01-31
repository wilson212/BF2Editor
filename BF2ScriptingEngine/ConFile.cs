using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        /// A list of objects that are referenced, but not created in this file
        /// </summary>
        /// <remarks>
        /// Contains any referenced objects (.Active and .activeSafe) that were created elsewhere.
        /// </remarks>
        public List<ObjectReference> References { get; protected set; }

        /// <summary>
        /// Gets a Key => Value map of expressions found in this file
        /// </summary>
        public Dictionary<string, Expression> Expressions { get; protected set; }

        /// <summary>
        /// A list of all found entries in this Con file. This include Objects, Object References,
        /// If, include, and run statements, as well as while loops
        /// </summary>
        public List<ConFileEntry> Entries { get; protected set; }

        /// <summary>
        /// Creates a new instance of ConFile
        /// </summary>
        /// <param name="filePath">The complete file path to this con file.</param>
        public ConFile(string filePath)
        {
            FilePath = filePath;
            Objects = new List<ConFileObject>();
            References = new List<ObjectReference>();
            Entries = new List<ConFileEntry>();
            Expressions = new Dictionary<string, Expression>();
        }

        /// <summary>
        /// Adds a new object to this confile, and returns it's reference
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public void AddEntry(ConFileEntry entry, Token token)
        {
            // Only add created objects to the Objects list
            if (token.Kind == TokenType.ObjectStart)
            {
                Objects.Add((ConFileObject)entry);
            }
            else if (token.Kind == TokenType.ActiveSwitch)
            {
                // Create a new reference and add it
                var reference = new ObjectReference() { Token = token, Object = (ConFileObject)entry };
                References.Add(reference);
            }
            else if (entry is Expression)
            {
                Expression exp = entry as Expression;
                if (!Expressions.ContainsKey(exp.Name))
                {
                    // Ensure that we are defined, or we are defining now!
                    // Note: v_arg{num} are always defined in scope! these are file arguments
                    if (!Regex.Match(exp.Name, "^v_arg[0-9]+$").Success)
                    {
                        if (!exp.Token.Value.StartsWithAny("var", "const"))
                        {
                            string err;
                            if (token.Kind == TokenType.Constant)
                                err = $"Undefined constant \"{exp.Name}\"";
                            else
                                err = $"Undefined variable \"{exp.Name}\"";

                            Logger.Error(err, this, token.Position);
                            throw new Exception(err);
                        }
                    }

                    // Add the expression
                    Expressions.Add(exp.Name, exp);
                }
            }

            // Always add the entry
            Entries.Add(entry);
        }

        /// <summary>
        /// Converts all the referenced objects and thier properties found in
        /// this confile into script (con) file format.
        /// </summary>
        /// <returns></returns>
        public string ToFileFormat()
        {
            StringBuilder builder = new StringBuilder();

            // Add defined objects and their properties
            foreach (ConFileEntry obj in Entries)
            {
                // Call the ToFileFormat method on the ConFileObject
                builder.AppendLine(obj.ToFileFormat());
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
