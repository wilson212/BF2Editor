using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BF2ScriptingEngine;

namespace BF2Editor
{
    /// <summary>
    /// A NameSpace provides us an easy way to group
    /// <seealso cref="ConFile"/>s together
    /// </summary>
    public class NameSpace
    {
        /// <summary>
        /// Gets or Sets the name of this Namespace
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the working directory of this Namespace's physical files
        /// </summary>
        public string Directory { get; protected set; }

        /// <summary>
        /// Gets a list of loaded files, by name, that have been loaded
        /// into this Namespace
        /// </summary>
        public Dictionary<string, ConFile> Files { get; set; }

        /// <summary>
        /// Gets the Scope for this specific namespace
        /// </summary>
        protected Scope GlobalScope;

        /// <summary>
        /// Creates a new instance of <seealso cref="NameSpace"/>
        /// </summary>
        /// <param name="name">The name of this namespace</param>
        /// <param name="workingDirectory">
        /// The directory where the physical files that will be
        /// loaded into this namespace are located
        /// </param>
        public NameSpace(string name, string workingDirectory)
        {
            Name = name;
            Directory = workingDirectory;
            GlobalScope = new Scope();
            GlobalScope.MissingObjectHandling = MissingObjectHandling.CreateNew;
            Files = new Dictionary<string, ConFile>(StringComparer.InvariantCultureIgnoreCase);
        }

        public async Task<bool> LoadFile(string fileName, bool overwrite = false)
        {
            // Make sure this isnt loaded already...
            if (!overwrite && Files.ContainsKey(fileName))
                throw new Exception($"{fileName} has already been loaded into this namespace!");

            // Get full file path
            string corrected = fileName.Replace('/', Path.DirectorySeparatorChar);
            string filePath = Path.Combine(Directory, corrected);

            // Skip not found files
            if (!File.Exists(filePath))
                return false;

            // Try-Catch this beast
            try
            {
                // Create sub scope for this file, and load it
                Scope fileScope = CreateScope();
                ConFile cFile = await ScriptEngine.LoadFileAsync(filePath, fileScope);

                // Add item
                Files[fileName] = cFile;
                return true;
            }
            catch
            {
                throw;
            }
        }

        public async Task LoadFiles(string[] fileNames)
        {
            foreach (string file in fileNames)
            {
                await LoadFile(file);
            }
        }

        protected Scope CreateScope()
        {
            Scope scope = new Scope(GlobalScope, ScopeType.Detached);
            scope.MissingObjectHandling = MissingObjectHandling.CheckParent;
            return scope;
        }
    }
}
