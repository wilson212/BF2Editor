﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BF2ScriptingEngine.Scripting;

namespace BF2ScriptingEngine
{
    /// <summary>
    /// A Scope acts as a local environment, that holds an array of 
    /// objects and expressions that have been created and referenced 
    /// within it.
    /// </summary>
    /// <remarks>
    /// As defined on: https://www.cs.cf.ac.uk/Dave/PERL/node52.html
    /// 
    /// Scope refers to the visibility of variables (as well as Objects). 
    /// In other words, which parts of your program can see or use it
    /// </remarks>
    public class Scope
    {
        /// <summary>
        /// Contains a list of all objects registered in this scope
        /// </summary>
        internal Dictionary<Tuple<string, ReferenceType>, ConFileObject> Objects;

        /// <summary>
        /// Gets a Key => Value map of expressions found in this file
        /// </summary>
        public Dictionary<string, Expression> Expressions { get; protected set; }

        /// <summary>
        /// Gets the parent <see cref="Scope"/>, if any, that this scope falls under
        /// </summary>
        public Scope ParentScope { get; protected set; }

        /// <summary>
        /// Gets the Scope Type
        /// </summary>
        public ScopeType ScopeType { get; protected set; }

        /// <summary>
        /// Gets the action to perform if this Scope does not have a 
        /// named <see cref="ConFileObject"/> defined.
        /// </summary>
        public MissingObjectHandling MissingObjectHandling { get; set; } = MissingObjectHandling.CreateNew;

        /// <summary>
        /// Gets or Sets whether this Scope is required to pass created objects
        /// up to the parent <see cref="Scope"/> if one is defined
        /// </summary>
        /// <remarks>
        /// The ScopeType will be respected, if this Scope is Detached, then a clone
        /// copy of the object will be givin to the parent scope
        /// </remarks>
        public bool RegisterObjects { get; set; } = false;

        /// <summary>
        /// Keeps a list of all active objects, for each template type
        /// </summary>
        internal Dictionary<ReferenceType, ConFileObject> ActiveObjects;

        /// <summary>
        /// Creates a new instance <see cref="Scope"/>
        /// </summary>
        public Scope() : this(null, ScopeType.Detached) { }

        /// <summary>
        /// Creates a new instance of <see cref="Scope"/> using the 
        /// specified parent <see cref="Scope"/> and attachment
        /// </summary>
        /// <param name="parentScope">The parent Scope that this Scope falls under</param>
        /// <param name="type">The method of attachment to our parent.</param>
        public Scope(Scope parentScope, ScopeType type)
        {
            // parentScope cannot be null if we are attached!
            if (parentScope == null && type == ScopeType.Attached)
            {
                throw new ArgumentNullException("parentScope", 
                    "Parent scope must be defined when using ScopeType.Attached!"
                );
            }

            // Set internals
            ParentScope = parentScope;
            ScopeType = type;

            // Get our active objects, if any
            if (ScopeType == ScopeType.Attached)
            {
                // Just use the parent's reference
                ActiveObjects = ParentScope.ActiveObjects;

                // Create new references because what is added to this Scope, should
                // NOT be added to the parents scope as well!!!
                var c = new ObjectEqualityComparer();
                Objects = new Dictionary<Tuple<string, ReferenceType>, ConFileObject>(ParentScope.Objects, c);
                Expressions = new Dictionary<string, Expression>(ParentScope.Expressions);
            }
            else
            {
                // Create new instances
                var c = new ObjectEqualityComparer();
                ActiveObjects = new Dictionary<ReferenceType, ConFileObject>();
                Objects = new Dictionary<Tuple<string, ReferenceType>, ConFileObject>(c);
                Expressions = new Dictionary<string, Expression>();

                // Copy over the active objects from our parent
                if (ParentScope != null)
                {
                    // Perform a deep copy since we are Detached
                    foreach (var item in ParentScope.ActiveObjects)
                    {
                        try
                        {
                            ActiveObjects[item.Key] = item.Value.Clone();
                        }
                        catch
                        {
                            throw;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Invokes the specified script command
        /// </summary>
        /// <param name="input">The ConFile formated command to execute in this scope</param>
        /// <param name="file">If a file is specified, the result of this command will also be saved
        /// in the <see cref="ConFile"/> specified here, so it can be stored when saved.</param>
        /// <example>
        ///     input => "ObjectTemplate.activeSafe GenericFireArm chrif_type95"
        /// </example>
        public void Execute(string input, ConFile file = null) 
        {
            // === Create Token
            Token token = Tokenizer.Tokenize(input);
            token.File = file;

            // === Execute on Scope
            ScriptEngine.ExecuteInScope(token, this);
        }

        /// <summary>
        /// Adds a <see cref="ConFileObject"/> to this Scope
        /// </summary>
        /// <param name="obj">The object reference to add</param>
        /// <param name="setActive">
        /// Set this as the active object of its <see cref="ReferenceType"/>?
        /// </param>
        public void AddObject(ConFileObject obj, bool setActive = true)
        {
            // Ensure that we aren't passing peoples problems
            CheckObjectToken(obj);

            // Generate key
            var type = obj.Token.TokenArgs.ReferenceType;
            var key = new Tuple<string, ReferenceType>(obj.Name, type);

            // If object exists, throw exception
            if (ContainsObject(key))
                throw new Exception("Object is already defined in this scope!");

            // Do we set as the active object?
            if (setActive)
                SetActiveObject(obj);

            // Register object?
            if (ParentScope != null && RegisterObjects)
            {
                if (ScopeType == ScopeType.Attached)
                    ParentScope.AddObject(obj, setActive);
                else
                    ParentScope.AddObject(obj.Clone(), setActive);
            }

            // Add object
            Objects.Add(key, obj);
        }

        /// <summary>
        /// Adds a <see cref="ConFileObject"/> to this Scope
        /// </summary>
        /// <remarks>
        /// Used by the script engine to add objects, and set them as active
        /// </remarks>
        /// <param name="obj"></param>
        /// <param name="token"></param>
        internal void AddObject(ConFileObject obj, Token token)
        {
            // Ensure that we aren't passing peoples problems
            CheckObjectToken(obj);

            // Generate key
            var type = obj.Token.TokenArgs.ReferenceType;
            var key = new Tuple<string, ReferenceType>(obj.Name, type);

            // If object exists, then we fetch the existing reference
            if (ContainsObject(key))
            {
                // Log warning
                string err = $"Object \"{obj.Name}\" is already defined, Setting existing one as active";
                Logger.Warning(err, token.File, token.Position);

                // Get existing reference
                obj = GetObject(key, token);
            }
            else
            {
                // Add object
                Objects.Add(key, obj);

                // Register object?
                if (ParentScope != null && RegisterObjects)
                {
                    if (ScopeType == ScopeType.Attached)
                        ParentScope.AddObject(obj, token);
                    else
                        ParentScope.AddObject(obj.Clone(), token);
                }
            }

            // Set object as active
            SetActiveObject(obj);
        }

        /// <summary>
        /// Determines whether the specified object (not reference) exists in this scope.
        /// </summary>
        /// <param name="obj">The object we are searching for</param>
        /// <returns>true if the object exists in this scope, otherwise false</returns>
        public bool ContainsObject(ConFileObject obj)
        {
            // Ensure that we aren't passing peoples problems
            CheckObjectToken(obj);

            // Check for object existance
            var type = obj.Token.TokenArgs.ReferenceType;
            var key = new Tuple<string, ReferenceType>(obj.Name, type);
            return ContainsObject(key);
        }

        /// <summary>
        /// Determines whether the specified object name and type exists in this scope.
        /// </summary>
        /// <param name="name">The object name we are searching for</param>
        /// <param name="type">The <see cref="ReferenceType"/> of the object</param>
        /// <returns>true if the object exists in this scope, otherwise false</returns>
        public bool ContainsObject(string name, Type type)
        {
            // Ensure that we have a valid Reference Type
            var ttype = ReferenceManager.GetReferenceType(type);
            if (ttype == null)
            {
                throw new Exception($"Unrecognized object type passed \"{type.Name}\"");
            }

            // Create our key
            var key = new Tuple<string, ReferenceType>(name, ttype);
            return ContainsObject(key);
        }

        /// <summary>
        /// Determines whether the specified object by key exists in this scope.
        /// </summary>
        /// <returns>true if the object exists in this scope, otherwise false</returns>
        public bool ContainsObject(Tuple<string, ReferenceType> key)
        {
            // First, check this scope
            if (Objects.ContainsKey(key))
            {
                return true;
            }
            else
            {
                // Check parent scope if settings allow us
                if (ParentScope != null && MissingObjectHandling == MissingObjectHandling.CheckParent)
                    return ParentScope.ContainsObject(key);
                else
                    return false;
            }
        }

        /// <summary>
        /// Sets the active object in this scope, of its base template type
        /// </summary>
        /// <remarks>The object DOES NOT need to exist in this scope, but must in a parent scope</remarks>
        /// <param name="obj">The object we are activating</param>
        public void SetActiveObject(ConFileObject obj)
        {
            // Ensure that we aren't passing peoples problems
            CheckObjectToken(obj);
            ReferenceType type = obj.Token.TokenArgs.ReferenceType;
            ActiveObjects[type] = obj;
        }

        /// <summary>
        /// Gets the current active object of the supplied type
        /// </summary>
        /// <param name="type"></param>
        /// <returns>Returns the active object if one exists for the supplied type, or null</returns>
        public ConFileObject GetActiveObject(ReferenceType type)
        {
            if (ActiveObjects.ContainsKey(type))
                return ActiveObjects[type];

            return null;
        }

        /// <summary>
        /// Attempts to fetch a <see cref="ConFileObject"/>, using the
        /// provided <see cref="ScopeOptions"/>
        /// </summary>
        /// <param name="token"></param>
        internal ConFileObject GetObject(Token token)
        {
            // Pull info
            try
            {
                TokenArgs tokenArgs = token.TokenArgs;
                string name = token.TokenArgs.Arguments.Last();

                // Create our Objects key
                var type = tokenArgs.ReferenceType;
                var key = new Tuple<string, ReferenceType>(name, type);
                return GetObject(key, token);
            }
            catch
            {
                throw;
            }

            
        }

        /// <summary>
        /// Fetches a <see cref="ConFileObject"/> by key, using the provided
        /// provided <see cref="ScopeOptions"/> defined in this scope
        /// </summary>
        /// <param name="key"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        internal ConFileObject GetObject(Tuple<string, ReferenceType> key, Token token)
        {
            // Check internal first!
            if (!Objects.ContainsKey(key))
            {
                // Pull into scope
                switch (MissingObjectHandling)
                {
                    case MissingObjectHandling.CreateNew:
                        // Get our method
                        var Method = token.TokenArgs.ReferenceType.GetMethod(token.TokenArgs.PropertyName);

                        // Create the object template
                        Objects[key] = Method.Invoke(token);
                        break;
                    case MissingObjectHandling.CheckParent:
                        // If we dont have a parent to ask, throw an error
                        if (ParentScope == null)
                            goto default;

                        // Ask the parent for the object
                        if (ScopeType == ScopeType.Attached)
                        {
                            // remember, we are Attached, so we keep the object reference!
                            Objects[key] = ParentScope.GetObject(key, token);
                        }
                        else
                        {
                            // === Deep Clone === //
                            Objects[key] = ParentScope.GetObject(key, token).Clone();
                        }
                        break;
                    default:
                        string error = $"Failed to load un-initialized object \"{key.Item1}\"";
                        Logger.Error(error, token?.File, token?.Position ?? 0);
                        throw new Exception(error);
                }
            }

            return Objects[key];
        }

        /// <summary>
        /// Gets the object of the specifed type and returns it. If the object
        /// does not exist, the default value of <typeparamref name="T"/> is 
        /// returned instead
        /// </summary>
        /// <typeparam name="T">The type of object to fetch; Must derive from ConFileObject</typeparam>
        /// <param name="name">The name of the object (case sensitive)</param>
        /// <returns>Returns the requested object if found, otherwise null</returns>
        public T GetObject<T>(string name) where T : ConFileObject
        {
            // Fetch our object type, and make sure its valid
            var type = ReferenceManager.GetReferenceType(typeof(T));
            if (type == null)
            {
                throw new Exception($"Unrecognized object type passed \"{typeof(T).Name}\"");
            }

            // Create our key
            var key = new Tuple<string, ReferenceType>(name, type);

            // Make sure the object exists before causing an exception!
            if (ContainsObject(key))
                return (T)(object)GetObject(key, null);

            return default(T);
        }

        /// <summary>
        /// Fetches the loaded object by th specified name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        internal ConFileObject GetObject(string name, Type type, Token token)
        {
            // Get the object type
            var ttype = ReferenceManager.GetReferenceType(type);
            if (type == null)
            {
                throw new Exception($"Unrecognized object type passed \"{type.Name}\"");
            }

            // Create our key
            var key = new Tuple<string, ReferenceType>(name, ttype);
            return GetObject(key, token);
        }

        /// <summary>
        /// Returns an array of all the loaded objects
        /// </summary>
        public ConFileObject[] GetObjects()
        {
            return Objects.Values.ToArray();
        }

        /// <summary>
        /// Returns an array of all the loaded objects that meet the specified criteria
        /// </summary>
        /// <param name="Where">A criteria method that returns a bool, determining whether the object
        /// will be returned or not.</param>
        public ConFileObject[] GetObjects(Func<ConFileObject, bool> Where)
        {
            List<ConFileObject> Objs = new List<ConFileObject>();

            foreach (ConFileObject obj in Objects.Values)
            {
                // Check if the method meets the criteria
                if (Where.Invoke(obj))
                    Objs.Add(obj);
            }

            return Objs.ToArray();
        }

        /// <summary>
        /// Sets the file arguments for this scope
        /// </summary>
        /// <param name="args"></param>
        public void SetArguments(string[] args)
        {
            // Add arg definitions
            if (args != null && args.Length > 0)
            {
                Token token = new Token() { Kind = TokenType.Variable };
                for (int i = 1; i <= args.Length; i++)
                {
                    Expression exp = new Expression(token);
                    Expressions[$"v_arg{i}"] = exp;
                }
            }
        }

        /// <summary>
        /// If any file arguments were supplied, they are returned
        /// </summary>
        /// <returns></returns>
        public Expression[] GetArguments()
        {
            List<Expression> args = new List<Expression>();
            foreach (var exp in Expressions)
            {
                if (exp.Key.StartsWith("v_arg"))
                    args.Add(exp.Value);
            }

            return args.ToArray();
        }

        /// <summary>
        /// Ensures that our token is valid
        /// </summary>
        /// <param name="obj"></param>
        private void CheckObjectToken(ConFileObject obj)
        {
            // Ensure that we aren't passing peoples problems
            if (obj?.Token?.TokenArgs == null)
            {
                throw new ArgumentNullException(
                    "obj.Token.TokenArgs",
                    $"TokenArgs are not defined on \"{obj.Name}\"!"
                );
            }
            else if (obj.Token.TokenArgs.ReferenceType == null)
            {
                throw new ArgumentNullException(
                    "obj.Token.TokenArgs.ReferenceType",
                    $"ReferenceType is not defined on \"{obj.Name}\"!"
                );
            }
        }
    }
}
