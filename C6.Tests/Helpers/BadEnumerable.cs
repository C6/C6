// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using System;

using SC = System.Collections;
using SCG = System.Collections.Generic;


namespace C6.Tests.Helpers
{
    public class BadEnumerable<T> : SCG.IEnumerable<T>
    {
        private readonly SCG.IEnumerable<T> _enumerable;
        private readonly Exception _exception;

        public BadEnumerable(SCG.IEnumerable<T> enumerable, Exception exception = null)
        {
            _enumerable = enumerable;
            _exception = exception ?? new BadEnumerableException();
        }

        public SCG.IEnumerator<T> GetEnumerator()
        {
            foreach (var t in _enumerable) {
                yield return t;
            }
            throw _exception;
        }

        SC.IEnumerator SC.IEnumerable.GetEnumerator() => GetEnumerator();
    }
}