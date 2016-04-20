// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using System;


namespace C6.Tests.Helpers
{
    public class CollectionEventHolder<T>
    {
        private readonly CollectionEvent<T>[] _expectedEvents;

        public CollectionEventHolder(CollectionEvent<T>[] expectedEvents)
        {
            _expectedEvents = expectedEvents;
        }

        public CollectionEventHolder<T> InNoParticularOrder()
        {
            // TODO: remember choice
            // return this;
            throw new NotImplementedException();
        }

        public CollectionEventConstraint<T> For(IListenable<T> collection) => new CollectionEventConstraint<T>(collection, _expectedEvents);
    }
}