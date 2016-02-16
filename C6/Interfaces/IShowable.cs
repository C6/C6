// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/lundmikkel/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Diagnostics.Contracts;
using System.Text;


namespace C6
{
    /// <summary>
    /// Provides functionality to format the value of an object into a string
    /// representation within a limited number of characters and append it to a
    /// <see cref="StringBuilder"/>.
    /// </summary>
    [ContractClass(typeof(IShowableContract))]
    public interface IShowable : IFormattable
    {
        //TODO: Replace StringBuilder with TextWriters?
        // TODO: Why is it "approximately"?
        /// <summary>
        /// Formats the value of the current instance with the specified format
        /// using at most approximately <paramref name="rest"/> characters and 
        /// appends the (possibly truncated) result to
        /// <paramref name="stringBuilder"/>. Subtracts the actual number of 
        /// used characters from <c>rest</c>.
        /// </summary>
        /// <param name="stringBuilder">The string builder to which the
        /// formatted string is appended.</param>
        /// <param name="rest">The number of characters to fit the formatted
        /// string to. The actual number of used characters is subtracted from
        /// the parameter on return.</param>
        /// <param name="formatProvider">The provider to use to format the
        /// value, or a null reference to obtain the numeric format information 
        /// from the current locale setting of the operating system.
        /// </param>
        /// <returns><c>true</c> if the appended formatted string was complete
        /// (not truncated); otherwise, <c>false</c>.</returns>
        /// <remarks>If the instance cannot not be formatted within
        /// <paramref name="rest"/> characters, then ellipses "..." are used to
        /// indicate missing pieces in the resulting output.</remarks>
        [Pure]
        bool Show(StringBuilder stringBuilder, ref int rest, IFormatProvider formatProvider);
    }


    [ContractClassFor(typeof(IShowable))]
    internal abstract class IShowableContract : IShowable
    {
        // ReSharper disable InvocationIsSkipped

        public bool Show(StringBuilder stringBuilder, ref int rest, IFormatProvider formatProvider)
        {
            // Argument must be non-null
            Contract.Requires(stringBuilder != null); // TODO: Use <ArgumentNullException>?

            // Returns true if rest >= 0 on return; otherwise, false.
            Contract.Ensures(Contract.Result<bool>() == (Contract.ValueAtReturn(out rest) >= 0));
            // The length of the formatted string is subtracted from rest
            Contract.Ensures(stringBuilder.Length - Contract.OldValue(stringBuilder.Length) == Contract.OldValue(rest) - Contract.ValueAtReturn(out rest));

            throw new NotImplementedException();
        }

        // ReSharper restore InvocationIsSkipped

        #region Non-Contract Methods

        public abstract string ToString(string format, IFormatProvider formatProvider);

        #endregion
    }
}