using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
    ///  - A <see cref="List{T}"/> represents that the property is used multiple times,
    ///    and each item will be seperated onto a new line. Ex: addTemplatePlugin. The
    ///    Generic Type {T} must be a supported value.
    ///    NOTE: if the list is meant to display its index before its values, then the 
    ///      IndexedCollection attribute should be attached to the Property.
    ///  - An Array of T represents that the property has multiple arguments for its value,
    ///    and uses just 1 line for all of its arguments. This can only be a single dimmension
    ///    of Double, Int32, String, or an internal Enum.
    ///  - A <see cref="Dictionary{TKey, TValue}"/> is used much like a List, wheras the
    ///    property can be used multiple times to add values, and will be seperated onto a new line,
    ///    however this collection uses a key as an identifier. Ex: setStrength. The Generic Type {TVal} 
    ///    must be a supported value.
    ///    
    /// Supported values (single dimmension array of the follwing or are single value):
    ///  - Int32
    ///  - Double
    ///  - Decimal
    ///  - String
    ///  - Bool used as an int ( 1 or 0 )
    ///  - Internal Enum
    ///  - ObjectProperty{T}
    /// </remarks>
    public abstract class ConFileObject
    {
        /// <summary>
        /// The Con or Ai File that contains this Object
        /// </summary>
        public ConFile File;

        /// <summary>
        /// The name of this Object given in the create command
        /// </summary>
        public string Name;

        /// <summary>
        /// The reference string used to reference this object entry
        /// </summary>
        /// <remarks>
        /// Ex: AiSettings.setMaxNumBots 32 :: AiSettings is the Reference Name
        /// Ex: [ReferenceName].[PropertyName] [Values seperated by a space/tab]
        /// </remarks>
        public string ReferenceName;

        /// <summary>
        /// Gets or Sets the comment if there is one
        /// </summary>
        public RemComment Comment;

        /// <summary>
        /// Gets a list of <see cref="Token"/> objects, which represent all reference points
        /// where this object is used at.
        /// </summary>
        public List<Token> Tokens;

        /// <summary>
        /// Contains a map of field info's for types
        /// </summary>
        public static Dictionary<string, Dictionary<string, FieldInfo>> PropertyMap;

        static ConFileObject()
        {
            PropertyMap = new Dictionary<string, Dictionary<string, FieldInfo>>();
        }

        /// <summary>
        /// Creates a new instance of ConFileObject
        /// </summary>
        /// <param name="name">The name of this object</param>
        /// <param name="referenceAs">How this object is referenced (ObjectTemplate, weaponTemplate etc etc)</param>
        /// <param name="token">The token which creates this object</param>
        public ConFileObject(string name, string referenceAs, Token token)
        {
            this.Name = name;
            this.File = token.File;
            this.Tokens = new List<Token>() { token };
            this.ReferenceName = referenceAs;
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
        /// <param name="ValueParams">An array of values for the property found in the .con file</param>
        /// <param name="token">The token for this ObjectProperty</param>
        /// <param name="comment">The Rem comment for this ObjectProperty if there is one</param>
        public virtual void Parse(string[] ValueParams, Token token, RemComment comment)
        {
            // Seperate our property name and values
            string propName = ValueParams[0];
            string[] values;

            // Fetch our property that we are setting the value to
            KeyValuePair<string, FieldInfo> prop = GetField(propName);

            // Skip the first param value for indexed lists
            if (prop.Value.FieldType.IsGenericType)
            {
                Attribute attribute = Attribute.GetCustomAttribute(prop.Value, typeof(IndexedList));
                values = (attribute == null) ? ValueParams.Skip(1).ToArray() : ValueParams.Skip(2).ToArray();
            }
            else
                values = ValueParams.Skip(1).ToArray();

            // Get the value that is set
            var value = prop.Value.GetValue(this);
            ObjectPropertyBase obj;

            // If the value is null, then we create a new object property instance
            if (value == null)
                obj = ObjectPropertyBase.Create(prop.Value, token, comment);
            else
                obj = (ObjectPropertyBase)value;
            

            // Create our instance, and parse
            obj.SetValueFromParams(values);
            prop.Value.SetValue(this, obj);
        }

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
        public virtual string ToFileFormat(ObjectReference reference)
        {
            // Use a string builder to append our values
            StringBuilder builder = new StringBuilder();

            // Apply comment to this object if there was one preceeding this object
            if (!String.IsNullOrWhiteSpace(Comment?.Value))
                builder.AppendLine(Comment.Value.TrimEnd());

            // Appy our reference (.create || .active || .activeSafe) line
            builder.AppendLine(reference.Token.Value);

            // Order our properties by the PropertyName.Priority value
            Type propertyType = typeof(PropertyName);
            var ordered = PropertyMap[this.GetType().Name].Values.OrderBy(
                x => (x.GetCustomAttribute(propertyType) as PropertyName).Priority
            );

            // Add defined properties
            foreach (FieldInfo field in ordered)
            {
                // Get the value of the property
                var property = field.GetValue(this) as ObjectPropertyBase;

                // Skip null values and properties that are defined/set elsewhere
                if (property == null || property.Token.File.FilePath != reference.Token.File.FilePath)
                    continue;

                // Write the property reference and value
                builder.AppendLine(property.ToFileFormat(reference, field));
            }

            return builder.ToString().TrimEnd();
        }

        /// <summary>
        /// Returns the field info for the specified property name
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public KeyValuePair<string, FieldInfo> GetField(string fieldName)
        {
            Type type = this.GetType();
            if (!PropertyMap.ContainsKey(type.Name))
                PropertyMap.Add(type.Name, GetPropertyMap(type));

            // Fetch our property that we are setting the value to
            var prop = PropertyMap[type.Name].FirstOrDefault(
                x => x.Key.Equals(fieldName, StringComparison.InvariantCultureIgnoreCase)
            );

            // Make sure this property is supported
            if (prop.Equals(default(KeyValuePair<string, FieldInfo>)))
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
        protected Dictionary<string, FieldInfo> GetPropertyMap(Type objectType)
        {
            // Create our return Map
            Dictionary<string, FieldInfo> Properties = new Dictionary<string, FieldInfo>();
            FieldInfo[] Fields = objectType.GetFields(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly
            );

            // Loop through each property, and search for the custom attribute
            foreach (FieldInfo Field in Fields)
            {
                // If the Custom attribute exists, we add it to the Mapping
                Attribute attribute = Attribute.GetCustomAttribute(Field, typeof(PropertyName));
                if (attribute != null)
                {
                    PropertyName fieldAttr = attribute as PropertyName;

                    // Check for Duplicates
                    if (Properties.ContainsKey(fieldAttr.Name))
                    {
                        string Message = $"Duplicate PropertyName found \"{fieldAttr.Name}\" in {objectType.FullName}";
                        Logger.Error(Message, this.File, this.Tokens[0].Position);
                        throw new Exception(Message);
                    }

                    Properties.Add(fieldAttr.Name, Field);
                }
            }

            return Properties;
        }

        /// <summary>
        /// Throws a generic Exception with an Invalid property name message
        /// </summary>
        /// <param name="FieldName"></param>
        protected void ThrowUnsuportedField(string FieldName)
        {
            throw new Exception("Invalid Object Property \"" + FieldName + "\".");
        }

        /// <summary>
        /// This method is used to ensure that 2 Types match
        /// </summary>
        /// <param name="sourceType"></param>
        /// <param name="destType"></param>
        protected void EnsureTypeMatch(Type sourceType, Type destType)
        {
            // Make sure the types match!
            if (sourceType.GenericTypeArguments[0] != destType) // && !sourceType.IsAssignableFrom(destType))
                throw new Exception("Cannot convert " + destType.Name + " to " + sourceType.Name);
        }
    }
}
