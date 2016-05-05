// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using SCG = System.Collections.Generic;


namespace C6.Tests.Helpers
{
    public class CaseInsensitiveStringComparer : SCG.IEqualityComparer<string>, SCG.IComparer<string>
    {
        private CaseInsensitiveStringComparer() {}

        public static CaseInsensitiveStringComparer Default => new CaseInsensitiveStringComparer();

        public int GetHashCode(string item) => ToLower(item).GetHashCode();

        public bool Equals(string x, string y) => ToLower(x).Equals(ToLower(y));

        // ReSharper disable once StringCompareToIsCultureSpecific
        public int Compare(string x, string y) => ToLower(x).CompareTo(ToLower(y));

        private string ToLower(string item) => item?.ToLower() ?? string.Empty;
    }
}