using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BF2ScriptingEngine.Scripting;

namespace BF2ScriptingEngine
{
    /// <summary>
    /// The object manager class is used to keep a list of all loaded
    /// ConFile templates, and provide methods to fetch them globally
    /// </summary>
    public static class ObjectManager
    {
        /// <summary>
        /// Contains a list of all objects registered in the global namespace
        /// </summary>
        private static Dictionary<Tuple<string, ObjectType>, ConFileObject> Globals;

        /// <summary>
        /// Indicates the number of objects loaded into the Global Namespace
        /// </summary>
        public static int ObjectsCount
        {
            get { return Globals.Count; }
        }

        /// <summary>
        /// Gets a list of Assignable types for the GetObjectType method
        /// </summary>
        /// <remarks>
        /// This list just contains what we can parse so far...
        /// </remarks>
        private static Dictionary<ObjectType, Type> AssignableTypes = new Dictionary<ObjectType, Type>()
        {
            { ObjectType.ObjectTemplate, typeof(ObjectTemplate) },
            { ObjectType.WeaponTemplate, typeof(WeaponTemplate) },
            { ObjectType.AiTemplate, typeof(AiTemplate) },
            { ObjectType.AiTemplatePlugin, typeof(AiTemplatePlugin) },
            { ObjectType.KitTemplate, typeof(KitTemplate) },
            { ObjectType.GeometryTemplate, typeof(GeometryTemplate) },
        };

        /// <summary>
        /// Static constructor
        /// </summary>
        static ObjectManager()
        {
            var Comparer = new ObjectEqualityComparer();
            Globals = new Dictionary<Tuple<string, ObjectType>, ConFileObject>(Comparer);
        }

        /// <summary>
        /// Clears all objects loaded from memory
        /// </summary>
        public static void ReleaseAll()
        {
            Globals.Clear();
        }

        /// <summary>
        /// Clears all the loaded objects from the specified ConFile
        /// </summary>
        /// <param name="workingFile"></param>
        public static void ReleaseAll(ConFile workingFile)
        {
            foreach (ConFileObject obj in workingFile.Objects)
            {
                var key = new Tuple<string, ObjectType>(obj.Name, GetObjectType(obj));
                Globals.Remove(key);
            }
        }

        /// <summary>
        /// This method analysis a <see cref="ConFileObject"/> and returns
        /// the <see cref="ObjectType"/> representation of the object.
        /// </summary>
        /// <param name="obj"></param>
        public static ObjectType GetObjectType(ConFileObject obj)
        {
            return GetObjectType(obj.ReferenceName, obj.File, obj.Tokens[0]?.Position ?? 0);
        }

        /// <summary>
        /// This method analysis a <see cref="ConFileObject.ReferenceName"/> and returns
        /// the <see cref="ObjectType"/> representation of the object.
        /// </summary>
        /// <param name="referenceName">The reference string used to call upon this type of object</param>
        public static ObjectType GetObjectType(string referenceName)
        {
            ObjectType type;
            if (!Enum.TryParse<ObjectType>(referenceName, true, out type))
            {
                string error = $"No ObjectType definition for \"{referenceName}\"";
                throw new Exception(error);
            }

            return type;
        }

        /// <summary>
        /// This method analysis a <see cref="ConFileObject.ReferenceName"/> and returns
        /// the <see cref="ObjectType"/> representation of the object.
        /// </summary>
        /// <param name="referenceName">The reference string used to call upon this type of object</param>
        /// <param name="file">Used in the <see cref="ScriptEngine"/></param>
        /// <param name="line">Used in the <see cref="ScriptEngine"/></param>
        public static ObjectType GetObjectType(string referenceName, ConFile file, int line)
        {
            ObjectType type;
            if (!Enum.TryParse<ObjectType>(referenceName, true, out type))
            {
                string error = $"No ObjectType definition for \"{referenceName}\"";
                Logger.Error(error, file, line);
                throw new Exception(error);
            }

            return type;
        }

        /// <summary>
        /// This method returns the <see cref="ObjectType"/> representation
        /// of the supplied confile object type
        /// </summary>
        /// <param name="objType">The derived <see cref="ConFileObject"/> type.</param>
        /// <returns></returns>
        public static ObjectType GetObjectType(Type objType)
        {
            foreach (KeyValuePair<ObjectType, Type> item in AssignableTypes)
            {
                if (item.Value.IsAssignableFrom(objType))
                    return item.Key;
            }

            throw new Exception("Invalid object type: " + objType.Name);
        }

        /// <summary>
        /// USED BY PARSER - Registers a new object into the namespace
        /// </summary>
        /// <param name="objectName"></param>
        /// <param name="file"></param>
        public static void RegisterObject(ConFileObject obj)
        {
            var key = new Tuple<string, ObjectType>(obj.Name, GetObjectType(obj));
            Globals.Add(key, obj);
        }

        /// <summary>
        /// Returns whether the specified object name is registered
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool ContainsObject(string name, ObjectType type)
        {
            var key = new Tuple<string, ObjectType>(name, type);
            return Globals.ContainsKey(key);
        }

        /// <summary>
        /// Fetches the loaded object by th specified name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ConFileObject GetObject(string name, ObjectType type)
        {
            var key = new Tuple<string, ObjectType>(name, type);
            return Globals[key];
        }

        /// <summary>
        /// Gets the object of the specifed type and returns it. If the object
        /// does not exist, the default value of <typeparamref name="T"/> is 
        /// returned instead
        /// </summary>
        /// <typeparam name="T">The type of object to fetch; Must derive from ConFileObject</typeparam>
        /// <param name="name">The name of the object (case sensitive)</param>
        /// <returns></returns>
        public static T GetObject<T>(string name) where T : ConFileObject
        {
            ObjectType type = GetObjectType(typeof(T));
            var key = new Tuple<string, ObjectType>(name, type);

            if (Globals.ContainsKey(key))
                return (T)(object)Globals[key];

            return default(T);
        }

        /// <summary>
        /// Returns an array of all the loaded objects
        /// </summary>
        public static ConFileObject[] GetObjects()
        {
            return Globals.Values.ToArray();
        }

        /// <summary>
        /// Returns an array of all the loaded objects that meet the specified criteria
        /// </summary>
        /// <param name="Where">A criteria method that returns a bool, determining whether the object
        /// will be returned or not.</param>
        public static ConFileObject[] GetObjects(Func<ConFileObject, bool> Where)
        {
            List<ConFileObject> Objs = new List<ConFileObject>();

            foreach (ConFileObject obj in Globals.Values)
            {
                // Check if the method meets the criteria
                if (Where.Invoke(obj))
                    Objs.Add(obj);
            }

            return Objs.ToArray();
        }
    }
}
