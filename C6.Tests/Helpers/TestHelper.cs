// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/lundmikkel/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Linq;

using NUnit.Framework.Internal;

using SCG = System.Collections.Generic;


namespace C6.Tests.Helpers
{
    public static class TestHelper
    {
        public static int GetRandomCount(Random random)
            => random.Next(5, 20);

        public static SCG.IEnumerable<int> GetRandomIntEnumerable(Random random)
            => GetRandomIntEnumerable(random, GetRandomCount(random));

        public static SCG.IEnumerable<int> GetRandomIntEnumerable(Random random, int count)
            => Enumerable.Range(0, count).Select(i => random.Next());

        public static SCG.IEnumerable<string> GetRandomStringEnumerable(Randomizer random)
            => GetRandomStringEnumerable(random, GetRandomCount(random));

        public static SCG.IEnumerable<string> GetRandomStringEnumerable(Randomizer random, int count)
            => Enumerable.Range(0, count).Select(i => random.GetString());
    }
}