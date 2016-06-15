// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using SCG = System.Collections.Generic;


namespace C6.Tests.Helpers
{
    public static class Breaks
    {
        public static EnumeratorConstraint<T> EnumeratorFor<T>(SCG.IEnumerable<T> enumerable) => new EnumeratorConstraint<T>(enumerable);
    }
}