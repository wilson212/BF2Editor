using System;
using System.Collections.Generic;
using BF2ScriptingEngine.Scripting;

namespace BF2ScriptingEngine
{
    /// <summary>
    /// Contains a registry of all object <see cref="ReferenceType"/>'s
    /// </summary>
    public static class ReferenceManager
    {
        private static Dictionary<string, ReferenceType> Registry;

        /// <summary>
        /// Register the supported by default ReferenceTypes
        /// </summary>
        static ReferenceManager()
        {
            Registry = new Dictionary<string, ReferenceType>();
            ReferenceType current;

            // Create Object Template
            current = new ReferenceType("ObjectTemplate", typeof(ObjectTemplate));
            current.Mappings.Add("create", ObjectTemplate.Create);
            current.Mappings.Add("activeSafe", ObjectTemplate.Create);
            AddType(current);

            // Create Weapon Template
            current = new ReferenceType("weaponTemplate", typeof(WeaponTemplate));
            current.Mappings.Add("create", WeaponTemplate.Create);
            current.Mappings.Add("activeSafe", WeaponTemplate.Create);
            AddType(current);

            // Create Ai Template
            current = new ReferenceType("aiTemplate", typeof(AiTemplate));
            current.Mappings.Add("create", AiTemplate.Create);
            AddType(current);

            // Create Ai Template Plugin
            current = new ReferenceType("aiTemplatePlugIn", typeof(AiTemplatePlugin));
            current.Mappings.Add("create", AiTemplatePlugin.Create);
            AddType(current);

            // Create Kit Template
            current = new ReferenceType("kitTemplate", typeof(KitTemplate));
            current.Mappings.Add("create", KitTemplate.Create);
            AddType(current);

            // Create Kit Template
            current = new ReferenceType("GeometryTemplate", typeof(GeometryTemplate));
            current.Mappings.Add("create", GeometryTemplate.Create);
            current.Mappings.Add("activeSafe", GeometryTemplate.Create);
            AddType(current);

            // Create Kit Template
            current = new ReferenceType("CollisionManager", typeof(CollisionManager));
            current.Mappings.Add("createTemplate", CollisionManager.Create);
            AddType(current);
        }

        /// <summary>
        /// Adds a new <see cref="ReferenceType"/> to the registry
        /// </summary>
        /// <param name="type">The Reference to add</param>
        /// <exception cref="ArgumentException">
        /// Thrown when a ReferenceType with the same name already exists
        /// </exception>
        public static void AddType(ReferenceType type)
        {
            Registry.Add(type.Name, type);
        }

        /// <summary>
        /// Removes the <see cref="ReferenceType"/> with the supplied name
        /// </summary>
        public static bool RemoveType(string referenceName)
        {
            return Registry.Remove(referenceName);
        }

        /// <summary>
        /// Removes the <see cref="ReferenceType"/> from the Registry
        /// </summary>
        public static bool RemoveType(ReferenceType type)
        {
            return Registry.Remove(type.Name);
        }

        /// <summary>
        /// Returns the <see cref="ReferenceType"/> object by name
        /// </summary>
        /// <param name="referenceName"></param>
        /// <returns>
        /// Returns the <see cref="ReferenceType"/> object if it exists, otherwise null
        /// </returns>
        public static ReferenceType GetReferenceType(string referenceName)
        {
            if (!Registry.ContainsKey(referenceName))
                return null;

            return Registry[referenceName];
        }

        /// <summary>
        /// This method returns the <see cref="ReferenceType"/> representation
        /// of the supplied confile object type
        /// </summary>
        /// <param name="objType">The derived <see cref="ConFileObject"/> type.</param>
        /// <returns></returns>
        public static ReferenceType GetReferenceType(Type objType)
        {
            foreach (KeyValuePair<string, ReferenceType> item in Registry)
            {
                Type check = item.Value.Type;
                if (check == objType || check.IsAssignableFrom(objType))
                    return item.Value;
            }

            throw new Exception("Invalid reference type: " + objType.Name);
        }
    }
}
