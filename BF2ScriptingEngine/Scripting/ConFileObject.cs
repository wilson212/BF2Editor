using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using BF2ScriptingEngine.Scripting.Attributes;

namespace BF2ScriptingEngine.Scripting
{
    /// <summary>
    /// Represents an object found within a con or ai script file
    /// </summary>
    /// <remarks>
    /// Inheriting classes make their own variables using <see cref="PropertyName"/>
    /// attributes to clarify that the variable represents a bf2 object property.
    /// 
    /// Inheriting objects will then use the SetValue() methods to set
    /// the values in their ObjectProperty fields using reflection.
    /// 
    /// Property Value Types:
    ///  - A <see cref="ObjectPropertyList{T1}"/> represents that the property is used multiple times,
    ///    and each item will be seperated onto a new line. Ex: addTemplatePlugin. The
    ///    Generic Type {T} must be a supported value.
    ///    NOTE: if the list is meant to display its index before its values, then the 
    ///      IndexedCollection attribute should be attached to the Property.
    ///  - A <see cref="ObjectPropertyDict{TKey, TVal}"/> is used much like a List, wheras the
    ///    property can be used multiple times to add values, and will be seperated onto a new line,
    ///    however this collection uses a key as an identifier. Ex: setStrength. The Generic Type {TVal} 
    ///    must be a supported value.
    ///    
    /// Supported values (single value, no Arrays or IEnumerables):
    ///  - Int32
    ///  - Double (Keeps up to 4 decimals, removes trailing zeros)
    ///  - Decimal (Keeps up to 16 decimals, does NOT remove trailing zeroes)
    ///  - String
    ///  - Bool used as an int ( 1 or 0 )
    ///  - Enum
    ///  - ConFileObject (Only if the the ObjectProperty has 1 argument)
    /// </remarks>
    public abstract class ConFileObject : ConFileEntry
    {
        /// <summary>
        /// The Con or Ai File that contains this Object
        /// </summary>
        public ConFile File { get; protected set; }

        /// <summary>
        /// The name of this Object given in the create command
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Gets or Sets the comment if there is one
        /// </summary>
        public RemComment Comment { get; protected set; }

        /// <summary>
        /// A collection of detached versions of this object. Any changes made to this object
        /// outside of its defined file, are stored here.
        /// </summary>
        /// <remarks>
        /// [RelaPath => ObjectReference]
        /// </remarks>
        public List<ConFileObject> References { get; protected set; }

        /// <summary>
        /// Contains a map of field info's for types
        /// </summary>
        public static Dictionary<string, Dictionary<string, PropertyInfo>> PropertyMap;

        static ConFileObject()
        {
            PropertyMap = new Dictionary<string, Dictionary<string, PropertyInfo>>();
        }

        /// <summary>
        /// Creates a new instance of <see cref="ConFileObject"/>
        /// </summary>
        /// <param name="name">The unique name of this object</param>
        /// <param name="token">
        /// The token which creates this object. If null, an attempt to make a token will be
        /// made by getting using this objects <see cref="ReferenceType"/>.
        /// </param>
        public ConFileObject(string name, Token token = null)
        {
            Type type = this.GetType();

            // Check for null token!
            if (token == null)
            {
                // === We need a token, so try and create one === //
                token = CreateToken(name, type, token.File);
            }

            // Set object variables
            this.Name = name;
            this.File = token.File;
            base.Token = token;

            // Build our property Map
            if (!PropertyMap.ContainsKey(type.Name))
                PropertyMap.Add(type.Name, GetPropertyMap(type));
        }

        /// <summary>
        /// This method is responsible for parsing an object property
        /// reference line from inside of a .con or .ai file, and converting
        /// the property into a C# property
        /// </summary>
        /// <remarks>
        /// The type of value is figured out within the method, but only certain types of
        /// object types can be parsed here.
        /// </remarks>
        /// <param name="token">The token for this ObjectProperty</param>
        /// <param name="objectLevel">Specifies the nested object level for this Property</param>
        public virtual void Parse(Token token, int objectLevel = 0)
        {
            // Seperate our property name and values
            TokenArgs tokenArgs = token.TokenArgs;
            string propName = tokenArgs.PropertyNames[objectLevel];

            // Fetch our property that we are setting the value to
            KeyValuePair<string, PropertyInfo> prop = GetProperty(propName);
            Type propType = prop.Value.PropertyType;
            bool isCollection = propType.GetInterface("IObjectPropertyCollection") != null;

            // Get the value that is set
            var value = prop.Value.GetValue(this);

            // Is this an object method, or Object Property?
            if (propType.BaseType.Name == "ObjectMethod")
            {
                // Object methods are always instantiated in the object's constructor
                ObjectMethod method = (ObjectMethod)value;
                ConFileEntry item = method.Invoke(token);

                // Add item to the file entry list
                if (item != null)
                    token.File?.AddEntry(item, token);
            }
            else
            {
                // If the value is null, then we create a new object property instance
                ObjectProperty obj = (ObjectProperty)value;
                if (obj == null)
                {
                    // Create instance and add it to the entries list
                    obj = ObjectProperty.Create(prop.Value, this, token);

                    // Add entry? Property Collections add thier own properties
                    if (!isCollection)
                        token.File?.AddProperty(obj);
                }

                // Create our instance, and parse
                obj.SetValue(token, objectLevel);
                prop.Value.SetValue(this, obj);
            }
        }

        /// <summary>
        /// Sets the value(s) of the specified property by name. If the 
        /// <see cref="ObjectProperty"/> is null, then a new instance is
        /// created with the inital values provided.
        /// </summary>
        /// <remarks>
        /// This method should only be used when the ObjectProperty is null 
        /// outside of the ScriptEngine, since it uses heavy relection.
        /// </remarks>
        /// <param name="propertyName">The <see cref="PropertyName"/> to assign</param>
        /// <param name="values">The values to set in the <see cref="ObjectProperty"/></param>
        public void SetValue(string propertyName, params object[] values)
        {
            // Fetch our property that we are setting the value to
            KeyValuePair<string, PropertyInfo> prop = GetProperty(propertyName);

            // If the first values argument is null, then we remove this property
            if (values == null)
            {
                Remove(prop.Value);
                return;
            }

            // If the value is null, then we create a new object property instance
            var obj = (ObjectProperty)prop.Value.GetValue(this);
            if (obj == null)
            {
                // === 
                // Since the property is null, we must create a con script command.
                // So here, we will use the ReferenceType of this object, and property
                // name to create a fake token, and use that to set the value
                // ===
                StringBuilder builder = new StringBuilder();
                builder.Append($"{Token.TokenArgs.ReferenceType.Name}.{propertyName}");
                foreach (object value in values)
                    builder.Append($" {value}");

                // Create a new Token
                Token token = Tokenizer.Tokenize(builder.ToString());
                token.File = File;

                // Create the ObjectProperty instance, and set this Property to that instance
                obj = ObjectProperty.Create(prop.Value, this, token);

                // Add entry? IObjectPropertyCollection's add thier own properties
                Type type = obj.GetType();
                if (type.GetInterface("IObjectPropertyCollection") == null)
                    Token.File?.AddProperty(obj);
            }

            // Set the new object property values
            obj.SetValues(values);
            prop.Value.SetValue(this, obj);
        }

        /// <summary>
        /// Removes the <see cref="ObjectProperty"/> isntance from this object.
        /// As a result, the property will be excluded when calling <see cref="ConFileObject.ToFileFormat"/>
        /// and the <see cref="ConFile.Save"/> methods
        /// </summary>
        /// <param name="propertyName"></param>
        public void Remove(string propertyName)
        {
            // Fetch our property that we are setting the value to
            KeyValuePair<string, PropertyInfo> prop = GetProperty(propertyName);
            Remove(prop.Value);
        }

        /// <summary>
        /// Removes the <see cref="ObjectProperty"/> isntance from this object.
        /// As a result, the property will be excluded when calling <see cref="ConFileObject.ToFileFormat"/>
        /// and the <see cref="ConFile.Save"/> methods
        /// </summary>
        protected void Remove(PropertyInfo property)
        {
            // Grab the current value
            var obj = (ObjectProperty)property.GetValue(this);
            if (obj == null) return;

            // Set the value to null
            property.SetValue(this, null);

            // Remove entry
            File?.Entries.Remove(obj);
        }

        /// <summary>
        /// Converts this <see cref="ConFileObject"/> into the proper script format,
        /// including the properties that were referenced in the <param name="reference"/>
        /// parameter.
        /// </summary>
        public override string ToFileFormat() => ToFileFormat(Token);

        /// <summary>
        /// Converts this <see cref="ConFileObject"/> into the proper script format,
        /// including the properties that were referenced in the <param name="reference"/>
        /// parameter.
        /// </summary>
        /// <remarks>
        /// The order in which properties are listed is based on 2 factors:
        /// 1. The order in which the properties are defined in the C# object file
        /// 2. The property attribute <see cref="PropertyName.Priority"/> value.
        /// </remarks>
        /// <param name="token">
        /// IF a token is supplied, we will only include properties that match the token filepath
        /// with the filepath of the objects properties.
        /// </param>
        public string ToFileFormat(Token token)
        {
            // Define our type
            Type objType = this.GetType();

            // Make sure we have a token
            Token tkn = token ?? Token;

            // Components use our own token!
            if (typeof(IComponent).IsAssignableFrom(objType))
                tkn = Token;

            // Appy our reference (.create || .active || .activeSafe) line
            return tkn.Value;
        }

        /// <summary>
        /// Returns the field info for the specified property name
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public KeyValuePair<string, PropertyInfo> GetProperty(string fieldName)
        {
            // Fetch our property that we are setting the value to
            // Note: Property map is built in the ctor, key exists fine!
            Type type = this.GetType();
            var prop = PropertyMap[type.Name].FirstOrDefault(
                x => x.Key.Equals(fieldName, StringComparison.InvariantCultureIgnoreCase)
            );

            // Make sure this property is supported
            if (prop.Equals(default(KeyValuePair<string, PropertyInfo>)))
                ThrowUnsuportedField(fieldName);

            return prop;
        }

        /// <summary>
        /// This method is used to map PropertyName attributes to C# object properties.
        /// This allows us to use Reflection to set property values, using con file property
        /// tags
        /// </summary>
        /// <param name="objectType">The type definition of the calling instance</param>
        /// <returns></returns>
        protected Dictionary<string, PropertyInfo> GetPropertyMap(Type objectType)
        {
            // Create our return Map
            Dictionary<string, PropertyInfo> Properties = new Dictionary<string, PropertyInfo>();
            PropertyInfo[] Fields = objectType.GetProperties(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
            );

            // Loop through each property, and search for the custom attribute
            foreach (PropertyInfo property in Fields)
            {
                // If the Custom attribute exists, we add it to the Mapping
                Attribute attribute = Attribute.GetCustomAttribute(property, typeof(PropertyName));
                if (attribute != null)
                {
                    PropertyName fieldAttr = attribute as PropertyName;

                    // Check for Duplicates
                    foreach (string name in fieldAttr.Names)
                    {
                        if (Properties.ContainsKey(name))
                        {
                            string Message = $"Duplicate PropertyName found \"{name}\" in {objectType.FullName}";
                            Logger.Error(Message, this.File, this.Token.Position);
                            throw new Exception(Message);
                        }

                        Properties.Add(name, property);
                    }
                }
            }

            return Properties;
        }

        /// <summary>
        /// Throws a generic Exception with an Invalid property name message
        /// </summary>
        /// <param name="propertyName"></param>
        protected void ThrowUnsuportedField(string propertyName)
        {
            throw new Exception("Invalid Object Property \"" + propertyName + "\".");
        }

        /// <summary>
        /// Creates a new token instance, that can be used to create a new
        /// instance of this object type.
        /// </summary>
        /// <param name="name">The unique name of the object</param>
        /// <param name="type">The derived type of this object</param>
        /// <returns></returns>
        private static Token CreateToken(string name, Type type, ConFile file)
        {
            Token token;

            // NOTE: an exception will be thrown in method if this object
            // type isnt added to the ReferenceManager
            var refType = ReferenceManager.GetReferenceType(type);
            var method = refType.Mappings.FirstOrDefault();

            // If we have a Mapping, we assume a non-static object
            if (method.Key != null && method.Value != null)
            {
                // Build a create string... this is pretty Generic
                // and may not cover custom types very well!!!
                string input = $"{refType.Name}.{method.Key} {type.Name} {name}";

                // Create a new Token
                token = Tokenizer.Tokenize(input);
                token.File = file;
            }
            else
            {
                // Must be a static class, just create a very generic token!
                token = new Token()
                {
                    Kind = TokenType.StaticObject,
                    File = file,
                    Value = name,
                    TokenArgs = new TokenArgs()
                    {
                        Arguments = new string[0],
                        ReferenceType = refType
                    }
                };
            }

            return token;
        }

        /// <summary>
        /// Creates a new, empty object of this type
        /// </summary>
        /// <returns></returns>
        public ConFileObject Clone(ConFile file = null)
        {
            // Create a new token, with no reference to our current one
            Type type = GetType();
            Token newToken = CreateToken(Name, type, file ?? new ConFile(this.File?.FilePath));

            // Return a new instance of this object, with the same name
            return (ConFileObject)Activator.CreateInstance(type, this.Name, newToken);
        }

        public override string ToString() => Name;
    }
}
