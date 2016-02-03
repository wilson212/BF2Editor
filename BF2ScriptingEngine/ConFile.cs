using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BF2ScriptingEngine.Scripting;
using BF2ScriptingEngine.Scripting.Attributes;

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
        public Dictionary<string, List<Expression>> Expressions { get; protected set; }

        /// <summary>
        /// A list of all found entries in this Con file. This include Objects, Properties, 
        /// Methods, Object References, If/include/run statements, as well as while 
        /// loops.
        /// </summary>
        /// <remarks>
        /// By keeping a list of all entries in this file, we can ensure ordering 
        /// in the ToFileFormat() method to an extent... Originally I had assinged 
        /// a priority attribute that would be used to order properties, but this 
        /// didn't help with things like comments, and if/while statments that were
        /// weaved in between object properties ("Objects_Server.zip -> /Kits/US/US_common.con" for example)
        /// </remarks>
        public List<ConFileEntry> Entries { get; protected set; }

        /// <summary>
        /// Indicates whether this ConFile has finished parsing
        /// </summary>
        protected bool Finalized = false;

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
            Expressions = new Dictionary<string, List<Expression>>();
        }

        /// <summary>
        /// Adds a new object to this confile, and returns it's reference
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public void AddEntry(ConFileEntry entry, Token token)
        {
            if (token.Kind == TokenType.RemComment)
            {
                return;
            }
            else if (token.Kind == TokenType.ObjectStart)
            {
                Objects.Add((ConFileObject)entry);
            }
            else if (token.Kind == TokenType.ActiveSwitch)
            {
                // Create a new reference and add it
                var reference = new ObjectReference() { Token = token, Object = (ConFileObject)entry };
                References.Add(reference);

                // Set entry to the object reference
                entry = reference;
            }
            else if (entry is Expression)
            {
                Expression exp = entry as Expression;
                if (!Expressions.ContainsKey(exp.Name))
                {
                    // Ensure that we are defined, or we are defining now!
                    // Note: v_arg{num} are always defined in scope! these are file arguments
                    //if (!Regex.Match(exp.Name, "^v_arg[0-9]+$").Success)
                    //{
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
                    //}

                    // Add the expression
                    Expressions.Add(exp.Name, new List<Expression>() { exp });
                }
            }

            // Always add the entry
            Entries.Add(entry);
        }

        internal void AddProperty(ObjectProperty property)
        {
            // To speed up the initial parsing, we only insert
            // to position if we are finalized
            if (Finalized)
            {
                // Make sure this entry doesnt already exist
                if (Entries.IndexOf(property) > 0)
                    throw new Exception("The specified ObjectProperty already exists in this confile.");

                // Find the owner object
                int index = Entries.IndexOf(property.Owner) + 1;
                if (index == 0)
                    throw new Exception("The property owner does not exist in this ConFile");

                // Search for next object
                for (int i = index + 1; i < Entries.Count; i++)
                {
                    TokenType kind = Entries[i].Token.Kind;
                    if (kind == TokenType.ActiveSwitch || kind == TokenType.ObjectStart)
                    {
                        Entries.Insert(i, property);
                        return;
                    }
                }
            }

            // If we are here, just append to the end
            Entries.Add(property);
        }

        internal void AddPropertyAfter(ObjectProperty property, ConFileEntry afterItem)
        {
            // Make sure this entry doesnt already exist
            if (Entries.IndexOf(property) > 0)
                throw new Exception("The specified ObjectProperty already exists in this confile.");

            // Find the owner object
            int index = Entries.IndexOf(afterItem) + 1;
            if (index == 0)
                throw new Exception("The specified \"afterObject\" does not exist in this ConFile");

            // Insert item
            if (index == Entries.Count)
                Entries.Add(property);
            else
                Entries.Insert(index, property);
        }

        /// <summary>
        /// Gets the Reference of an Expression, that occurs before the specified
        /// entry.
        /// </summary>
        /// <param name="name">The name of the variable or constant expression</param>
        /// <param name="beforeEntry">
        /// The entry where this variable or constant expression is referenced
        /// </param>
        /// <returns>
        /// Returns the last reference value of the specifed expression, occuring
        /// before the <paramref name="beforeEntry"/>
        /// </returns>
        public Expression GetExpressionReference(string name, ConFileEntry beforeEntry)
        {
            // Check to see if this expression exists
            if (!Expressions.ContainsKey(name))
                goto Undefined;

            // Grab the last reference before this confile entry
            int index = Entries.IndexOf(beforeEntry);
            if (index == -1)
                throw new Exception("The specified entry does not exist in this ConFile");

            // Now find the last set expression reference value
            for (int i = index - 1; i >= 0; i--)
            {
                ConFileEntry entry = Entries[i];
                TokenType kind = entry.Token.Kind;
                if (kind == TokenType.Constant || kind == TokenType.Variable)
                {
                    // cast to Expression
                    Expression exp = entry as Expression;
                    if (exp.Name == name)
                    {
                        // This expression hasnt been assigned yet.. shame
                        /***
                         * We have no way yet to determine if a variable is assigned
                         * inside an If/While statement, so skip for now
                         *
                         * if (String.IsNullOrWhiteSpace(exp.Value))
                            throw new Exception($"Value cannot be null; Expression \"{name}\" is unassigned");
                         */

                        return exp;
                    }
                }
            }

            // If we are here, the expression was not defined yet
            Undefined:
            {
                string err;
                if (name.StartsWith("c"))
                    err = $"Undefined constant \"{name}\"";
                else
                    err = $"Undefined variable \"{name}\"";

                throw new Exception(err);
            }
        }

        /// <summary>
        /// Converts all the referenced objects and thier properties found in
        /// this confile into script (con) file format.
        /// </summary>
        /// <remarks>
        /// Please see the remarks on the "Entries" List above
        /// </remarks>
        /// <returns></returns>
        public string ToFileFormat()
        {
            StringBuilder builder = new StringBuilder();
            int counter = 1;

            // Add defined objects and their properties
            foreach (ConFileEntry obj in Entries)
            {
                if (obj.Token.Kind == TokenType.ObjectProperty)
                {
                    // Get the value of the property
                    ObjectProperty property = obj as ObjectProperty;

                    // Skip null values and properties that are defined/set elsewhere
                    if (property == null || property.Token.File.FilePath != this.FilePath)
                        continue;

                    // Ensure the value is not null
                    string formated = property.ToFileFormat();
                    if (String.IsNullOrWhiteSpace(formated))
                        continue;

                    // Hard coded comments?
                    PropertyInfo propInfo = property.Property;
                    Comment comments = propInfo.GetCustomAttribute(typeof(Comment)) as Comment;
                    if (!String.IsNullOrEmpty(comments?.Before))
                        builder.AppendLine($"rem {comments.Before}");

                    // Write the property reference and value
                    builder.AppendLine(formated);

                    // Apply Before comment
                    if (!String.IsNullOrEmpty(comments?.After))
                        builder.AppendLine($"rem {comments.After}");
                }
                else
                {
                    // Ensure the value is not null
                    string formated = obj.ToFileFormat();
                    if (!String.IsNullOrWhiteSpace(formated))
                    {
                        // Space out token types
                        if (counter > 1)
                            builder.AppendLine();

                        // Call the ToFileFormat method on the ConFileObject
                        builder.AppendLine(formated.TrimEnd());
                    }
                }

                counter++;
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

        internal void Finish()
        {
            // Check for unasigned values
            foreach (var item in Expressions)
            {
                // If we just have a defining expression, with no value ever...
                if (item.Value.Count == 1)
                {
                    Expression exp = (Expression)item.Value[0];
                    if (String.IsNullOrWhiteSpace(exp.Value))
                    {
                        string prefix = (exp.Token.Kind == TokenType.Constant)
                            ? "constant"
                            : "variable";

                        // Log as a warning
                        Logger.Warning(
                            $"The {prefix} \"{exp.Name}\" is never assigned a value.",
                            this,
                            exp.Token.Position
                        );
                    }
                }
            }

            // WE parsed successfully
            Finalized = true;
        }
    }
}
