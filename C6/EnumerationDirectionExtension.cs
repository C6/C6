// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Diagnostics.Contracts;

using static System.Diagnostics.Contracts.Contract;

using static C6.Contracts.ContractMessage;


namespace C6
{
    /// <summary>
    ///     Provides a set of static methods for <see cref="EnumerationDirection"/>.
    /// </summary>
    public static class EnumerationDirectionExtension
    {
        /// <summary>
        ///     Determines if a <see cref="EnumerationDirection"/> is <see cref="EnumerationDirection.Forwards"/>.
        /// </summary>
        /// <param name="direction">
        ///     The <see cref="EnumerationDirection"/> to check.
        /// </param>
        /// <returns>
        ///     <c>true</c> if direction is forwards; otherwise, <c>false</c>.
        /// </returns>
        [Pure]
        public static bool IsForward(this EnumerationDirection direction)
        {
            // Argument must be valid enum constant
            Requires(Enum.IsDefined(typeof(EnumerationDirection), direction), EnumMustBeDefined);


            return direction == EnumerationDirection.Forwards;
        }

        /// <summary>
        ///     Determines if a <see cref="EnumerationDirection"/> is the opposite direction of another
        ///     <see cref="EnumerationDirection"/>.
        /// </summary>
        /// <param name="direction">
        ///     The <see cref="EnumerationDirection"/> to check.
        /// </param>
        /// <param name="otherDirection">
        ///     The <see cref="EnumerationDirection"/> to check against.
        /// </param>
        /// <returns>
        ///     <c>true</c> if directions are opposite of each other; otherwise, <c>false</c>.
        /// </returns>
        [Pure]
        public static bool IsOppositeOf(this EnumerationDirection direction, EnumerationDirection otherDirection)
        {
            // Argument must be valid enum constant
            Requires(Enum.IsDefined(typeof(EnumerationDirection), direction), EnumMustBeDefined);

            // Argument must be valid enum constant
            Requires(Enum.IsDefined(typeof(EnumerationDirection), otherDirection), EnumMustBeDefined);


            return direction != otherDirection;
        }

        /// <summary>
        ///     Returns the opposite direction.
        /// </summary>
        /// <param name="direction">
        ///     The direction to get the opposite of.
        /// </param>
        /// <returns>
        ///     <see cref="EnumerationDirection.Backwards"/> if direction is <see cref="EnumerationDirection.Forwards"/>;
        ///     otherwise, <see cref="EnumerationDirection.Forwards"/>.
        /// </returns>
        [Pure]
        public static EnumerationDirection Opposite(this EnumerationDirection direction)
        {
            // Argument must be valid enum constant
            Requires(Enum.IsDefined(typeof(EnumerationDirection), direction), EnumMustBeDefined);

            // Result is the opposite direction
            Ensures(direction == EnumerationDirection.Forwards
                ? Result<EnumerationDirection>() == EnumerationDirection.Backwards
                : direction == EnumerationDirection.Backwards && Result<EnumerationDirection>() == EnumerationDirection.Forwards);


            return direction.IsForward() ? EnumerationDirection.Backwards : EnumerationDirection.Forwards;
        }
    }
}