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

        /// <summary>
        /// Loads the file name specified, with a relative path from the 
        /// <see cref="NameSpace.Directory"/> property. Use a forward slash 
        /// to represent files in sub folders.
        /// </summary> 
        /// <param name="fileName">
        /// The fileName to load in this <see cref="NameSpace.Directory"/>, or sub
        /// folder path and filename from this NameSpace directory.
        /// </param>
        /// <param name="overwrite">
        /// If the file is already loaded in this NameSpace, do we overwrite the existing
        /// loaded file and its objects? If false, and Exception will be thrown if this file 
        /// is already loaded.
        /// </param>
        /// <returns>Returns true if the file exists and loaded successfully, false otherwise</returns>
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
                // Pass the exception up
                throw;
            }
        }

        /// <summary>
        /// Loads an array of files, with a relative path from the 
        /// <see cref="NameSpace.Directory"/> property.
        /// </summary>
        /// <param name="fileNames"></param>
        public async Task LoadFiles(string[] fileNames)
        {
            foreach (string file in fileNames)
            {
                await LoadFile(file);
            }
        }

        protected Scope CreateScope()
        {
            // Create a new Detached scope
            Scope scope = new Scope(GlobalScope, ScopeType.Detached);
            scope.MissingObjectHandling = MissingObjectHandling.CheckParent;
            return scope;
        }
    }
}
