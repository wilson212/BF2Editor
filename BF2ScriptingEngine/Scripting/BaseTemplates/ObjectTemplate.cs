namespace BF2ScriptingEngine.Scripting
{
    /// <summary>
    /// Used to define many objects within the game and link in other data.
    /// </summary>
    public abstract class ObjectTemplate : ConFileObject
    {
        /// <summary>
        /// This property is valid for only the debug version of the game engine.
        /// </summary>
        [PropertyName("saveInSeparateFile", 1)]
        public ObjectProperty<bool> SaveInSeparateFile;

        /// <summary>
        /// Gets the creator of this object
        /// </summary>
        [PropertyName("creator", 2)]
        public ObjectProperty<string> CreatedBy;

        /// <summary>
        /// Gets the last user to modify this object
        /// </summary>
        [PropertyName("modifiedByUser", 3)]
        public ObjectProperty<string> ModifiedBy;

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
        public ObjectProperty<bool> NotInGrid;

        /// <summary>
        /// 
        /// </summary>
        [PropertyName("createdInEditor", 21)]
        public ObjectProperty<bool> CreatedInEditor;

        /// <summary>
        /// Creates a new instance of an ObjectTemplate
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Token"></param>
        public ObjectTemplate(string Name, Token Token) : base(Name, "ObjectTemplate", Token) { }

        public static ConFileObject Create(string[] Params, Token Token)
        {
            switch (Params[1].ToLowerInvariant())
            {
                case "kit":
                    // Create a kit object
                    //break;
                case "itemcontainer":
                    // Create ItemContainer
                    //break;
                case "playercontrolobject":
                    // VehicleObject
                    //break;
                case "genericfirearm":
                // WeaponObject
                //break;
                default:
                    throw new System.NotSupportedException("Invalid Object Type \"" + Params[1] + "\".");
            }
        }
    }
}
