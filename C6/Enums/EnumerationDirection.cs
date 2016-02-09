// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/lundmikkel/C6/blob/master/LICENSE.md for licensing details.


using System;
using System.Diagnostics.Contracts;


namespace C6
{
    /// <summary>
    /// Defines the directions of an enumeration order relative to the original collection.
    /// </summary>
    public enum EnumerationDirection
    {
        /// <summary>
        /// Enumeration order is the same as the original collection.
        /// </summary>
        Forwards,

        /// <summary>
        /// Enumeration order is the opposite of the original collection.
        /// </summary>
        Backwards,
    }



    // TODO: Move to separate file?
    /// <summary>
    /// Provides a set of static methods for <see cref="EnumerationDirection"/>.
    /// </summary>
    public static class EnumerationDirectionExtension
    {
        /// <summary>
        /// Determines if a <see cref="EnumerationDirection"/> is
        /// <see cref="EnumerationDirection.Forwards"/>.
        /// </summary>
        /// <param name="direction">The <see cref="EnumerationDirection"/> to
        /// check.</param>
        /// <returns><c>true</c> if direction is forwards;
        /// otherwise, <c>false</c>.</returns>
        [Pure]
        public static bool IsForward(this EnumerationDirection direction)
        {
            // Argument must be valid enum constant
            Contract.Requires(Enum.IsDefined(typeof(EnumerationDirection), direction));


            return direction == EnumerationDirection.Forwards;
        }
    }
}