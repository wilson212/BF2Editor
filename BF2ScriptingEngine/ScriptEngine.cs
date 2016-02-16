using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BF2ScriptingEngine.Scripting;

namespace BF2ScriptingEngine
{
    /// <summary>
    /// The ScriptEngine is used to parse Battlefield 2 con/Ai file scripts,
    /// and convert those scripts into Type-Strong C# objects
    /// </summary>
    /// <seealso cref="http://bf2tech.org/index.php/ConGrammar"/>
    public static class ScriptEngine
    {
        /// <summary>
        /// An array of all the expressions needed to tokenize the contents
        /// of a Con file.
        /// </summary>
        /// <remarks>
        /// Order is important here, as we itterate through these expressions, lines
        /// that match an expression are removed from our source. As we get to the bottom
        /// of this array, the source will get smaller and smaller as we match our expressions.
        /// </remarks>
        internal static KeyValuePair<TokenType, string>[] TokenExpressions = new[]
        {
            // === Parse comments first, as somethings like objects and properties can be commented out === //

            // Multiline Rem Comment on a single line
            new KeyValuePair<TokenType, string>(TokenType.RemComment,
                @"^beginRem(?<value>.*?)endRem$"
            ),

            // Multiline Rem Comment START
            new KeyValuePair<TokenType, string>(TokenType.BeginRem,
                @"^beginRem(?<value>.*?)$"
            ),

            // Multiline Rem Comment END
            new KeyValuePair<TokenType, string>(TokenType.EndRem,
                @"^(?<value>.*)endRem$"
            ),

            // Single line Rem Comment
            new KeyValuePair<TokenType, string>(TokenType.RemComment,
                @"^rem([\s|\t]+)(?<value>.*)?$"
            ),

            // === Objects === //

            new KeyValuePair<TokenType, string>(TokenType.ActiveSwitch,
                @"^(?<reference>[a-z]+)\.active(?<type>[a-z_]*)([\s|\t]+)(?<value>.*)?$"
            ),

            // Object property value, HAS TO FOLLOW everything else with a similar expression
            // due to the ungreediness of this regular expression
            new KeyValuePair<TokenType, string>(TokenType.ObjectProperty,
                @"^(?<reference>[a-z]+)\.(?<property>[a-z_\.]+)([\s|\t]+)(?<value>.*)?$"
            ),

            // === Vars Conditionals === //

            // If statements
            new KeyValuePair<TokenType, string>(TokenType.IfStart, @"^if(?<value>.*)?$"),

            // End If
            new KeyValuePair<TokenType, string>(TokenType.EndIf, @"^(?<value>.*)endIf$"),

            // Variable
            new KeyValuePair<TokenType, string>(TokenType.Variable, @"^(?:var[\s\t]+)(?<name>[a-z0-9_]+)[\s\t]*(?:=[\s\t]*)?(?<value>.*?)?$"),
            new KeyValuePair<TokenType, string>(TokenType.Variable, @"^(?<name>[a-z0-9_]+)[\s\t]*(?:=[\s\t]*)(?<value>.*?)$"),

            // Constant
            new KeyValuePair<TokenType, string>(TokenType.Constant, @"^(?:const[\s\t]+)(?<name>[a-z0-9_]+)[\s\t]*(?:=[\s\t]*)(?<value>.*?)$"),

            // Include command
            new KeyValuePair<TokenType, string>(TokenType.Include, @"^include(?<value>.*)?$"),

            // Run command
            new KeyValuePair<TokenType, string>(TokenType.Run, @"^run(?<value>.*)?$"),

            // Finally, match everything else
            new KeyValuePair<TokenType, string>(TokenType.None, @"^(?<value>.*?)$")
        };

        /// <summary>
        /// Gets a list of characters used to split a confile line into arguments
        /// </summary>
        public static readonly char[] SplitChars = new char[] { ' ', '\t' };

        /// <summary>
        /// Loads a .con or .ai file, and converts the script objects into C# objects
        /// </summary>
        /// <param name="filePath">The path to the .con / .ai file</param>
        /// <param name="scope">
        /// Sets the scope in which the objects from this <see cref="ConFile"/> 
        /// will be loaded into.
        /// </param>
        /// <param name="runInstruction">
        /// Defines how the script engine should handle nested file includes
        /// </param>
        /// <exception cref="Exception">
        /// Thrown if there is a problem loading the script file in any way.
        /// </exception>
        /// <returns>Returns the parsed bf2 script file.</returns>
        public static async Task<ConFile> LoadFileAsync(string filePath, Scope scope = null, 
            ExecuteInstruction runInstruction = ExecuteInstruction.Skip)
        {
            // Create new ConFile contents
            ConFile cFile = new ConFile(filePath, scope);
            Dictionary<int, string> fileContents = new Dictionary<int, string>();
            int lineNum = 1;

            // Create Log
            Logger.Info("Loading file: " + filePath);

            // Open the confile, and read its contents
            using (FileStream fStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (StreamReader reader = new StreamReader(fStream))
            {
                // While we can keep reading
                while (!reader.EndOfStream)
                {
                    string line = await reader.ReadLineAsync();
                    fileContents.Add(lineNum++, line);
                }
            }

            // Parse the contents of the Con / Ai file
            try
            {
                await ParseFileLines(fileContents, cFile, runInstruction);
            }
            catch
            {
                // Pass the exception up
                throw;
            }

            // Return the file
            return cFile;
        }

        /// <summary>
        /// Executes the the specified token on the specified scope
        /// </summary>
        /// <param name="token"></param>
        /// <param name="scope"></param>
        public static void ExecuteInScope(Token token, Scope scope)
        {
            // create properties
            ConFileObject currentObj;

            switch (token.Kind)
            {
                case TokenType.ObjectStart:
                case TokenType.ActiveSwitch:
                    // Fetch our object
                    if (token.Kind == TokenType.ActiveSwitch)
                    {
                        // Fetch the object and Set as the active object
                        currentObj = scope.GetObject(token);
                        scope.SetActiveObject(currentObj);
                    }
                    else
                    {
                        // Get our method
                        var Method = token.TokenArgs.ReferenceType.GetMethod(
                            token.TokenArgs.PropertyName
                        );

                        // Fetch our new working object.
                        currentObj = Method.Invoke(token);
                        scope.AddObject(currentObj, token);
                    }

                    // Add file entry?
                    if (token.File != null)
                    {
                        // see if the object exists in file
                        if (token.File.Entries.Contains(currentObj))
                            token.File.AddEntry(currentObj, token);
                    }
                    break;
                case TokenType.ObjectProperty:
                    // Get the last used object
                    ReferenceType type = token.TokenArgs.ReferenceType;
                    currentObj = scope.GetActiveObject(type);

                    // Make sure we have an object to work with and the object
                    // reference matches our current working object
                    if (currentObj == null)
                    {
                        // If we are here, we have an issue...
                        string error = $"Failed to set property \"{token.TokenArgs.ReferenceType.Name}.\""
                            + $"{token.TokenArgs.PropertyName}. No object reference set!";
                        Logger.Error(error, token.File, token.Position);
                        throw new ParseException(error, token);
                    }

                    // Let the object parse its own lines...
                    try
                    {
                        currentObj.Parse(token);
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e.Message, token.File, token.Position, e);
                        throw;
                    }
                    break;
                case TokenType.RemComment:
                case TokenType.BeginRem:
                case TokenType.IfStart:
                case TokenType.Run:
                case TokenType.Include:
                    break;
                case TokenType.Constant:
                case TokenType.Variable:
                    Expression exp = new Expression(token);
                    scope.Expressions[exp.Name] = exp;
                    break;
                case TokenType.None:
                    // Throw error if the line is not empty
                    if (!String.IsNullOrWhiteSpace(token.Value))
                    {
                        string message = $"Unable to parse file entry \"{token.Value}\" on line {token.Position}";
                        Logger.Error(message, token.File, token.Position);
                        throw new ParseException(message, token);
                    }
                    break;
            }
        }

        /// <summary>
        /// Parses the contents of a .con or .ai file, and converts the contents into C# objects
        /// </summary>
        /// <param name="fileContents">A dictionary of file contents [LineNumber => LineContents]</param>
        /// <param name="workingFile">A reference to the ConFile that contains the contents</param>
        internal static async Task ParseFileLines(
            Dictionary<int, string> fileContents, 
            ConFile workingFile, 
            ExecuteInstruction run)
        {
            // ============
            // First we convert our confile lines into parsed tokens
            // ============
            Token[] fileTokens = Tokenizer.Tokenize(workingFile, ref fileContents, TokenExpressions);
            TokenArgs tokenArgs;
            Scope currentScope = workingFile.Scope;

            // ============
            // Now we create an object reference of all objects in the Confile
            // before parsing the object properties, which **can** reference an object
            // in the same file before its defined
            // NOTE: Do not create object references for .Active and .safeActive
            // ============
            foreach (Token token in fileTokens.Where(x => x.Kind == TokenType.ObjectStart).OrderBy(x => x.Position))
            {
                // Create the object
                var Method = token.TokenArgs.ReferenceType.GetMethod(token.TokenArgs.PropertyName);
                ConFileObject template = Method.Invoke(token);

                // Finally, register the object with the ObjectManager
                currentScope.AddObject(template, token);
                Logger.Info($"Created {token.TokenArgs.ReferenceType} \"{template.Name}\"", workingFile, token.Position);
            }

            // ============
            // Finally, we load all of the object properties, and assign them to their 
            // respective objects
            // ============

            // Create our needed objects
            RemComment comment = null;
            ConFileObject currentObj = null;
            ReferenceType type;
            var builder = new StringBuilder();

            // We use a for loop here so we can skip rem blocks and statements
            for (int i = 0; i < fileTokens.Length; i++)
            {
                // Grab token value
                Token token = fileTokens[i];
                try
                {
                    switch (token.Kind)
                    {
                        case TokenType.ObjectStart:
                        case TokenType.ActiveSwitch:
                            // NOTE: the object was created before this loop!
                            currentObj = currentScope.GetObject(token);
                            currentScope.SetActiveObject(currentObj);

                            // === Objects are already added to the working file before hand === //
                            // Add object reference to file
                            workingFile.AddEntry(currentObj, token);

                            // Reset comment
                            comment = null;

                            // Log
                            Logger.Info($"Loading object properties for \"{currentObj.Name}\"",
                                workingFile, token.Position
                            );
                            break;
                        case TokenType.ObjectProperty:
                            // Convert args to an object
                            tokenArgs = token.TokenArgs;

                            // Get the last used object
                            type = tokenArgs.ReferenceType;
                            currentObj = currentScope.GetActiveObject(type);

                            // Make sure we have an object to work with and the object
                            // reference matches our current working object
                            if (currentObj == null)
                            {
                                // If we are here, we have an issue...
                                string error = $"Failed to set property \"{token.TokenArgs.ReferenceType.Name}.\""
                                    + $"{token.TokenArgs.PropertyName}. No object reference set!";
                                throw new ParseException(error, token);
                            }

                            // Let the object parse its own lines...
                            try
                            {
                                currentObj.Parse(token);

                                // Ensure comment is null
                                comment = null;
                            }
                            catch (Exception e)
                            {
                                Logger.Error(e.Message, workingFile, token.Position, e);
                                throw;
                            }
                            break;
                        case TokenType.RemComment:
                            // Create a new comment if we need to
                            if (comment == null)
                            {
                                comment = new RemComment(token);
                            }

                            // Add comment to the current string
                            comment.AppendLine(token.Value);
                            break;
                        case TokenType.BeginRem:
                            RemComment rem = new RemComment(token);
                            rem.IsRemBlock = true;

                            // Skip every line until we get to the endRem
                            builder.AppendLine(token.Value);
                            i = ScopeUntil(TokenType.EndRem, fileTokens, i, builder);

                            // Set rem value
                            rem.Value = builder.ToString().TrimEnd();
                            workingFile.AddEntry(rem, rem.Token);

                            // Clear the string builder
                            builder.Clear();
                            break;
                        case TokenType.IfStart:
                            Statement statement = new Statement(token);
                            if (token.Kind == TokenType.IfStart)
                            {
                                // Skip every line until we get to the endIf
                                builder.AppendLine(token.Value);
                                i = ScopeUntil(TokenType.EndIf, fileTokens, i, builder);

                                // Clear the string builder
                                statement.Token.Value = builder.ToString().TrimEnd();
                                builder.Clear();
                            }

                            // Add entry
                            workingFile.AddEntry(statement, statement.Token);
                            break;
                        case TokenType.Run:
                        case TokenType.Include:
                            // Just add to as a string
                            RunStatement stmt = new RunStatement(token);
                            workingFile.AddEntry(stmt, stmt.Token);

                            // Do we execute the statement?
                            if (run == ExecuteInstruction.Skip)
                                continue;

                            // Create new scope for execution
                            Scope runScope = currentScope;

                            // Are we executing in a new scope?
                            if (run == ExecuteInstruction.ExecuteInNewScope)
                            {
                                // For now, we just inherit the parent scope type
                                runScope = new Scope(currentScope, currentScope.ScopeType);
                                runScope.MissingObjectHandling = MissingObjectHandling.CheckParent;
                            }

                            // Get the filepath
                            string filePath = Path.GetDirectoryName(workingFile.FilePath);
                            string fileName = Path.Combine(filePath, stmt.FileName);

                            // Define file arguments
                            runScope.SetArguments(stmt.Arguments);

                            // Load the file
                            try
                            {
                                ConFile include = await LoadFileAsync(fileName, runScope, run);
                                workingFile.ExecutedIncludes.Add(include);
                            }
                            catch (FileNotFoundException) // Only acceptable exception
                            {
                                fileName = Path.GetFileName(fileName);
                                Logger.Warning($"Failed to run file \"{fileName}\". File Not Found", 
                                    workingFile, token.Position);
                            }
                            break;
                        case TokenType.Constant:
                        case TokenType.Variable:
                            // Set the new expression reference in Scope
                            Expression exp = new Expression(token);
                            currentScope.Expressions[exp.Name] = exp;

                            // Add expression to the confile as well
                            workingFile.AddEntry(exp, exp.Token);
                            break;
                        case TokenType.None:
                            // Dont attach comment to a property if we have an empty line here
                            if (comment != null)
                            {
                                workingFile.AddEntry(comment, comment.Token);
                                comment = null;
                            }

                            // Throw error if the line is not empty
                            if (!String.IsNullOrWhiteSpace(token.Value))
                            {
                                string message = $"Unable to parse file entry \"{token.Value}\" on line {token.Position}";
                                throw new ParseException(message, token);
                            }
                            break;
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e.Message, token.File, token.Position, e);
                    throw;
                }
            }

            // Finalize this confile
            workingFile.Finish();
        }

        /// <summary>
        /// Starting at <paramref name="offset"/>, this method will not parse any of the 
        /// <paramref name="fileTokens"/> until the current Token type matches the 
        /// <paramref name="endType"/> specified. All contents starting at the offset are stored
        /// into the <paramref name="builder"/> provided.
        /// </summary>
        /// <remarks>This method DOES support nested statements</remarks>
        /// <param name="endType">The token type we skip to</param>
        /// <param name="fileTokens">The file tokens we are itterating through</param>
        /// <param name="offset">The starting offset in the <paramref name="fileTokens"/></param>
        /// <param name="builder">The string builder object we are using to storing the contents 
        /// i nbetween the <paramref name="offset"/> and <paramref name="endType"/></param>
        /// <returns>Returns the index of <paramref name="fileTokens"/> that we stopped at.</returns>
        private static int ScopeUntil(TokenType endType, Token[] fileTokens, int offset, StringBuilder builder)
        {
            // Get the open tag file token
            Token currentToken = fileTokens[offset];

            // Loop until we find the closing tag
            for (int i = offset + 1; i < fileTokens.Length; i++)
            {
                // Append line, no matter what it is
                Token token = fileTokens[i];

                // Add value
                builder.AppendLine(token.Value);

                // Search for nested types
                switch (token.Kind)
                {
                    case TokenType.IfStart:
                        i = ScopeUntil(TokenType.EndIf, fileTokens, i, builder);
                        break;
                    case TokenType.BeginRem:
                        i = ScopeUntil(TokenType.EndRem, fileTokens, i, builder);
                        break;
                }

                // We stop our journey here if we found the tag
                if (token.Kind == endType)
                    return i;
            }

            // Log error
            string error = $"No closing tag found for \"{currentToken.Kind}\" ({currentToken.Position}) found!";
            //Logger.Error(error, currentToken.File, currentToken.Position);
            throw new ParseException(error, currentToken);
        }
    }
}
