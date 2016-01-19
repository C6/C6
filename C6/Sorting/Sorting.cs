// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/lundmikkel/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Diagnostics.Contracts;
using SCG = System.Collections.Generic;



namespace C6
{
    // TODO: Add extension methods
    /// <summary>
    /// A utility class with functions for sorting arrays with respect to an
    /// <see cref="IComparable{T}"/>.
    /// </summary>
    [Serializable]
    public static partial class Sorting
    {
        #region Code Contracts

        [ContractAbbreviator]
        private static void ContractAbbreviatorSortingParameters<T>(T[] array, int start, int count, SCG.IComparer<T> comparer = null)
        {
            Contract.Requires(array != null);
            Contract.Requires(0 <= start);
            Contract.Requires(0 <= count);
            Contract.Requires(start + count <= array.Length);

            // Either a comparer must be provided, or T must be IComparable<T>
            //Contract.Requires(comparer != null || typeof(T).IsAssignableFrom(typeof(IComparable<T>)));
        }

        #endregion
        


        private partial class Sorter<T>
        {
            private readonly T[] _array;
            private readonly SCG.IComparer<T> _comparer;


            public Sorter(T[] array, SCG.IComparer<T> comparer)
            {
                Contract.Requires(array != null);

                // Either a comparer must be provided, or T must be IComparable<T>
                //Contract.Requires(comparer != null || typeof(T).IsAssignableFrom(typeof(IComparable<T>)));


                _array = array;
                _comparer = comparer ?? SCG.Comparer<T>.Default;
            }


            private int Compare(T x, T y) => _comparer.Compare(x, y);
        }
    }
}
