// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using NUnit.Framework;

using static C6.Tests.Helpers.CollectionEvent;
using static C6.Tests.Helpers.TestHelper;


namespace C6.Tests
{
    [TestFixture]
    public class EventTests : TestBase
    {
        private IList<string> collection;

        [Test]
        [Category("Exact item match")]
        [Category("Exact count match")]
        public void ICollection_Add()
        {
            var item = Random.GetString();
            var expectedEvents = new[] {
                Added(item, 1, collection),
            };

            Assert.That(() => collection.Add(item), Raises(expectedEvents).For(collection));
        }

        [Test]
        [Category("Exact count match")]
        public void ICollection_Clear()
        {
            var expectedEvents = new[] {
                Cleared(true, collection.Count, null, collection),
            };

            Assert.That(() => collection.Clear(), Raises(expectedEvents).For(collection));
        }

        [Test]
        [Category("Exact item match")]
        [Category("Exact count match")]
        public void ICollection_FindOrAdd()
        {
            var item = Random.GetString();
            var expectedEvents = new[] {
                Added(item, 1, collection),
            };

            Assert.That(() => collection.FindOrAdd(ref item), Raises(expectedEvents).For(collection));
        }

        [Test]
        [Category("Equal item if duplicates")]
        [Category("Exact count match")]
        public void ICollection_Remove_WithDuplicates()
        {
            var item = Random.GetString();
            var expectedEvents = new[] {
                Removed(item, 1, collection),
            };

            Assert.That(() => collection.Remove(item), Raises(expectedEvents).WithEqualButNotSameItems.For(collection));
        }

        [Test]
        [Category("Exact item match")]
        [Category("Exact count match")]
        public void ICollection_Remove_WithOne()
        {
            var item = Random.GetString();
            var expectedEvents = new[] {
                Removed(item, 1, collection),
            };

            Assert.That(() => collection.Remove(item), Raises(expectedEvents).For(collection));
        }

        // TODO: What about the out parameter when removing? It should be the same in the event

        [Test]
        public void ICollection_RemoveRange_WithDuplicates()
        {
            string item1 = "1", item2 = "2", item3 = "3";
            var items = GetStrings(Random);
            var expectedEvents = new[] {
                Removed(item1, 2, collection),
                Removed(item2, 1, collection),
                Removed(item3, 3, collection),
            };
            
            Assert.That(() => collection.RemoveRange(items), Raises(expectedEvents).WithEqualItems.InNoParticularOrder.For(collection));
        }
    }
}