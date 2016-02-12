using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BF2ScriptingEngine
{
    /// <summary>
    /// Provides a set of methods, allowing a Deep clone of any object type
    /// </summary>
    /// <remarks>
    /// Credits to Alex Burtsev @ StackOverflow
    /// 
    /// A Deep object copy extension method, based on recursive "MemberwiseClone", 
    /// it is fast (3 times faster then BinaryFormatter), it works with any object, 
    /// you don't need default constructor or serializable attributes.
    /// </remarks>
    /// <seealso cref="http://stackoverflow.com/questions/129389/how-do-you-do-a-deep-copy-an-object-in-net-c-specifically"/>
    /// <seealso cref="https://github.com/Burtsev-Alexey/net-object-deep-copy"/>
    public static class ObjectExtensions
    {
        private static readonly MethodInfo CloneMethod = typeof(Object).GetMethod(
            "MemberwiseClone",
            BindingFlags.NonPublic | BindingFlags.Instance);

        /// <summary>
        /// Performs a Deep clone of the specified object
        /// </summary>
        /// <param name="originalObject"></param>
        /// <returns></returns>
        public static Object Copy(this Object originalObject)
        {
            return InternalCopy(originalObject, new Dictionary<Object, Object>(
                new ReferenceEqualityComparer()));
        }

        /// <summary>
        /// Performs a Deep clone of the specified object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="original"></param>
        /// <returns></returns>
        public static T Copy<T>(this T original)
        {
            return (T)Copy((Object)original);
        }

        private static Object InternalCopy(Object originalObject, IDictionary<Object, Object> visited)
        {
            if (originalObject == null) return null;
            var typeToReflect = originalObject.GetType();
            if (IsPrimitive(typeToReflect)) return originalObject;
            if (visited.ContainsKey(originalObject)) return visited[originalObject];
            if (typeof(Delegate).IsAssignableFrom(typeToReflect)) return null;
            var cloneObject = CloneMethod.Invoke(originalObject, null);
            if (typeToReflect.IsArray)
            {
                var arrayType = typeToReflect.GetElementType();
                if (IsPrimitive(arrayType) == false)
                {
                    Array clonedArray = (Array)cloneObject;
                    clonedArray.ForEach((array, indices) =>
                    {
                        array.SetValue(
                            InternalCopy(clonedArray.GetValue(indices), visited),
                            indices
                        );
                    });
                }

            }
            visited.Add(originalObject, cloneObject);
            CopyFields(originalObject, visited, cloneObject, typeToReflect);
            RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect);
            return cloneObject;
        }

        public static bool IsPrimitive(this Type type)
        {
            if (type == typeof(String)) return true;
            return (type.IsValueType & type.IsPrimitive);
        }

        private static void RecursiveCopyBaseTypePrivateFields(object originalObject, IDictionary<object, object> visited, object cloneObject, Type typeToReflect)
        {
            if (typeToReflect.BaseType != null)
            {
                RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect.BaseType);
                CopyFields(originalObject, visited, cloneObject, typeToReflect.BaseType, 
                    BindingFlags.Instance | BindingFlags.NonPublic, info => info.IsPrivate);
            }
        }

        private static void CopyFields(object originalObject, IDictionary<object, object> visited, object cloneObject, Type typeToReflect, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy, Func<FieldInfo, bool> filter = null)
        {
            foreach (FieldInfo fieldInfo in typeToReflect.GetFields(bindingFlags))
            {
                if (filter != null && filter(fieldInfo) == false) continue;
                if (IsPrimitive(fieldInfo.FieldType)) continue;
                var originalFieldValue = fieldInfo.GetValue(originalObject);
                var clonedFieldValue = InternalCopy(originalFieldValue, visited);
                fieldInfo.SetValue(cloneObject, clonedFieldValue);
            }
        }

        internal class ReferenceEqualityComparer : EqualityComparer<Object>
        {
            public override bool Equals(object x, object y)
            {
                return ReferenceEquals(x, y);
            }
            public override int GetHashCode(object obj)
            {
                if (obj == null) return 0;
                return obj.GetHashCode();
            }
        }
    }
}
