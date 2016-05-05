// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

namespace C6
{
    /// <summary>
    ///     Defines the directions of an enumeration order relative to the original collection.
    /// </summary>
    public enum EnumerationDirection
    {
        /// <summary>
        ///     Enumeration order is the same as the original collection.
        /// </summary>
        Forwards = 1,

        /// <summary>
        ///     Enumeration order is the opposite of the original collection.
        /// </summary>
        Backwards = -1,
    }
}