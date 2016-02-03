using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BF2ScriptingEngine.Scripting
{
    /// <summary>
    /// Contains a list of <see cref="ObjectProperty{TKey, TVal}"/> property
    /// references. Much like a <see cref="Dictionary{TKey, TValue}"/>, the
    /// <typeparamref name="Tkey"/> elements will be distinct in the collection.
    /// </summary>
    public class ObjectPropertyDict<TKey, TVal> : ObjectProperty, IObjectPropertyCollection, IEnumerable
    {
        /// <summary>
        /// An internal Dictionary of our Key-Unique Object Properties
        /// </summary>
        internal Dictionary<TKey, ObjectProperty<TKey, TVal>> Items;

        /// <summary>
        /// Gets the <see cref="ObjectProperty{TKey, TVal}"/> value associated 
        /// with the specified key.
        /// </summary>
        /// <param name="index">The <typeparamref name="TKey"/> value to get</param>
        /// <returns></returns>
        public ObjectProperty<TKey, TVal> this[TKey index]
        {
            get { return Items[index]; }
            internal set { Items[index] = value; }
        }

        /// <summary>
        /// Creates a new instance of <see cref="ObjectPropertyDict{TKey, TVal}"/>
        /// </summary>
        /// <param name="name">the property name</param>
        /// <param name="token">the token information</param>
        /// <param name="property">The PropertyInfo object for this instance</param>
        /// <param name="owner">the ConFileObject that owns this property instance</param>
        public ObjectPropertyDict(string name, Token token, PropertyInfo property, ConFileObject owner)
        {
            Name = name;
            Token = token;
            Property = property;
            Owner = owner;
            Items = new Dictionary<TKey, ObjectProperty<TKey, TVal>>();
        }

        /// <summary>
        /// Determines whether the specified key exists in this collection.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContaintsKey(TKey key) => Items.ContainsKey(key);

        /// <summary>
        /// Sets the Value of the specified key (item #1 in <paramref name="values"/>
        /// </summary>
        /// <param name="values">An array, or 2 elements, with the 0 index indicating the Key value,
        /// and index 1 representing the value</param>
        /// <param name="token"></param>
        public override void SetValues(object[] values, Token token = null)
        {
            Token tkn = token ?? Token;

            // Ensure that we have the correct number of arguments
            if (values.Length != 2)
            {
                string error = $"Invalid values count for {tkn.TokenArgs.ReferenceName}; Got {values.Length}, Expecting 2.";
                Logger.Error(error, token?.File, token?.Position ?? 0);
                throw new Exception(error);
            }

            // Convert values and add them to the list
            var item = Create<ObjectProperty<TKey, TVal>>(Property, Owner, tkn);
            item.SetValues(values, tkn);
            Items[item.Value1] = item;

            // Add item to the Entries list
            tkn.File.AddProperty(item);
        }

        public override string ToFileFormat()
        {
            return null;
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)Items).GetEnumerator();
        }
    }
}
