using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BF2ScriptingEngine.Scripting.Attributes;
using BF2ScriptingEngine.Scripting.Enumerations;

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
        [PropertyName("saveInSeparateFile")]
        public virtual ObjectProperty<bool> SaveInSeparateFile { get; internal set; }

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
        [PropertyName("createNotInGrid")]
        public virtual ObjectProperty<bool> NotInGrid { get; internal set; }

        /// <summary>
        /// Should be true if object or part does not have a visible mesh.
        /// </summary>
        [PropertyName("createdInEditor")]
        public virtual ObjectProperty<bool> CreatedInEditor { get; internal set; }

        [PropertyName("castsDynamicShadow")]
        public virtual ObjectProperty<bool> CastsDynamicShadow { get; internal set; }

        /// <summary>
        /// Do not change, Unknown Function!
        /// </summary>
        [PropertyName("preCacheObject")]
        public virtual ObjectProperty<bool> PreCacheObject { get; internal set; }

        #endregion Flags

        #region Default

        /// <summary>
        /// Last person to save the tweaks file.
        /// </summary>
        [PropertyName("creator")]
        public virtual ObjectProperty<string> CreatedBy { get; internal set; }

        /// <summary>
        /// Gets the last user to modify this object
        /// </summary>
        [PropertyName("modifiedByUser")]
        public virtual ObjectProperty<string> ModifiedBy { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Storing this as a string name for now... since AiTemplate and KitTemplate
        /// are trying to be stored here...
        /// </remarks>
        [PropertyName("aiTemplate")] //, ExistingObject]
        public virtual ObjectProperty<string> AiTemplate { get; internal set; }

        /// <summary>
        /// Gets or Sets the named geometry object
        /// </summary>
        [PropertyName("geometry")]
        public virtual ObjectProperty<GeometryTemplate> Geometry { get; internal set; }

        [PropertyName("geometryPart")]
        public ObjectProperty<int> GeometryPart { get; internal set; }

        /// <summary>
        /// Gets or Sets the name of the network id for this object.
        /// </summary>
        [PropertyName("networkableInfo", "setNetworkableInfo")]
        public virtual ObjectProperty<NetworkableInfo> NetworkableInfo { get; internal set; }

        [PropertyName("collisionMesh", "setCollisionMesh")]
        public virtual ObjectProperty<string> CollisionMesh { get; internal set; }

        [PropertyName("collisionPart")]
        public virtual ObjectProperty<int> CollisionPart { get; internal set; }

        /// <summary>
        /// Do not change, Unknown Function!
        /// </summary>
        [PropertyName("anchor")]
        public virtual ObjectProperty<string> Anchor { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// First Param is the meterial index of the mesh
        /// Second Param is The material name
        /// 3rd Param is material number
        /// </remarks>
        [PropertyName("mapMaterial")]
        public virtual ObjectPropertyList<int, string, int> Materials { get; internal set; }

        #endregion Default

        #region Physics

        /// <summary>
        /// Gets or Sets that this object can collide with other objects
        /// </summary>
        /// <remarks>
        /// When set, the geometry mesh associated with this object must have at least one collision mesh
        /// </remarks>
        [PropertyName("hasCollisionPhysics", "setHasCollisionPhysics")]
        public virtual ObjectProperty<bool> HasCollisionPhysics { get; internal set; }

        /// <summary>
        /// Gets or Sets that this object can collide with other objects
        /// </summary>
        /// <remarks>
        /// When set, the geometry mesh associated with this object must have at least one collision mesh
        /// </remarks>
        [PropertyName("physicsType")]
        public virtual ObjectProperty<PhysicsType> PhysicsType { get; internal set; }

        /// <summary>
        /// Gets or Sets whether this object is mobile
        /// </summary>
        /// <remarks>
        /// If the object is mobile, this should be set to 1 so that the physics engine knows. 
        /// This property sets the physics engine to calculate the object in the world as a mobile object. 
        /// </remarks>
        [PropertyName("hasMobilePhysics")]
        public virtual ObjectProperty<bool> HasMobilePhysics { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        [PropertyName("floaterMod")]
        public virtual ObjectProperty<decimal> FloaterMod { get; internal set; }

        #endregion Physics

        /// <summary>
        /// Gets or Sets the minimum rotation angles in degrees for an object, 
        /// from its initial position.
        /// </summary>
        /// <remarks>
        /// This and <see cref="SetMaxRotation"/> define the range of motion of any object, 
        /// be it a gun emplacement, a view out of a vehicle, etc. 
        /// 
        /// The first value is the yaw rotation, i.e. how much you can turn your head left or right. 
        /// The second value is the pitch rotation, i.e. how much you can nod up and down. 
        /// The last number is the backwards movement of the object such as an Engine, or the 
        /// minimum roll rotation (e.g. for a steering wheel).
        /// 
        /// When used in an Engine, SetMinRotation and SetMaxRotation for the Engine set how fine the 
        /// control scale is for the engine (basically how many distinct speeds the engine can be at). 
        /// For example, a SetMinRotation of -100 and SetMaxRotation of 200 means that the total speed 
        /// is split into 100 different speeds for reversing and and 200 different speeds for going 
        /// forward (so feasibly you could control the engine in such a way as to be able to go at 
        /// 200 different speeds but none between them).
        /// 
        /// If you set the engine to SetAutomaticReset 0 and then set SetMinRotation to -1 and SetMaxRotation 
        /// to 1 then basically if you press forward once, your car will speed up till maximum speed is reached, 
        /// pressing back once will basically make your car just set throttle to 0 so you will come to a stop 
        /// eventually, pressing back again will reverse. Basically having -1 and 1 makes it so that the engine 
        /// has 3 different positions that it can be in (Forward, idle, reverse). Although the engine will take a 
        /// while to get up to max speed, set by the differential and torque, your input to the engine is going 
        /// to be either full reverse, idle or full forward.
        /// </remarks>
        /// <seealso cref="http://bfmods.com/mdt/scripting/ObjectTemplate/Properties/SetMinRotation.html"/>
        [PropertyName("setMinRotation")]
        public virtual ObjectProperty<Point3D> SetMinRotation { get; internal set; }

        /// <summary>
        /// Gets or Sets the maximum rotation angles in degrees for an object, 
        /// from its initial position.
        /// </summary>
        /// <remarks>
        /// See SetMinRotation for a full explanation.
        /// </remarks>
        [PropertyName("setMaxRotation")]
        public virtual ObjectProperty<Point3D> SetMaxRotation { get; internal set; }

        /// <summary>
        /// Gets or Sets the maximum speed an object rotates in a plane of rotation in 
        /// response to the mouse. When used in an Engine, this setting controls how 
        /// an engine responds to player input,
        /// </summary>
        [PropertyName("setMaxSpeed")]
        public virtual ObjectProperty<Point3D> SetMaxSpeed { get; internal set; }

        /// <summary>
        /// Gets or Sets the amount of acceleration per second an object can gain 
        /// or lose when rotating in a plane of rotation. When used in an Engine object, 
        /// acceleration sets how quickly the engine accelerates when changing between 
        /// speed settings
        /// </summary>
        /// <remarks>
        /// A lower value means it is slower to get the object moving in response to the mouse. 
        /// For example, the defgun has 0/75/0 and tank turrets are 0/1000/0, meaning that you 
        /// can whip the tank turrets around but it's slow going to get the defgun to start 
        /// turning and to reverse its direction. SetMaxSpeed sets the maximum speed an object 
        /// can turn in response to a mouse move.
        /// 
        /// When used in an Engine object, low acceleration will make turning sluggish but will 
        /// increase in response the longer you hold the throttle key. Having it set very high 
        /// should make the car start to alter speed settings more rapidly (which results in 
        /// faster response). This controls how quickly the SetMaxSpeed part can get up to its 
        /// set speeds, which in turn sets how quickly the engine changes between the individual 
        /// speed segments
        /// </remarks>
        /// <seealso cref="http://bfmods.com/mdt/scripting/ObjectTemplate/Properties/SetAcceleration.html"/>
        [PropertyName("setAcceleration")]
        public virtual ObjectProperty<Point3D> SetAcceleration { get; internal set; }

        [PropertyName("setDeAcceleration")]
        public virtual ObjectProperty<Point3D> SetDeAcceleration { get; internal set; }

        [PropertyName("setUseDeAcceleration")]
        public virtual ObjectProperty<bool> UseDeAcceleration { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        [PropertyName("setInputToYaw")]
        public virtual ObjectProperty<PlayerInput> SetInputToYaw { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        [PropertyName("setInputToPitch")]
        public virtual ObjectProperty<PlayerInput> SetInputToPitch { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Limits the response speed of yaw functions?
        /// </remarks>
        [PropertyName("regulatePitch")]
        public virtual ObjectProperty<string> RegulatePitch { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        [PropertyName("setInputToRoll")]
        public virtual ObjectProperty<PlayerInput> SetInputToRoll { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        [PropertyName("regulateYawInput")]
        public virtual ObjectProperty<PlayerInput> RegulateYawInput { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Limits the response speed of yaw functions?
        /// </remarks>
        [PropertyName("regulateYaw")]
        public virtual ObjectProperty<string> RegulateYaw { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        [PropertyName("regulateVerticalPosInput")]
        public virtual ObjectProperty<PlayerInput> RegulateVerticalPosInput { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Limits the response speed of vertical functions?
        /// </remarks>
        [PropertyName("regulateVerticalPos")]
        public virtual ObjectProperty<string> RegulateVerticalPos { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        [PropertyName("maxVertRegAngle")]
        public virtual ObjectProperty<double> MaxVertRegAngle { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        [PropertyName("noVertRegAngle")]
        public virtual ObjectProperty<double> NoVertRegAngle { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        [PropertyName("unlockIndex")]
        public virtual ObjectProperty<int> UnlockIndex { get; internal set; }

        /// <summary>
        /// This property determines at what distance an object is entirely culled out from display.
        /// </summary>
        [PropertyName("cullRadiusScale")]
        public virtual ObjectProperty<double> CullRadiusScale { get; internal set; }

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

        [PropertyName("addTemplate")]
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

        /// <summary>
        /// Contains a Mapping of component types, that inherit from <see cref="IComponent"/>
        /// </summary>
        public static Dictionary<string, Type> ComponentTypes { get; set; }

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

            // Create object mappings of type ObjectTemplate
            var Comparer = StringComparer.InvariantCultureIgnoreCase;
            Type baseType = typeof(ObjectTemplate);
            Type[] typelist = TypeCache.GetTypesInNamespace("BF2ScriptingEngine.Scripting")
                .Where(x => baseType.IsAssignableFrom(x)).ToArray();
            ObjectTypes = typelist.ToDictionary(x => x.Name, v => v, Comparer);

            // Create mappings of all Components
            typelist = TypeCache.GetTypesInNamespace("BF2ScriptingEngine.Scripting.Components")
                .Where(x => !x.IsAbstract && x.GetInterface("IComponent") != null)
                .ToArray();
            ComponentTypes = typelist.ToDictionary(x => x.Name, v => v, Comparer);
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

            // Map components that are used in this template
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

                // [name] => array of component types that are used in this template
                ComponentMap[type.Name] = properties;
            }
        }

        #region ObjectMethod Actions

        /// <summary>
        /// Creates a component (Sub Object) for an ObjectTemplate
        /// </summary>
        /// <param name="token"></param>
        /// <param name="comment"></param>
        protected virtual ConFileEntry Method_CreateComponent(Token token, string name)
        {
            Type type = this.GetType();

            // Token correction
            token.Kind = TokenType.Component;

            // Ensure we have a map of components => property
            if (!ComponentTypes.ContainsKey(name))
            {
                throw new Exception($"Unregistered component type \"{name}\"");
            }
            else if (!ComponentMap.ContainsKey(type.Name))
            {
                throw new Exception($"Object type \"{type.Name}\" does not support Components");
            }
            else if (!ComponentMap[type.Name].ContainsKey(name))
            {
                throw new Exception($"Unsupported Component type \"{name}\" in \"{type.Name}\"");
            }

            // Get our field 
            PropertyInfo property = ComponentMap[type.Name][name];
            //Type componentType = property.PropertyType.GenericTypeArguments[0];
            Type componentType = ComponentTypes[name];
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
        protected virtual ConFileEntry Method_AddTemplate(Token token, string name)
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
        protected virtual ConFileEntry Method_SetPosition(Token token, string arg1 = "0/0/0")
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
                item.SetPosition = new ObjectProperty<Point3D>("setPosition", token, field, this);
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
        protected virtual ConFileEntry Method_SetRotation(Token token, string arg1 = "0/0/0")
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
                item.SetRotation = new ObjectProperty<Point3D>("setRotation", token, field, this);
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
