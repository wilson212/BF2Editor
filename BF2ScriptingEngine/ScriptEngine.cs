﻿using System;
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

            // Object creation
            new KeyValuePair<TokenType, string>(TokenType.ObjectStart, 
                @"^(?<reference>[a-z]+)\.create([\s|\t]+)(?<value>.*)$"
            ),

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
        /// Gets a list of Assignable types for the GetObjectType method
        /// </summary>
        /// <remarks>
        /// This list just contains what we can parse so far...
        /// </remarks>
        private static Dictionary<TemplateType, Type> AssignableTypes = new Dictionary<TemplateType, Type>()
        {
            { TemplateType.ObjectTemplate, typeof(ObjectTemplate) },
            { TemplateType.WeaponTemplate, typeof(WeaponTemplate) },
            { TemplateType.AiTemplate, typeof(AiTemplate) },
            { TemplateType.AiTemplatePlugin, typeof(AiTemplatePlugin) },
            { TemplateType.KitTemplate, typeof(KitTemplate) },
            { TemplateType.GeometryTemplate, typeof(GeometryTemplate) },
        };

        /// <summary>
        /// Contains our list of spliting characters
        /// </summary>
        public static readonly char[] SplitChars = new char[] { ' ', '\t' };

        /// <summary>
        /// A quote
        /// </summary>
        private const string QUOTE = "\"";

        internal static Assembly Assembly { get; set; }

        static ScriptEngine()
        {
            Assembly = Assembly.GetExecutingAssembly();
        }

        /// <summary>
        /// Loads a .con or .ai file, and converts the script objects into C# objects
        /// </summary>
        /// <param name="filePath">The path to the .con / .ai file</param>
        /// <returns></returns>
        public static Task<ConFile> LoadFileAsync(
            string filePath, 
            Scope scope = null, 
            ExecuteInstruction runInstruction = ExecuteInstruction.Skip)
        {
            return Task.Run(async() =>
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
                await ParseFileLines(fileContents, cFile, runInstruction);

                // Return the file
                return cFile;
            });
        }

        /// <summary>
        /// Executes the the specified token on the specified scope
        /// </summary>
        /// <param name="token"></param>
        /// <param name="scope"></param>
        public static void ExecuteInScope(Token token, Scope scope)
        {
            // create properties
            TokenArgs tokenArgs;
            ConFileObject currentObj;

            switch (token.Kind)
            {
                case TokenType.ObjectStart:
                case TokenType.ActiveSwitch:
                    // Split line into function call followed by and arguments
                    tokenArgs = GetTokenArgs(token.Value);
                    token.TokenArgs = tokenArgs;

                    // Fetch our object
                    if (token.Kind == TokenType.ActiveSwitch)
                    {
                        currentObj = scope.GetObject(token);

                        // Set as the active object
                        scope.SetActiveObject(currentObj);
                    }
                    else
                    {
                        // Fetch our new working object.
                        currentObj = CreateObject(token);
                        scope.AddObject(currentObj, token);
                    }
                    break;
                case TokenType.ObjectProperty:
                    // Convert args to an object
                    tokenArgs = GetTokenArgs(token.Value);
                    token.TokenArgs = tokenArgs;

                    // Get the last used object
                    TemplateType type = GetTemplateType(tokenArgs.ReferenceName);
                    currentObj = scope.GetActiveObject(type);

                    // Make sure we have an object to work with and the object
                    // reference matches our current working object
                    if (currentObj == null)
                    {
                        // If we are here, we have an issue...
                        string error = $"Failed to set property \"{tokenArgs.ReferenceName}.{tokenArgs.PropertyName}\""
                            + ". No object reference set!";
                        Logger.Error(error, token.File, token.Position);
                        throw new Exception(error);
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
                        throw new Exception(message);
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

            // ============
            // Now we create an object reference of all objects in the Confile
            // before parsing the object properties, which **can** reference an object
            // in the same file before its defined
            // NOTE: Do not create object references for .Active and .safeActive
            // ============
            var Tokens = fileTokens.Where(x => x.Kind == TokenType.ObjectStart).OrderBy(x => x.Position);
            foreach (Token Tkn in Tokens)
            {
                // Split line into function call followed by and arguments
                Tkn.TokenArgs = GetTokenArgs(Tkn.Value);

                // Create the object template
                ConFileObject template = CreateObject(Tkn);

                // Finally, register the object with the ObjectManager
                workingFile.Scope.AddObject(template, Tkn);
                Logger.Info($"Created {template.ReferenceName} \"{template.Name}\"", workingFile, Tkn.Position);
            }

            // ============
            // Finally, we load all of the object properties, and assign them to their 
            // respective objects
            // ============

            // Create our needed objects
            RemComment comment = null;
            ConFileObject currentObj = null;
            TemplateType type;
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
                            // Split line into function call followed by and arguments
                            token.TokenArgs = GetTokenArgs(token.Value);
                            //token.Comment = comment;

                            // NOTE: the object was created before this loop!
                            currentObj = workingFile.Scope.GetObject(token);
                            workingFile.Scope.SetActiveObject(currentObj);

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
                            tokenArgs = GetTokenArgs(token.Value);
                            token.TokenArgs = tokenArgs;
                            //token.Comment = comment;

                            // Get the last used object
                            type = GetTemplateType(tokenArgs.ReferenceName);
                            currentObj = workingFile.Scope.GetActiveObject(type);

                            // Make sure we have an object to work with and the object
                            // reference matches our current working object
                            if (currentObj == null)
                            {
                                // If we are here, we have an issue...
                                string error = $"Failed to set property \"{tokenArgs.ReferenceName}.{tokenArgs.PropertyName}\""
                                    + ". No object reference set!";
                                throw new Exception(error);
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
                            Scope scp = workingFile.Scope;

                            // Are we executing in a new scope?
                            if (run == ExecuteInstruction.ExecuteInNewScope)
                            {
                                // For now, we just inherit the parent scope type
                                scp = new Scope(workingFile.Scope, workingFile.Scope.ScopeType);
                                scp.MissingObjectHandling = MissingObjectHandling.CheckParent;
                            }

                            // Get the filepath
                            string filePath = Path.GetDirectoryName(workingFile.FilePath);
                            string fileName = Path.Combine(filePath, stmt.FileName);

                            // Define file arguments
                            scp.SetArguments(stmt.Arguments);

                            // Load the file
                            ConFile include = await LoadFileAsync(fileName, scp, run);
                            workingFile.ExecutedIncludes.Add(include);
                            break;
                        case TokenType.Constant:
                        case TokenType.Variable:
                            // Set the new expression reference in Scope
                            Expression exp = new Expression(token);
                            workingFile.Scope.Expressions[exp.Name] = exp;

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
                                throw new Exception(message);
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
        /// Converts the value of a <see cref="Token"/> into an array of parameters.
        /// Any values that are qouted will remain intact
        /// </summary>
        /// <param name="tokenValue">The value of the token</param>
        /// <returns></returns>
        private static TokenArgs GetTokenArgs(string tokenValue)
        {
            // Begin our array builder
            TokenArgs tokenArgs = new TokenArgs();
            List<string> args = new List<string>();

            // Break the line into {0 => referenceCall, 1 => The rest of the line}
            // We only split into 2 strings, because some values have dots
            string[] temp = tokenValue.Split(new char[] { '.' }, 2);
            tokenArgs.ReferenceName = temp[0];

            // Split the line after the reference call into arguments
            string[] parts = temp[1].Split(SplitChars, StringSplitOptions.RemoveEmptyEntries);
            tokenArgs.PropertyName = parts[0];

            // Skip the property/function name
            parts = parts.Skip(1).ToArray();

            // Fix Quotes
            if (temp[1].Contains(QUOTE))
            {
                StringBuilder builder = new StringBuilder();
                bool inQuote = false;
                foreach (string part in parts)
                {
                    if (!inQuote && part.StartsWith(QUOTE))
                    {
                        if (part.EndsWith(QUOTE))
                        {
                            builder.Append($"{part}");

                            // Add the final quoted string as a single part
                            args.Add(builder.ToString());
                            builder.Clear();
                        }
                        else
                        {
                            inQuote = true;
                            builder.Append($"{part} ");
                        }
                    }
                    else if (inQuote && part.EndsWith(QUOTE))
                    {
                        inQuote = false;
                        builder.Append($"{part}");

                        // Add the final quoted string as a single part
                        args.Add(builder.ToString());
                        builder.Clear();
                    }
                    else
                    {
                        args.Add(part);
                    }
                }
            }
            else
                args.AddRange(parts);

            // Convert to array, because I was too lazy to recode all following code to reference a list :S
            tokenArgs.Arguments = args.ToArray();
            return tokenArgs;
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
            Logger.Error(error, currentToken.File, currentToken.Position);
            throw new Exception(error);
        }

        /// <summary>
        /// The Create command is used to inform the scripting engine that a new object 
        /// is going to be created. All properties and commands following a create command 
        /// will be applied to that object, until another create command or the end of the 
        /// file is encountered.
        /// </summary>
        /// <param name="tokenArgs">The token arguments that make up the create command line</param>
        /// <param name="Tkn">The token object from the <see cref="Tokenizer"/></param>
        /// <returns></returns>
        internal static ConFileObject CreateObject(Token Tkn)
        {
            TokenArgs tokenArgs = Tkn.TokenArgs;
            var type = GetTemplateType(tokenArgs.ReferenceName, Tkn.File, Tkn.Position);

            switch (type)
            {
                case TemplateType.AiTemplate: return AiTemplate.Create(tokenArgs, Tkn);
                case TemplateType.AiTemplatePlugin: return AiTemplatePlugin.Create(tokenArgs, Tkn);
                case TemplateType.WeaponTemplate: return WeaponTemplate.Create(tokenArgs, Tkn);
                case TemplateType.ObjectTemplate: return ObjectTemplate.Create(tokenArgs, Tkn);
                case TemplateType.KitTemplate: return KitTemplate.Create(tokenArgs, Tkn);
                case TemplateType.GeometryTemplate: return GeometryTemplate.Create(tokenArgs, Tkn);
                default:
                    string error = $"Reference call to '{tokenArgs.ReferenceName}' is not supported";
                    Logger.Error(error, Tkn.File, Tkn.Position);
                    throw new NotSupportedException(error);
            }
        }

        /// <summary>
        /// This method analysis a <see cref="ConFileObject"/> and returns
        /// the <see cref="TemplateType"/> representation of the object.
        /// </summary>
        /// <param name="obj"></param>
        public static TemplateType GetTemplateType(ConFileObject obj)
        {
            return GetTemplateType(obj.ReferenceName, obj.File, obj.Tokens[0]?.Position ?? 0);
        }

        /// <summary>
        /// This method analysis a <see cref="ConFileObject.ReferenceName"/> and returns
        /// the <see cref="TemplateType"/> representation of the object.
        /// </summary>
        /// <param name="referenceName">The reference string used to call upon this type of object</param>
        public static TemplateType GetTemplateType(string referenceName)
        {
            TemplateType type;
            if (!Enum.TryParse<TemplateType>(referenceName, true, out type))
            {
                string error = $"No TemplateType definition for \"{referenceName}\"";
                throw new Exception(error);
            }

            return type;
        }

        /// <summary>
        /// This method analysis a <see cref="ConFileObject.ReferenceName"/> and returns
        /// the <see cref="TemplateType"/> representation of the object.
        /// </summary>
        /// <param name="referenceName">The reference string used to call upon this type of object</param>
        /// <param name="file">Used in the <see cref="ScriptEngine"/></param>
        /// <param name="line">Used in the <see cref="ScriptEngine"/></param>
        public static TemplateType GetTemplateType(string referenceName, ConFile file, int line)
        {
            TemplateType type;
            if (!Enum.TryParse<TemplateType>(referenceName, true, out type))
            {
                string error = $"No TemplateType definition for \"{referenceName}\"";
                Logger.Error(error, file, line);
                throw new Exception(error);
            }

            return type;
        }

        /// <summary>
        /// This method returns the <see cref="TemplateType"/> representation
        /// of the supplied confile object type
        /// </summary>
        /// <param name="objType">The derived <see cref="ConFileObject"/> type.</param>
        /// <returns></returns>
        public static TemplateType GetTemplateType(Type objType)
        {
            foreach (KeyValuePair<TemplateType, Type> item in AssignableTypes)
            {
                if (item.Value.IsAssignableFrom(objType))
                    return item.Key;
            }

            throw new Exception("Invalid template type: " + objType.Name);
        }
    }
}
