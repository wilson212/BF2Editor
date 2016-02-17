using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using BF2ScriptingEngine.Scripting.Attributes;

namespace BF2ScriptingEngine.Scripting
{
    /// <summary>
    /// Contains a collection of <see cref="ObjectProperty"/> objects. Every
    /// time a property is assigned a value in the Bf2 con file, a new
    /// <see cref="ObjectProperty"/> object is created, and added to this properties
    /// internal list.
    /// </summary>
    public abstract class ObjectPropertyList : ObjectProperty
    {
        /// <summary>
        /// Gets the number of items in this ObjectPropertyList
        /// </summary>
        public abstract int Count { get; }

        /// <summary>
        /// Takes an array of arguments, and attempts to set the values
        /// of this <see cref="ObjectPropertyList"/>
        /// </summary>
        /// <exception cref="System.Exception">
        /// if the length of values is more/less than the size of this ObjectPropertyList's
        /// arguments
        /// </exception>
        public override abstract void SetValues(object[] values, Token token = null);

        /// <summary>
        /// Removes the <see cref="ObjectProperty"/> at the specified index
        /// </summary>
        /// <param name="index">The index to remove at</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public abstract void RemoveAt(int index);

        /// <summary>
        /// Removes a range of <see cref="ObjectProperty"/>'s from this list
        /// </summary>
        /// <param name="start">The starting index</param>
        /// <param name="count">The number of elements to remove</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// index is less than 0.-or-count is less than 0.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// index and count do not denote a valid range of elements in this
        /// ObjectPropertyList
        /// </exception>
        public abstract void RemoveRange(int start, int count);

        /// <summary>
        /// Converts the value of this property to file format.
        /// Each ObjectProperty in this list is returned on a new
        /// line.
        /// </summary>
        public override abstract string ToFileFormat();
    }

    /// <summary>
    /// Contains a collection of <see cref="ObjectProperty"/> objects. Every
    /// time a property is assigned a value in the Bf2 con file, a new
    /// <see cref="ObjectProperty"/> object is created, and added to this properties
    /// internal list.
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    public class ObjectPropertyList<T1> : ObjectPropertyList, IObjectPropertyCollection, IEnumerable
    {
        /// <summary>
        /// An internal list of all calls to this object property name
        /// </summary>
        internal List<ObjectProperty<T1>> Items;

        /// <summary>
        /// Gets the number of items in this ObjectPropertyList
        /// </summary>
        public override int Count => Items.Count;

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

        /// <summary>
        /// Takes an array of arguments, and attempts to set the values
        /// of this <see cref="ObjectPropertyList"/>
        /// </summary>
        /// <exception cref="System.Exception">
        /// if the length of values is more/less than the size of this ObjectPropertyList's
        /// arguments
        /// </exception>
        public override void SetValues(object[] values, Token token = null)
        {
            Token tkn = token ?? Token;
            var item = Create<ObjectProperty<T1>>(Property, this.Owner, tkn);

            // Add item to the Entries list
            tkn.File?.AddProperty(item);

            // Set values
            item.SetValue(token);
            Items.Add(item);
        }

        /// <summary>
        /// Removes the <see cref="ObjectProperty"/> at the specified index
        /// </summary>
        /// <param name="index">The index to remove at</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public override void RemoveAt(int index)
        {
            // Ensure index
            if (index < 0 || index > (Items.Count - 1))
                throw new ArgumentOutOfRangeException("index", "index was out of range");

            // Get item at index
            var item = Items[index];

            // Remove from internal list
            Items.RemoveAt(index);

            // Remove item from the ConFile as well!
            item.Owner.File?.Entries.Remove(item);
        }

        /// <summary>
        /// Removes a range of <see cref="ObjectProperty"/>'s from this list
        /// </summary>
        /// <param name="start">The starting index</param>
        /// <param name="count">The number of elements to remove</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// index is less than 0.-or-count is less than 0.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// index and count do not denote a valid range of elements in this
        /// ObjectPropertyList
        /// </exception>
        public override void RemoveRange(int start, int count)
        {
            // Get items at index
            var items = Items.GetRange(start, count);

            // Remove from internal list
            Items.RemoveRange(start, count);

            // Remove item from the ConFile as well!
            foreach (var item in items)
                item.Owner.File?.Entries.Remove(item);
        }

        /// <summary>
        /// Converts the value of this property to file format.
        /// Each ObjectProperty in this list is returned on a new
        /// line.
        /// </summary>
        public override string ToFileFormat()
        {
            StringBuilder builder = new StringBuilder();
            foreach (var item in Items)
                builder.AppendLine(item.ToFileFormat());

            return builder.ToString();
        }

        /// <summary>
        /// Returns an <see cref="IEnumerator"/> that itterates
        /// through this ObjectPropertyList
        /// </summary>
        public IEnumerator GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        public IEnumerable<ObjectProperty<T1>> Where(Func<ObjectProperty<T1>, bool> predicate)
        {
            return Items.Where(predicate);
        }
    }

    public class ObjectPropertyList<T1, T2> : ObjectPropertyList, IObjectPropertyCollection, IEnumerable
    {
        /// <summary>
        /// An internal list of all calls to this object property name
        /// </summary>
        internal List<ObjectProperty<T1, T2>> Items;

        /// <summary>
        /// Gets the number of items in this ObjectPropertyList
        /// </summary>
        public override int Count => Items.Count;

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

        /// <summary>
        /// Takes an array of arguments, and attempts to set the values
        /// of this <see cref="ObjectPropertyList"/>
        /// </summary>
        /// <exception cref="System.Exception">
        /// if the length of values is more/less than the size of this ObjectPropertyList's
        /// arguments
        /// </exception>
        public override void SetValues(object[] values, Token token = null)
        {
            var item = Create<ObjectProperty<T1, T2>>(Property, this.Owner, token ?? Token);

            // Add item to the Entries list
            Owner.File?.AddProperty(item);

            // Set values
            item.SetValues(values);
            Items.Add(item);
        }

        /// <summary>
        /// Removes the <see cref="ObjectProperty"/> at the specified index
        /// </summary>
        /// <param name="index">The index to remove at</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public override void RemoveAt(int index)
        {
            // Ensure index
            if (index < 0 || index > (Items.Count - 1))
                throw new ArgumentOutOfRangeException("index", "index was out of range");

            // Get item at index
            var item = Items[index];

            // Remove from internal list
            Items.RemoveAt(index);

            // Remove item from the ConFile as well!
            item.Owner.File?.Entries.Remove(item);
        }

        /// <summary>
        /// Removes a range of <see cref="ObjectProperty"/>'s from this list
        /// </summary>
        /// <param name="start">The starting index</param>
        /// <param name="count">The number of elements to remove</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// index is less than 0.-or-count is less than 0.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// index and count do not denote a valid range of elements in this
        /// ObjectPropertyList
        /// </exception>
        public override void RemoveRange(int start, int count)
        {
            // Get items at index
            var items = Items.GetRange(start, count);

            // Remove from internal list
            Items.RemoveRange(start, count);

            // Remove item from the ConFile as well!
            foreach (var item in items)
                item.Owner.File?.Entries.Remove(item);
        }

        /// <summary>
        /// Converts the value of this property to file format.
        /// Each ObjectProperty in this list is returned on a new
        /// line.
        /// </summary>
        public override string ToFileFormat()
        {
            StringBuilder builder = new StringBuilder();
            foreach (var item in Items)
                builder.AppendLine(item.ToFileFormat());

            return builder.ToString();
        }

        /// <summary>
        /// Returns an <see cref="IEnumerator"/> that itterates
        /// through this ObjectPropertyList
        /// </summary>
        public IEnumerator GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        public IEnumerable<ObjectProperty<T1, T2>> Where(Func<ObjectProperty<T1, T2>, bool> predicate)
        {
            return Items.Where(predicate);
        }
    }

    public class ObjectPropertyList<T1, T2, T3> : ObjectPropertyList, IObjectPropertyCollection, IEnumerable
    {
        /// <summary>
        /// An internal list of all calls to this object property name
        /// </summary>
        internal List<ObjectProperty<T1, T2, T3>> Items;

        /// <summary>
        /// Gets the number of items in this ObjectPropertyList
        /// </summary>
        public override int Count => Items.Count;

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

        /// <summary>
        /// Takes an array of arguments, and attempts to set the values
        /// of this <see cref="ObjectPropertyList"/>
        /// </summary>
        /// <exception cref="System.Exception">
        /// if the length of values is more/less than the size of this ObjectPropertyList's
        /// arguments
        /// </exception>
        public override void SetValues(object[] values, Token token = null)
        {
            // Grab generics
            var item = Create<ObjectProperty<T1, T2, T3>>(Property, this.Owner, token ?? Token);

            // Add item to the Entries list
            Owner.File?.AddProperty(item);

            // Set values
            item.SetValues(values);
            Items.Add(item);
        }

        /// <summary>
        /// Removes the <see cref="ObjectProperty"/> at the specified index
        /// </summary>
        /// <param name="index">The index to remove at</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public override void RemoveAt(int index)
        {
            // Ensure index
            if (index < 0 || index > (Items.Count - 1))
                throw new ArgumentOutOfRangeException("index", "index was out of range");

            // Get item at index
            var item = Items[index];

            // Remove from internal list
            Items.RemoveAt(index);

            // Remove item from the ConFile as well!
            item.Owner.File?.Entries.Remove(item);
        }

        /// <summary>
        /// Removes a range of <see cref="ObjectProperty"/>'s from this list
        /// </summary>
        /// <param name="start">The starting index</param>
        /// <param name="count">The number of elements to remove</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// index is less than 0.-or-count is less than 0.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// index and count do not denote a valid range of elements in this
        /// ObjectPropertyList
        /// </exception>
        public override void RemoveRange(int start, int count)
        {
            // Get items at index
            var items = Items.GetRange(start, count);

            // Remove from internal list
            Items.RemoveRange(start, count);

            // Remove item from the ConFile as well!
            foreach (var item in items)
                item.Owner.File?.Entries.Remove(item);
        }

        /// <summary>
        /// Converts the value of this property to file format.
        /// Each ObjectProperty in this list is returned on a new
        /// line.
        /// </summary>
        public override string ToFileFormat()
        {
            StringBuilder builder = new StringBuilder();
            foreach (var item in Items)
                builder.AppendLine(item.ToFileFormat());

            return builder.ToString();
        }

        /// <summary>
        /// Returns an <see cref="IEnumerator"/> that itterates
        /// through this ObjectPropertyList
        /// </summary>
        public IEnumerator GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        public IEnumerable<ObjectProperty<T1, T2, T3>> Where(
            Func<ObjectProperty<T1, T2, T3>, bool> predicate
        )
        {
            return Items.Where(predicate);
        }
    }

    public class ObjectPropertyList<T1, T2, T3, T4> : ObjectPropertyList, IObjectPropertyCollection, IEnumerable
    {
        /// <summary>
        /// An internal list of all calls to this object property name
        /// </summary>
        internal List<ObjectProperty<T1, T2, T3, T4>> Items;

        /// <summary>
        /// Gets the number of items in this ObjectPropertyList
        /// </summary>
        public override int Count => Items.Count;

        /// <summary>
        /// Gets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get</param>
        /// <returns></returns>
        public ObjectProperty<T1, T2, T3, T4> this[int index]
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
            Items = new List<ObjectProperty<T1, T2, T3, T4>>();
        }

        /// <summary>
        /// Takes an array of arguments, and attempts to set the values
        /// of this <see cref="ObjectPropertyList"/>
        /// </summary>
        /// <exception cref="System.Exception">
        /// if the length of values is more/less than the size of this ObjectPropertyList's
        /// arguments
        /// </exception>
        public override void SetValues(object[] values, Token token = null)
        {
            // Grab generics
            var item = Create<ObjectProperty<T1, T2, T3, T4>>(Property, this.Owner, token ?? Token);

            // Add item to the Entries list
            Owner.File?.AddProperty(item);

            // Set values
            item.SetValues(values);
            Items.Add(item);
        }

        /// <summary>
        /// Removes the <see cref="ObjectProperty"/> at the specified index
        /// </summary>
        /// <param name="index">The index to remove at</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public override void RemoveAt(int index)
        {
            // Ensure index
            if (index < 0 || index > (Items.Count - 1))
                throw new ArgumentOutOfRangeException("index", "index was out of range");

            // Get item at index
            var item = Items[index];

            // Remove from internal list
            Items.RemoveAt(index);

            // Remove item from the ConFile as well!
            item.Owner.File?.Entries.Remove(item);
        }

        /// <summary>
        /// Removes a range of <see cref="ObjectProperty"/>'s from this list
        /// </summary>
        /// <param name="start">The starting index</param>
        /// <param name="count">The number of elements to remove</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// index is less than 0.-or-count is less than 0.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// index and count do not denote a valid range of elements in this
        /// ObjectPropertyList
        /// </exception>
        public override void RemoveRange(int start, int count)
        {
            // Get items at index
            var items = Items.GetRange(start, count);

            // Remove from internal list
            Items.RemoveRange(start, count);

            // Remove item from the ConFile as well!
            foreach (var item in items)
                item.Owner.File?.Entries.Remove(item);
        }

        /// <summary>
        /// Converts the value of this property to file format.
        /// Each ObjectProperty in this list is returned on a new
        /// line.
        /// </summary>
        public override string ToFileFormat()
        {
            StringBuilder builder = new StringBuilder();
            foreach (var item in Items)
                builder.AppendLine(item.ToFileFormat());

            return builder.ToString();
        }

        /// <summary>
        /// Returns an <see cref="IEnumerator"/> that itterates
        /// through this ObjectPropertyList
        /// </summary>
        public IEnumerator GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        public IEnumerable<ObjectProperty<T1, T2, T3, T4>> Where(
            Func<ObjectProperty<T1, T2, T3, T4>, bool> predicate
        )
        {
            return Items.Where(predicate);
        }
    }
}
