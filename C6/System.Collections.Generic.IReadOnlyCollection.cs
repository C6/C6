// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/lundmikkel/C6/blob/master/LICENSE.md for licensing details.

using System.Diagnostics.Contracts;



namespace System.Collections.Generic
{
    /// <summary>
    /// Represents a strongly-typed, read-only collection of elements.
    /// </summary>
    /// <typeparam name="T">The type of the elements.</typeparam>
    /// <remarks>
    /// Enables <c>System.Collections.Generic.IReadOnlyCollection</c> to be
    /// used in .NET 4.5 projects.
    /// </remarks>
    [ContractClass(typeof(IReadOnlyCollectionContract<>))]
    public interface IReadOnlyCollection<out T> : IEnumerable<T>
    {
        /// <summary>
        /// Gets the number of elements in the collection.
        /// </summary>
        /// <value>The number of elements in the collection.</value>
        [Pure]
        int Count { get; }
    }


    [ContractClassFor(typeof(IReadOnlyCollection<>))]
    internal abstract class IReadOnlyCollectionContract<T> : IReadOnlyCollection<T>
    {
        // ReSharper disable InvocationIsSkipped

        public int Count
        {
            get
            {
                // No Requires


                // Returns a non-negative number
                Contract.Ensures(Contract.Result<int>() >= 0);


                throw new NotImplementedException();
            }
        }

        // ReSharper restore InvocationIsSkipped


        #region Non-Contract Methods

        public IEnumerator<T> GetEnumerator() { throw new NotImplementedException(); }
        IEnumerator IEnumerable.GetEnumerator() { throw new NotImplementedException(); }

        #endregion
    }
}