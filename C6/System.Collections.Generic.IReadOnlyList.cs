// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/lundmikkel/C6/blob/master/LICENSE.md for licensing details.

using System.Diagnostics.Contracts;

using static System.Diagnostics.Contracts.Contract;

using static C6.Contracts.ContractMessage;


namespace System.Collections.Generic
{
    /// <summary>
    /// Represents a read-only collection of elements that can be accessed by
    /// index. 
    /// </summary>
    /// <typeparam name="T">The type of the elements.</typeparam>
    /// <remarks>
    /// Enables <c>System.Collections.Generic.IReadOnlyCollection</c> to be
    /// used in .NET 4.5 projects.
    /// </remarks>
    [ContractClass(typeof(IReadOnlyListContract<>))]
    public interface IReadOnlyList<out T> : IReadOnlyCollection<T>
    {
        /// <summary>
        /// Gets the element at the specified index in the read-only list.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.
        /// </param>
        /// <value>The element at the specified index in the read-only list.
        /// </value>
        [Pure]
        T this[int index] { get; }
    }


    [ContractClassFor(typeof(IReadOnlyList<>))]
    internal abstract class IReadOnlyListContract<T> : IReadOnlyList<T>
    {
        // ReSharper disable InvocationIsSkipped

        public T this[int index]
        {
            get
            {
                // Argument must be within bounds (collection must be non-empty)
                Requires(0 <= index, ArgumentMustBeWithinBounds);
                Requires(index < Count, ArgumentMustBeWithinBounds);

                return default(T);
            }
            set
            {
                // Argument must be within bounds (collection must be non-empty)
                Requires(0 <= index, ArgumentMustBeWithinBounds);
                Requires(index < Count, ArgumentMustBeWithinBounds);

                return;
            }
        }

        // ReSharper restore InvocationIsSkipped

        #region Non-Contract Methods

        public abstract int Count { get; }
        public abstract IEnumerator<T> GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
    }
}