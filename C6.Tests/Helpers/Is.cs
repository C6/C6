// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/lundmikkel/C6/blob/master/LICENSE.md for licensing details.

using C6.Tests.Helpers;


namespace C6.Tests
{
    // ReSharper disable once InconsistentNaming
    // ReSharper disable once ClassNeverInstantiated.Global
    public class _Is : NUnit.Framework.Is
    {
        public static CollectionEventHolder<T> Raising<T>(CollectionEvent<T>[] expectedEvents) => new CollectionEventHolder<T>(expectedEvents);
    }
}