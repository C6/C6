// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using SCG = System.Collections.Generic;


namespace C6.Tests.Helpers
{
    public class TenEqualityComparer : SCG.IEqualityComparer<int>, SCG.IComparer<int>
    {
        private TenEqualityComparer() {}

        public static TenEqualityComparer Default => new TenEqualityComparer();

        public int GetHashCode(int item) => (item / 10).GetHashCode();

        public bool Equals(int x, int y) => x / 10 == y / 10;

        public int Compare(int x, int y) => (x / 10).CompareTo(y / 10);
    }
}