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
        private static KeyValuePair<TokenType, string>[] TokenExpressions = new[]
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
                @"^(?<reference>[a-z]+)\.create(?<type>[a-z_]*)([\s|\t]+)(?<value>.*)$"
            ),

            new KeyValuePair<TokenType, string>(TokenType.ActiveSwitch,
                @"^(?<reference>[a-z]+)\.active(?<type>[a-z_]*)([\s|\t]+)(?<value>.*)?$"
            ),

            // Object property value, HAS TO FOLLOW everything else with a similar expression
            // due to the ungreediness of this regular expression
            new KeyValuePair<TokenType, string>(TokenType.ObjectProperty,
                @"^(?<reference>[a-z]+)\.(?<property>[a-z_]+)([\s|\t]+)(?<value>.*)?$"
            ),

            // === Vars Conditionals === //

            // If statements
            new KeyValuePair<TokenType, string>(TokenType.IfStart, @"^if(?<value>.*)?$"),

            // End If
            new KeyValuePair<TokenType, string>(TokenType.EndIf, @"^(?<value>.*)endIf$"),

            // Variable
            new KeyValuePair<TokenType, string>(TokenType.Variable, @"^var(?<value>.*)?$"),

            // Constant
            new KeyValuePair<TokenType, string>(TokenType.Constant, @"^const(?<value>.*)?$"),

            // Include command
            new KeyValuePair<TokenType, string>(TokenType.Include, @"^include(?<value>.*)?$"),

            // Run command
            new KeyValuePair<TokenType, string>(TokenType.Run, @"^run(?<value>.*)?$"),

            // Finally, match everything else
            new KeyValuePair<TokenType, string>(TokenType.None, @"^(?<value>.*?)$")
        };

        /// <summary>
        /// Contains our list of spliting characters
        /// </summary>
        public static readonly char[] SplitChars = new char[] { ' ', '\t' };

        /// <summary>
        /// A quote
        /// </summary>
        private const string QUOTE = "\"";

        /// <summary>
        /// Loads a .con or .ai file, and converts the script objects into C# objects
        /// </summary>
        /// <param name="filePath">The path to the .con / .ai file</param>
        /// <returns></returns>
        public static Task<ConFile> LoadFileAsync(string filePath, bool supressErrors = false)
        {
            return Task.Run(() =>
            {
                // Create new ConFile contents
                ConFile cFile = new ConFile(filePath);
                Dictionary<int, string> fileContents = new Dictionary<int, string>();
                int lineNum = 1;

                // Create Log
                Logger.Info("Loading file: " + filePath);

                // Open the confile, and read its contents
                using (FileStream fStream = new FileStream(filePath, FileMode.Open))
                using (StreamReader reader = new StreamReader(fStream))
                {
                    // While we can keep reading
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        fileContents.Add(lineNum++, line);
                    }
                }

                // Parse the contents of the Con / Ai file
                try
                {
                    Execute(fileContents, ref cFile);
                }
                catch
                {
                    // If we are not supressing errors, throw this
                    if (!supressErrors)
                    {
                        throw;
                    }

                    return null;
                }

                // Return the file
                return cFile;
            });
        }

        /// <summary>
        /// Parses the contents of a .con or .ai file, and converts the contents into C# objects
        /// </summary>
        /// <param name="fileContents">A dictionary of file contents [LineNumber => LineContents]</param>
        /// <param name="workingFile">A reference to the ConFile that contains the contents</param>
        public static void Execute(Dictionary<int, string> fileContents, ref ConFile workingFile)
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
            foreach(Token Tkn in Tokens)
            {
                // Split line into function call followed by and arguments
                tokenArgs = GetTokenArgs(Tkn.Value);

                // Create the object template
                ConFileObject template = CreateObjectType(tokenArgs, Tkn);
                ObjectType oType = ObjectManager.GetObjectType(template);

                // Make sure we are trying to create an object that is already created
                if (ObjectManager.ContainsObject(template.Name, oType))
                {
                    // Fetch the object
                    ConFileObject Obj = ObjectManager.GetObject(template.Name, oType);
                    string message = "Object \"" + template.Name + "\" has already been Initialized. "
                        + Environment.NewLine
                        + "Original object file: " 
                        + Obj.File.FilePath 
                        + " [" + Obj.Tokens[0].Position + "]"; // 0 index is always the defining index
                    Logger.Error(message, workingFile, Tkn.Position);
                    throw new Exception(message);
                }

                // Add the new object template to the ConFile objects list
                workingFile.AddEntry(template, Tkn);

                // Finally, register the object with the ObjectManager
                ObjectManager.RegisterObject(template);
                
                // Fire event
                Logger.Info(
                    "Created  " + template.ReferenceName + "  \"" + template.Name + "\"", 
                    workingFile, Tkn.Position);
            }

            // ============
            // Finally, we load all of the object properties, and assign them to their 
            // respective objects
            // ============

            // Create our needed objects
            RemComment comment = null;
            ConFileObject currentObj = null;
            ObjectType type;
            Dictionary<ObjectType, ConFileObject> lastObj = new Dictionary<ObjectType, ConFileObject>();
            StringBuilder builder = new StringBuilder();

            // We use a for loop here so we can skip rem blocks and statements
            for (int i = 0; i < fileTokens.Length; i++)
            {
                // Grab token value
                Token token = fileTokens[i];
                switch (token.Kind)
                {
                    case TokenType.ObjectStart:
                    case TokenType.ActiveSwitch:
                        // Split line into function call followed by and arguments
                        tokenArgs = GetTokenArgs(token.Value);
                        token.TokenArgs = tokenArgs;

                        // Fetch the object
                        type = ObjectManager.GetObjectType(tokenArgs.ReferenceName, workingFile, token.Position);
                        string objName = tokenArgs.Arguments.Last();

                        // Ensure our new working object has been referenced
                        if (!ObjectManager.ContainsObject(objName, type))
                        {
                            string error = $"Failed to load un-initialized object \"{objName}\"";
                            Logger.Error(error, workingFile, token.Position);
                            throw new Exception(error);
                        }

                        // Fetch our new working object.
                        currentObj = ObjectManager.GetObject(objName, type);

                        // Set comment if we have a value
                        if (token.Kind == TokenType.ObjectStart && comment != null)
                            currentObj.Comment = comment;

                        // === Objects are already added to the working file before hand === //
                        // Add object reference to file
                        if (token.Kind == TokenType.ActiveSwitch)
                            workingFile.AddEntry(currentObj, token);

                        // Set last loaded object
                        lastObj[type] = currentObj;

                        // Reset comment
                        comment = null;

                        // Log
                        Logger.Info($"Loading object properties for \"{objName}\"", 
                            workingFile, token.Position
                        );
                        break;
                    case TokenType.ObjectProperty:
                        // Convert args to an object
                        tokenArgs = GetTokenArgs(token.Value);
                        token.TokenArgs = tokenArgs;

                        // Make sure we have an object to work with and the object
                        // reference matches our current working object
                        if (currentObj == null || !currentObj.ReferenceName.Equals(
                            tokenArgs.ReferenceName, 
                            StringComparison.InvariantCultureIgnoreCase))
                        {
                            // ============
                            // If the object type does not match our current object type, and we didnt use a 
                            // .Active or .Create command, then we must be referencing an already defined object
                            // of a different type (IE: switching from ObjectTemplate to GeometryTemplate)
                            // ============
                            type = ObjectManager.GetObjectType(tokenArgs.ReferenceName);
                            if (lastObj.ContainsKey(type))
                            {
                                // switch the last active object of this type
                                currentObj = lastObj[type];
                            }
                            else
                            {
                                // If we are here, we have an issue...
                                string error = $"Failed to set property \"{tokenArgs.ReferenceName}.{tokenArgs.PropertyName}\""
                                    + ". No object reference set!";
                                Logger.Error(error, workingFile, token.Position);
                                throw new Exception(error);
                            }
                        }

                        // Let the object parse its own lines...
                        try
                        {
                            currentObj.Parse(token, comment);

                            // Ensure comment is null
                            comment = null;
                        }
                        catch (Exception e)
                        {
                            Logger.Error(e.Message, workingFile, token.Position);
                            ObjectManager.ReleaseAll(workingFile);
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
                    case TokenType.Run:
                    case TokenType.Include:
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
                    case TokenType.Constant:
                    case TokenType.Variable:
                        Expression exp = new Expression(token);
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
                            Logger.Error(message, workingFile, token.Position);
                            throw new Exception(message);
                        }
                        break;
                }
            }
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
                        inQuote = true;
                        builder.Append($"{part} ");
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
        /// <param name="endType"></param>
        /// <param name="fileTokens"></param>
        /// <param name="offset"></param>
        /// <param name="builder"></param>
        /// <returns>Returns the index of <paramref name="fileTokens"/> that we stopped at.</returns>
        private static int ScopeUntil(TokenType endType, Token[] fileTokens, int offset, StringBuilder builder)
        {
            // Get the open tag file token
            Token currentToken = fileTokens[offset];

            // Loop until we find the closing tag
            for (; offset < fileTokens.Length; offset++)
            {
                // Append line, no matter what it is
                Token token = fileTokens[offset];
                builder.AppendLine(token.Value);

                // We stop our journey here if we found the tag
                if (token.Kind == endType)
                    return offset;
            }

            // Log error
            string error = $"No closing tag found for \"{nameof(currentToken)}\" ({currentToken.Position}) found!";
            Logger.Error(error, currentToken.File, currentToken.Position);
            throw new Exception(error);
        }

        /// <summary>
        /// The Create command is used to inform the scripting engine that a new object 
        /// is going to be created. All properties and commands following a create command 
        /// will be applied to that object, until another create command or the end of the 
        /// file is encountered.
        /// </summary>
        /// <param name="Params"></param>
        /// <param name="File"></param>
        /// <param name="Tkn"></param>
        /// <returns></returns>
        private static ConFileObject CreateObjectType(TokenArgs tokenArgs, Token Tkn)
        {
            switch (tokenArgs.ReferenceName.ToLowerInvariant())
            {
                case "aitemplate": return AiTemplate.Create(tokenArgs, Tkn);
                case "aitemplateplugin": return AiTemplatePlugin.Create(tokenArgs, Tkn);
                case "weapontemplate": return WeaponTemplate.Create(tokenArgs, Tkn);
                case "objecttemplate": return ObjectTemplate.Create(tokenArgs, Tkn);
                case "aisettings":
                default:
                    string error = $"Reference call to '{tokenArgs.ReferenceName}' is not supported";
                    Logger.Error(error, Tkn.File, Tkn.Position);
                    throw new NotSupportedException(error);
            }
        }
    }
}
