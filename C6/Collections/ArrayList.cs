// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/lundmikkel/C6/blob/master/LICENSE.md for licensing details.

using System.Collections;
using System.Linq;

using SCG = System.Collections.Generic;


namespace C6
{
    public class ArrayList<T> : SCG.IEnumerable<T>
    {
        public ArrayList() {}

        public ArrayList(SCG.IEnumerable<T> enumerable) {}

        public SCG.IEnumerator<T> GetEnumerator()
        {
            return Enumerable.Empty<T>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}