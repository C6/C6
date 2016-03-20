// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/lundmikkel/C6/blob/master/LICENSE.md for licensing details.

namespace C6.Tests.Helpers
{
    public class CollectionEventHolder<T>
    {
        private readonly CollectionEvent<T>[] _expectedEvents;

        public CollectionEventHolder(CollectionEvent<T>[] expectedEvents)
        {
            _expectedEvents = expectedEvents;
        }

        public CollectionEventConstraint<T> For(ICollectionValue<T> collection) => new CollectionEventConstraint<T>(collection, _expectedEvents);
    }
}