// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Diagnostics.Contracts;
using System.Text;

using SCG = System.Collections.Generic;


namespace C6
{
    // TODO: Add contracts
    // TODO: Make IEquatable<SCG.KeyValuePair<K,V>?
    // TODO: Implement IShowable and IFormattable members properly
    /// <summary>
    ///     Defines a key/value pair that is equatable and showable.
    /// </summary>
    [Serializable]
    public struct KeyValuePair<TKey, TValue> : IEquatable<KeyValuePair<TKey, TValue>>, IShowable
    {
        #region Properties

        /// <summary>
        ///     Gets the key in the key/value pair.
        /// </summary>
        /// <value>
        ///     The key in the key/value pair.
        /// </value>
        [Pure]
        public TKey Key { get; }


        /// <summary>
        ///     Gets the key in the key/value pair.
        /// </summary>
        /// <value>
        ///     The value in the key/value pair.
        /// </value>
        [Pure]
        public TValue Value { get; }

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="KeyValuePair{TKey,TValue}"/> structure with the specified key.
        /// </summary>
        /// <param name="key">
        ///     The key in the key/value pair.
        /// </param>
        /// <remarks>
        ///     The value is set to the default value of type <typeparamref name="TValue"/>.
        /// </remarks>
        public KeyValuePair(TKey key) : this(key, default(TValue)) {}


        /// <summary>
        ///     Initializes a new instance of the <see cref="KeyValuePair{TKey,TValue}"/> structure with the specified key and
        ///     value.
        /// </summary>
        /// <param name="key">
        ///     The key in the key/value pair.
        /// </param>
        /// <param name="value">
        ///     The value in the key/value pair.
        /// </param>
        public KeyValuePair(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }

        #endregion

        #region Object Methods

        /// <summary>
        ///     Returns a string representation of the
        ///     <see cref="KeyValuePair{TKey,TValue}"/>, using the string representations of the key and value.
        /// </summary>
        /// <returns>
        ///     A string representation of the
        ///     <see cref="KeyValuePair{TKey,TValue}"/>.
        /// </returns>
        [Pure]
        public override string ToString() => $"({Key}, {Value})";


        /// <summary>
        ///     Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">
        ///     The object to compare with the current instance.
        /// </param>
        /// <returns>
        ///     <c>true</c> if <paramref name="obj"/> and this instance are the same type and represent the same value; otherwise,
        ///     <c>false</c>.
        /// </returns>
        [Pure]
        public override bool Equals(object obj)
        {
            var other = obj as KeyValuePair<TKey, TValue>?;
            return other.HasValue && Equals(other.Value);
        }


        /// <summary>
        ///     Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        ///     A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        [Pure]
        public override int GetHashCode()
            // Uses SCG.EqualityComparer as Key and Value could be null
            => SCG.EqualityComparer<TKey>.Default.GetHashCode(Key) + 13984681 * SCG.EqualityComparer<TValue>.Default.GetHashCode(Value);

        #endregion

        #region IEquatable Members

        /// <summary>
        ///     Indicates whether the current key-value pair is equal to the specified key-value pair.
        /// </summary>
        /// <param name="other">
        ///     The specified key-value pair to compare with this key-value pair.
        /// </param>
        /// <returns>
        ///     <c>true</c> if this key-value pair is equal to the specified key-value pair; otherwise, <c>false</c>.
        /// </returns>
        [Pure]
        public bool Equals(KeyValuePair<TKey, TValue> other) => Key.Equals(other.Key) && Value.Equals(other.Value);


        /// <summary>
        ///     Indicates whether the two key-value pair are equal to each other.
        /// </summary>
        /// <param name="left">The left key-value pair.</param>
        /// <param name="right">The right key-value pair.</param>
        /// <returns>
        ///     <c>true</c> if the two key-value pairs are equal; otherwise, <c>false</c>.
        /// </returns>
        [Pure]
        public static bool operator ==(KeyValuePair<TKey, TValue> left, KeyValuePair<TKey, TValue> right) => left.Equals(right);


        /// <summary>
        ///     Indicates whether the two key-value pair are not equal to each other.
        /// </summary>
        /// <param name="left">The left key-value pair.</param>
        /// <param name="right">The right key-value pair.</param>
        /// <returns>
        ///     <c>true</c> if the two key-value pairs are not equal; otherwise, <c>false</c>.
        /// </returns>
        [Pure]
        public static bool operator !=(KeyValuePair<TKey, TValue> left, KeyValuePair<TKey, TValue> right) => !left.Equals(right);

        #endregion

        #region IShowable Members
        
        public bool Show(StringBuilder stringBuilder, ref int rest, IFormatProvider formatProvider)
        {
            throw new NotImplementedException();

            // if (rest < 0)
            //     return false;
            // if (!Showing.Show(Key, stringBuilder, ref rest, formatProvider))
            //     return false;
            // stringBuilder.Append(" => ");
            // rest -= 4;
            // if (!Showing.Show(Value, stringBuilder, ref rest, formatProvider))
            //     return false;
            // return rest >= 0;
        }

        #endregion

        #region IFormattable Members

        /// <summary>
        ///     Formats the value of the current instance using the specified format.
        /// </summary>
        /// <param name="format">
        ///     The format to use, or a <c>null</c> reference to use the default format defined for the type of the
        ///     <see cref="IFormattable"/> implementation.
        /// </param>
        /// <param name="formatProvider">
        ///     The provider to use to format the value, or a <c>null</c> reference to obtain the numeric format information from
        ///     the current locale setting of the operating system.
        /// </param>
        /// <returns>
        ///     The value of the current instance in the specified format.
        /// </returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            throw new NotImplementedException();

            // return Showing.ShowString(this, format, formatProvider);
        }

        #endregion
    }
}