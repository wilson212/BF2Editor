using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BF2ScriptingEngine.Scripting.Attributes;

namespace BF2ScriptingEngine.Scripting
{
    /// <summary>
    /// Used to define many objects within the game and link in other data.
    /// </summary>
    public abstract class ObjectTemplate : ConFileObject
    {
        #region Object Properties

        #region Flags

        /// <summary>
        /// This property is valid for only the debug version of the game engine.
        /// </summary>
        /// <remarks>
        /// The root object in the hierarchy should have this as true
        /// </remarks>
        [PropertyName("saveInSeparateFile", 1)]
        public ObjectProperty<bool> SaveInSeparateFile { get; internal set; }

        /// <summary>
        /// This command tells the game that the object is not limited to any space constraints, 
        /// meaning it can go outside the boundaries of the levels, high and low.
        /// </summary>
        /// <remarks>
        /// It is always set (to 1) for major projectiles (flak, tank shells, ship's shells), 
        /// but not for bullets. It is also used in Terrain.con for every battle as a property 
        /// of the terrain geometry itself, so that the terrain can legally "wrap" outside the 
        /// bounds of the level.
        /// </remarks>
        /// <seealso cref="GenericProjectile"/>
        /// <seealso cref="http://bfmods.com/mdt/scripting/ObjectTemplate/Properties/CreateNotInGrid.html"/>
        [PropertyName("createNotInGrid", 20)]
        public ObjectProperty<bool> NotInGrid { get; internal set; }

        /// <summary>
        /// Should be true if object or part does not have a visible mesh.
        /// </summary>
        [PropertyName("createdInEditor", 21)]
        public ObjectProperty<bool> CreatedInEditor { get; internal set; }

        /// <summary>
        /// Do not change, Unknown Function!
        /// </summary>
        [PropertyName("PreCacheObject", 22)]
        public ObjectProperty<bool> PreCacheObject { get; internal set; }

        #endregion Flags

        #region Default

        /// <summary>
        /// Last person to save the tweaks file.
        /// </summary>
        [PropertyName("creator", 2)]
        public ObjectProperty<string> CreatedBy { get; internal set; }

        /// <summary>
        /// Gets the last user to modify this object
        /// </summary>
        [PropertyName("modifiedByUser", 3)]
        public ObjectProperty<string> ModifiedBy { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Storing this as a string name for now... since AiTemplate and KitTemplate
        /// are trying to be stored here...
        /// </remarks>
        [PropertyName("aiTemplate", 30)] //, ExistingObject]
        public ObjectProperty<string> AiTemplate { get; internal set; }

        /// <summary>
        /// Gets or Sets the named geometry object
        /// </summary>
        [PropertyName("geometry", 40)]
        public virtual ObjectProperty<GeometryTemplate> Geometry { get; internal set; }

        [PropertyName("geometryPart")]
        public ObjectProperty<int> GeometryPart { get; internal set; }

        /// <summary>
        /// Gets or Sets the name of the network id for this object.
        /// </summary>
        [PropertyName("networkableInfo", 50)]
        public ObjectProperty<NetworkableInfo> NetworkableInfo { get; internal set; }

        [PropertyName("collisionMesh")]
        public ObjectProperty<string> CollisionMesh { get; internal set; }

        [PropertyName("collisionPart")]
        public ObjectProperty<int> CollisionPart { get; internal set; }

        /// <summary>
        /// Do not change, Unknown Function!
        /// </summary>
        [PropertyName("anchor")]
        public ObjectProperty<string> Anchor { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// First Param is the meterial index of the mesh
        /// Second Param is The material name
        /// 3rd Param is material number
        /// </remarks>
        [PropertyName("mapMaterial")]
        public ObjectPropertyList<int, string, int> Materials { get; internal set; }

        #endregion Default

        #region Physics

        /// <summary>
        /// Gets or Sets that this object can collide with other objects
        /// </summary>
        /// <remarks>
        /// When set, the geometry mesh associated with this object must have at least one collision mesh
        /// </remarks>
        [PropertyName("hasCollisionPhysics", 60)]
        public ObjectProperty<bool> HasCollisionPhysics { get; internal set; }

        [PropertyName("setHasCollisionPhysics", 60)]
        internal ObjectProperty<bool> SetHasCollisionPhysics
        {
            get { return HasCollisionPhysics; }
            set { HasCollisionPhysics = value; }
        }

        /// <summary>
        /// Gets or Sets that this object can collide with other objects
        /// </summary>
        /// <remarks>
        /// When set, the geometry mesh associated with this object must have at least one collision mesh
        /// </remarks>
        [PropertyName("physicsType", 60)]
        public ObjectProperty<int> PhysicsType { get; internal set; }

        /// <summary>
        /// Gets or Sets whether this object is mobile
        /// </summary>
        /// <remarks>
        /// If the object is mobile, this should be set to 1 so that the physics engine knows. 
        /// This property sets the physics engine to calculate the object in the world as a mobile object. 
        /// </remarks>
        [PropertyName("hasMobilePhysics", 60)]
        public ObjectProperty<bool> HasMobilePhysics { get; internal set; }

        #endregion Physics

        /// <summary>
        /// 
        /// </summary>
        [PropertyName("unlockIndex", 100)]
        public ObjectProperty<int> UnlockIndex { get; internal set; }

        /// <summary>
        /// This property determines at what distance an object is entirely culled out from display.
        /// </summary>
        [PropertyName("cullRadiusScale", 110)]
        public ObjectProperty<double> CullRadiusScale { get; internal set; }

        /// <summary>
        /// Contains a list of child objects attached to this object
        /// </summary>
        /// <remarks>
        /// Any SetPosition and SetRotation commands following this will 
        /// apply to the attached object, until another AddTemplate command 
        /// is encountered.
        /// </remarks>
        [PropertyName("__addTemplate")]
        public virtual ObjectPropertyList<ChildTemplate> Templates { get; internal set; }

        #endregion

        #region Object Methods

        [PropertyName("addTemplate", 400)]
        protected ObjectMethod<string> AddTemplate { get; set; }

        [PropertyName("setPosition")]
        protected ObjectMethod<string> SetPosition { get; set; }

        [PropertyName("setRotation")]
        protected ObjectMethod<string> SetRotation { get; set; }

        [PropertyName("createComponent")]
        protected ObjectMethod<string> CreateComponent { get; set; }

        #endregion

        #region Mappings

        /// <summary>
        /// Contains a Mapping of object types, that derive from <see cref="ObjectTemplate"/>
        /// </summary>
        public static Dictionary<string, Type> ObjectTypes { get; set; }

        #endregion

        /// <summary>
        /// Contains a map of properties in the deriving classes that are
        /// component objects
        /// </summary>
        protected static Dictionary<string, Dictionary<string, PropertyInfo>> ComponentMap;

        static ObjectTemplate()
        {
            // Create our component map
            ComponentMap = new Dictionary<string, Dictionary<string, PropertyInfo>>();

            // Create object mappings
            var Comparer = StringComparer.InvariantCultureIgnoreCase;
            Type baseType = typeof(ObjectTemplate);
            Type[] typelist = TypeCache.GetTypesInNamespace("BF2ScriptingEngine.Scripting")
                .Where(x => baseType.IsAssignableFrom(x)).ToArray();
            ObjectTypes = typelist.ToDictionary(x => x.Name, v => v, Comparer);
        }

        /// <summary>
        /// Creates a new instance of an ObjectTemplate
        /// </summary>
        /// <param name="Name">The name of this object</param>
        /// <param name="Token">The ConFile token</param>
        public ObjectTemplate(string Name, Token Token) : base(Name, Token)
        {
            // === Create method instances
            CreateComponent = new ObjectMethod<string>(Method_CreateComponent);
            AddTemplate = new ObjectMethod<string>(Method_AddTemplate);
            SetPosition = new ObjectMethod<string>(Method_SetPosition);
            SetRotation = new ObjectMethod<string>(Method_SetRotation);
            // ===

            // Grab this derived type information
            Type type = this.GetType();

            // Ensure we have a map of components => property
            if (!ComponentMap.ContainsKey(type.Name))
            {
                Dictionary<string, PropertyInfo> properties = new Dictionary<string, PropertyInfo>();
                PropertyInfo[] fields = type.GetProperties(
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                );

                // Loop through each property, and search for the custom attribute
                foreach (PropertyInfo property in fields)
                {
                    // If the Custom attribute exists, we add it to the Mapping
                    Attribute attribute = Attribute.GetCustomAttribute(property, typeof(Component));
                    if (attribute != null)
                    {
                        Component fieldAttr = attribute as Component;
                        foreach (string cType in fieldAttr.Types)
                            properties[cType] = property;
                    }
                }

                ComponentMap[type.Name] = properties;
            }
        }

        #region ObjectMethod Actions

        /// <summary>
        /// Creates a component (Sub Object) for an ObjectTemplate
        /// </summary>
        /// <param name="token"></param>
        /// <param name="comment"></param>
        public virtual ConFileEntry Method_CreateComponent(Token token, string name)
        {
            Type type = this.GetType();

            // Token correction
            token.Kind = TokenType.Component;

            // Ensure we have a map of components => property
            if (!ComponentMap.ContainsKey(type.Name))
            {
                throw new Exception($"Object type \"{type.Name}\" does not support Components");
            }
            else if (!ComponentMap[type.Name].ContainsKey(name))
            {
                throw new Exception($"Unsupported Component type \"{name}\" in \"{type.Name}\"");
            }

            // Get our field 
            PropertyInfo property = ComponentMap[type.Name][name];
            Type componentType = property.PropertyType.GenericTypeArguments[0];
            var args = new object[] { name, token };

            // Create instances
            var component = (ConFileObject)Activator.CreateInstance(componentType, args);
            //var objProperty = ObjectProperty.Create(property, component, token);
            var objProperty = ObjectProperty.Create(property, this, token);

            // Use reflection to set the value of the new component instance
            objProperty.SetValues(new object[] { component }, token);

            // Set value of this.{name}
            property.SetValue(this, objProperty);

            // Add component to file entries by returning it
            return objProperty;
        }

        /// <summary>
        /// Action method for adding child templates to this object
        /// </summary>
        /// <param name="token"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual ConFileEntry Method_AddTemplate(Token token, string name)
        {
            // Get the internal property, and check if templates is null
            var info = GetProperty("__addTemplate").Value;
            if (Templates == null)
                Templates = new ObjectPropertyList<ChildTemplate>("addTemplate", token, info, this);

            // Create the object template value, and the object property
            ChildTemplate ct = new ChildTemplate(token.TokenArgs.Arguments.Last(), token);
            var prop = new ObjectProperty<ChildTemplate>("addTemplate", token, info, this);

            // We must also manually define the ValueInfo, because ChildTemplate
            // is NOT a RefernceType object
            prop.Argument = new ValueInfo<ChildTemplate>(ct);
            Templates.Items.Add(prop);

            return prop;
        }

        /// <summary>
        /// Action method for setting the position of a child object attached 
        /// to the current object with the AddTemplate command.
        /// </summary>
        /// <remarks>
        /// This position is relative to the center of the this object. 
        /// </remarks>
        /// <param name="token"></param>
        /// <param name="arg1"></param>
        private ConFileEntry Method_SetPosition(Token token, string arg1 = "0/0/0")
        {
            // Ensure that we have a child template to set the position on
            ChildTemplate item = Templates?.Items?.LastOrDefault()?.Value;
            if (item == null)
            {
                string err = $"SetPosition called on a non-instantiated child object";
                Logger.Error(err, token.File, token.Position);
                throw new Exception(err);
            }

            // Ensure the SetPosition is not null
            if (item.SetPosition == null)
            {
                PropertyInfo field = item.GetProperty("SetPosition").Value;
                item.SetPosition = new ObjectProperty<string>("setPosition", token, field, this);
                token.File.AddProperty(item.SetPosition);
            }

            // Set the new value
            item.SetPosition.SetValue(token);

            // Don't create an entry!
            return null;
        }

        /// <summary>
        /// Action method for setting the direction and angles of a child object attached 
        /// to the this object by the AddTemplate command. The angle is in terms of 
        /// yaw/pitch/roll. 
        /// </summary>
        /// <remarks>
        /// The units used for this parameter are degrees (1 - 360) but you can have 
        /// negative degrees as well. If you define multiple rotations, they happen 
        /// in this order.
        /// </remarks>
        /// <param name="token"></param>
        /// <param name="arg1"></param>
        private ConFileEntry Method_SetRotation(Token token, string arg1 = "0/0/0")
        {
            // Ensure that we have a child template
            ChildTemplate item = Templates?.Items?.LastOrDefault()?.Value;
            if (item == null)
            {
                string err = $"SetRotation called on a non-instantiated child object";
                Logger.Error(err, token.File, token.Position);
                throw new Exception(err);
            }

            // Get the "SetRotation" field, and make sure the ObjectProperty is not null
            if (item.SetRotation == null)
            {
                PropertyInfo field = item.GetProperty("SetRotation").Value;
                item.SetRotation = new ObjectProperty<string>("setRotation", token, field, this);
                token.File.AddProperty(item.SetRotation);
            }

            // Set the new value
            item.SetRotation.SetValue(token);

            // Don't create an entry!
            return null;
        }

        #endregion

        /// <summary>
        /// Creates a new instance of ObjectTemplate with the following attributes
        /// </summary>
        /// <param name="tokenArgs">The command line token</param>
        /// <param name="token">The ConFile token</param>
        public static ConFileObject Create(Token token)
        {
            // Make sure we have the correct number of arguments
            if (token.TokenArgs.Arguments.Length != 2)
            {
                throw new ArgumentException(String.Concat(
                    "Invalid arguments count for ObjectTemplate;",
                     $"Got {token.TokenArgs.Arguments.Length}, Expecting 2."
                ));
            }

            // Extract our arguments
            string type = token.TokenArgs.Arguments[0];
            string name = token.TokenArgs.Arguments[1];

            // Ensure this type is supported
            if (!ObjectTypes.ContainsKey(type))
                throw new ParseException("Invalid ObjectTemplate derived type \"" + type + "\".", token);

            // Create and return our object instance
            var t = ObjectTypes[type];
            return (ConFileObject)Activator.CreateInstance(t, name, token);
        }
    }
}
