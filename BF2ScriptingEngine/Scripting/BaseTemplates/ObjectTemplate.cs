using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BF2ScriptingEngine.Scripting.Attributes;
using BF2ScriptingEngine.Scripting.GeometryTemplates;

namespace BF2ScriptingEngine.Scripting
{
    /// <summary>
    /// Used to define many objects within the game and link in other data.
    /// </summary>
    public abstract class ObjectTemplate : ConFileObject
    {
        #region Object Properties

        /// <summary>
        /// This property is valid for only the debug version of the game engine.
        /// </summary>
        [PropertyName("saveInSeparateFile", 1)]
        public ObjectProperty<bool> SaveInSeparateFile { get; set; }

        /// <summary>
        /// Gets the creator of this object
        /// </summary>
        [PropertyName("creator", 2)]
        public ObjectProperty<string> CreatedBy { get; set; }

        /// <summary>
        /// Gets the last user to modify this object
        /// </summary>
        [PropertyName("modifiedByUser", 3)]
        public ObjectProperty<string> ModifiedBy { get; set; }

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
        public ObjectProperty<bool> NotInGrid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [PropertyName("createdInEditor", 21)]
        public ObjectProperty<bool> CreatedInEditor { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Storing this as a string name for now... since AiTemplate and KitTemplate
        /// are trying to be stored here...
        /// </remarks>
        [PropertyName("aiTemplate", 30)] //, ExistingObject]
        public ObjectProperty<string> AiTemplate { get; set; }

        /// <summary>
        /// Gets or Sets the named geometry object
        /// </summary>
        [PropertyName("geometry", 40), ExistingObject]
        public virtual ObjectProperty<GeometryTemplate> Geometry { get; set; }

        /// <summary>
        /// Gets or Sets the name of the network id for this object.
        /// </summary>
        [PropertyName("networkableInfo", 50)]
        public ObjectProperty<string> NetworkableInfo { get; set; }

        /// <summary>
        /// Gets or Sets that this object can collide with other objects
        /// </summary>
        /// <remarks>
        /// When set, the geometry mesh associated with this object must have at least one collision mesh
        /// </remarks>
        [PropertyName("setHasCollisionPhysics", 60)]
        public ObjectProperty<bool> HasCollisionPhysics { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [PropertyName("unlockIndex", 100)]
        public ObjectProperty<int> UnlockIndex { get; set; }

        /// <summary>
        /// This property determines at what distance an object is entirely culled out from display.
        /// </summary>
        [PropertyName("cullRadiusScale", 110)]
        public ObjectProperty<double> CullRadiusScale { get; set; }

        /// <summary>
        /// Contains a list of child objects attached to this object
        /// </summary>
        /// <remarks>
        /// Any SetPosition and SetRotation commands following this will 
        /// apply to the attached object, until another AddTemplate command 
        /// is encountered.
        /// </remarks>
        [PropertyName("addTemplate", 400)]
        [Comment(
            Before = "-------------------------------------",
            After =  "-------------------------------------"
        )]
        public virtual ObjectProperty<List<ChildTemplate>> Templates { get; set; }

        #endregion

        #region Object Methods

        [PropertyName("setPosition")]
        protected ObjectMethod<string> SetPosition { get; }

        [PropertyName("setRotation")]
        protected ObjectMethod<string> SetRotation { get; }

        [PropertyName("createComponent")]
        protected ObjectMethod<string> CreateComponent { get; }

        #endregion

        /// <summary>
        /// Contains a map of properties in the deriving classes that are
        /// component objects
        /// </summary>
        protected static Dictionary<string, Dictionary<string, PropertyInfo>> ComponentMap;

        static ObjectTemplate()
        {
            ComponentMap = new Dictionary<string, Dictionary<string, PropertyInfo>>();
        }

        /// <summary>
        /// Creates a new instance of an ObjectTemplate
        /// </summary>
        /// <param name="Name">The name of this object</param>
        /// <param name="Token">The ConFile token</param>
        public ObjectTemplate(string Name, Token Token) : base(Name, "ObjectTemplate", Token)
        {
            // === Create method instances
            CreateComponent = new ObjectMethod<string>(Action_CreateComponent);
            SetPosition = new ObjectMethod<string>(Action_SetPosition);
            SetRotation = new ObjectMethod<string>(Action_SetRotation);
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
        public virtual void Action_CreateComponent(Token token, string name)
        {
            Type type = this.GetType();

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
            object component = Activator.CreateInstance(componentType, args);
            var objProperty = ObjectProperty.Create(property, token);

            // Use reflection to set the value of the new component instance
            objProperty.GetType().GetMethod("SetValues").Invoke(
                objProperty, 
                new object[] {
                    new object[] { component }, // First param is an object array
                    token // Second Param is a Token
                }
            );

            // Set value of this.{name}
            property.SetValue(this, objProperty);
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
        private void Action_SetPosition(Token token, string arg1 = "0/0/0")
        {
            // Ensure that we have a child template to set the position on
            ChildTemplate item = Templates?.Value?.Last();
            if (item == null)
            {
                string err = $"SetPosition called on a non-instantiated child object";
                Logger.Error(err, token.File, token.Position);
                throw new Exception(err);
            }

            // Get the "SetPosition" field, and make sure the ObjectProperty is not null
            PropertyInfo field = item.GetProperty("SetPosition").Value;
            if (field.GetValue(item) == null)
                item.SetPosition = new ObjectProperty<string>("setPosition", token, field);

            // Set the new value
            item.SetPosition.SetValue(token);
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
        private void Action_SetRotation(Token token, string arg1 = "0/0/0")
        {
            // Ensure that we have a child template
            ChildTemplate item = Templates?.Value?.Last();
            if (item == null)
            {
                string err = $"SetRotation called on a non-instantiated child object";
                Logger.Error(err, token.File, token.Position);
                throw new Exception(err);
            }

            // Get the "SetPosition" field, and make sure the ObjectProperty is not null
            PropertyInfo field = item.GetProperty("SetRotation").Value;
            if (field.GetValue(item) == null)
                item.SetRotation = new ObjectProperty<string>("setRotation", token, field);

            // Set the new value
            item.SetRotation.SetValue(token);
        }

        #endregion

        /// <summary>
        /// Creates a new instance of ObjectTemplate with the following attributes
        /// </summary>
        /// <param name="tokenArgs">The command line token</param>
        /// <param name="Token">The ConFile token</param>
        public static ConFileObject Create(TokenArgs tokenArgs, Token Token)
        {
            string type = tokenArgs.Arguments[0];
            string name = tokenArgs.Arguments[1];

            switch (type.ToLowerInvariant())
            {
                case "simpleobject": return new SimpleObject(name, Token);
                case "kit": return new Kit(name, Token);
                case "itemcontainer": return new ItemContainer(name, Token);
                case "playercontrolobject":
                case "genericfirearm":
                default:
                    throw new NotSupportedException("Invalid Object Type \"" + type + "\".");
            }
        }
    }
}
