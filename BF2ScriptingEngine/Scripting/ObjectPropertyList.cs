using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BF2ScriptingEngine.Scripting
{
    /// <summary>
    /// Contains a collection of <see cref="ObjectProperty"/> objects. Every
    /// time a property is assigned a value in the Bf2 con file, a new
    /// <see cref="ObjectProperty"/> object is created with the vaules.
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    public class ObjectPropertyList<T1> : ObjectProperty, IObjectPropertyCollection, IEnumerable
    {
        /// <summary>
        /// An internal list of all calls to this object property name
        /// </summary>
        internal List<ObjectProperty<T1>> Items;

        /// <summary>
        /// Gets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get</param>
        /// <returns></returns>
        public ObjectProperty<T1> this[int index]
        {
            get
            {
                return Items[index];
            }
        }

        /// <summary>
        /// Creates a new instance of <see cref="ObjectPropertyList{T}"/>
        /// </summary>
        /// <param name="name">the property name</param>
        /// <param name="token">the token information</param>
        /// <param name="property">The PropertyInfo object for this instance</param>
        /// <param name="owner">the ConFileObject that owns this property instance</param>
        public ObjectPropertyList(string name, Token token, PropertyInfo property, ConFileObject owner)
        {
            Name = name;
            Token = token;
            Property = property;
            Owner = owner;
            Items = new List<ObjectProperty<T1>>();
        }

        public override void SetValues(object[] values, Token token = null)
        {
            Token tkn = token ?? Token;
            var item = Create<ObjectProperty<T1>>(Property, this.Owner, tkn);

            // Add item to the Entries list
            tkn.File.AddProperty(item);

            // Set values
            item.SetValues(values);
            Items.Add(item);
        }

        public override string ToFileFormat()
        {
            StringBuilder builder = new StringBuilder();
            foreach (var item in Items)
                builder.AppendLine(item.ToFileFormat());

            return builder.ToString();
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)Items).GetEnumerator();
        }
    }

    public class ObjectPropertyList<T1, T2> : ObjectProperty, IObjectPropertyCollection, IEnumerable
    {
        /// <summary>
        /// An internal list of all calls to this object property name
        /// </summary>
        internal List<ObjectProperty<T1, T2>> Items;

        /// <summary>
        /// Gets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get</param>
        /// <returns></returns>
        public ObjectProperty<T1, T2> this[int index]
        {
            get
            {
                return Items[index];
            }
        }

        /// <summary>
        /// Creates a new instance of <see cref="ObjectPropertyList{T1,T2}"/>
        /// </summary>
        /// <param name="name">the property name</param>
        /// <param name="token">the token information</param>
        /// <param name="property">The PropertyInfo object for this instance</param>
        /// <param name="owner">the ConFileObject that owns this property instance</param>
        public ObjectPropertyList(string name, Token token, PropertyInfo property, ConFileObject owner)
        {
            Name = name;
            Token = token;
            Property = property;
            Owner = owner;

            Items = new List<ObjectProperty<T1, T2>>();
        }

        public override void SetValues(object[] values, Token token = null)
        {
            var item = Create<ObjectProperty<T1, T2>>(Property, this.Owner, token ?? Token);

            // Add item to the Entries list
            Owner.File.AddProperty(item);

            // Set values
            item.SetValues(values);
            Items.Add(item);
        }

        public override string ToFileFormat()
        {
            StringBuilder builder = new StringBuilder();
            foreach (var item in Items)
                builder.AppendLine(item.ToFileFormat());

            return builder.ToString();
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)Items).GetEnumerator();
        }
    }

    public class ObjectPropertyList<T1, T2, T3> : ObjectProperty, IObjectPropertyCollection, IEnumerable
    {
        /// <summary>
        /// An internal list of all calls to this object property name
        /// </summary>
        internal List<ObjectProperty<T1, T2, T3>> Items;

        /// <summary>
        /// Gets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get</param>
        /// <returns></returns>
        public ObjectProperty<T1, T2, T3> this[int index]
        {
            get
            {
                return Items[index];
            }
        }

        /// <summary>
        /// Creates a new instance of <see cref="ObjectPropertyList{T1,T2}"/>
        /// </summary>
        /// <param name="name">the property name</param>
        /// <param name="token">the token information</param>
        /// <param name="property">The PropertyInfo object for this instance</param>
        /// <param name="owner">the ConFileObject that owns this property instance</param>
        public ObjectPropertyList(string name, Token token, PropertyInfo property, ConFileObject owner)
        {
            Name = name;
            Token = token;
            Property = property;
            Owner = owner;
            Items = new List<ObjectProperty<T1, T2, T3>>();
        }

        public override void SetValues(object[] values, Token token = null)
        {
            // Grab generics
            var item = Create<ObjectProperty<T1, T2, T3>>(Property, this.Owner, token ?? Token);

            // Add item to the Entries list
            Owner.File.AddProperty(item);

            // Set values
            item.SetValues(values);
            Items.Add(item);
        }

        public override string ToFileFormat()
        {
            StringBuilder builder = new StringBuilder();
            foreach (var item in Items)
                builder.AppendLine(item.ToFileFormat());

            return builder.ToString();
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)Items).GetEnumerator();
        }
    }
}
