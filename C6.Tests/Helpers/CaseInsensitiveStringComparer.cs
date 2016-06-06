// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using System;

using SCG = System.Collections.Generic;


namespace C6.Tests.Helpers
{
    [Serializable]
    public class CaseInsensitiveStringComparer : SCG.IEqualityComparer<string>, SCG.IComparer<string>
    {
        #region Constructors

        private CaseInsensitiveStringComparer() {}

        #endregion

        #region Properties

        public static CaseInsensitiveStringComparer Default { get; } = new CaseInsensitiveStringComparer();

        #endregion

        #region Public Methods

        public int GetHashCode(string item) => ToLower(item).GetHashCode();

        public bool Equals(string x, string y) => ToLower(x).Equals(ToLower(y));

        // ReSharper disable once StringCompareToIsCultureSpecific
        public int Compare(string x, string y) => ToLower(x).CompareTo(ToLower(y));

        #endregion

        #region Private Methods

        private static string ToLower(string item) => item?.ToLower() ?? string.Empty;

        #endregion
    }
}