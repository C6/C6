// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

namespace C6
{
    // TODO: Add information about amortize/expected. See http://docs.scala-lang.org/overviews/collections/performance-characteristics.html
    // Values are order to be comparable: Constant < Log < Linear < PotentiallyInfinite
    /// <summary>
    ///     The symbolic characterization of the speed of lookups for a collection.
    /// </summary>
    /// <remarks>
    ///     The values may refer to worst-case, amortized and/or expected asymptotic complexity with regards to the collection
    ///     size.
    /// </remarks>
    public enum Speed
    {
        /// <summary>
        ///     The operation takes constant time.
        /// </summary>
        Constant,

        /// <summary>
        ///     The operation may take time proportional to the logarithm of the collection size.
        /// </summary>
        Log,

        /// <summary>
        ///     The operation may take time proportional to the collection size.
        /// </summary>
        Linear,

        /// <summary>
        ///     The operation might never end.
        /// </summary>
        PotentiallyInfinite,
    }
}