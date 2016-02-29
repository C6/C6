// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/lundmikkel/C6/blob/master/LICENSE.md for licensing details.

using System.Collections;
using System.Linq;

using SCG = System.Collections.Generic;


namespace C6
{
    public class ArrayList<T> : SCG.IEnumerable<T>
    {
        private readonly SCG.IEnumerable<T> _enumerable;

        public ArrayList()
        {
            _enumerable = Enumerable.Empty<T>();
        }

        public ArrayList(SCG.IEnumerable<T> enumerable)
        {
            _enumerable = enumerable;
        }

        public SCG.IEnumerator<T> GetEnumerator()
        {
            return _enumerable.GetEnumerator();
        }

        #region Explicit Implementations

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

    }
}