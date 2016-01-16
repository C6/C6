using System;
using System.Diagnostics.Contracts;
using NUnit.Framework;

namespace C6.Tests.Contracts
{
    [TestFixture]
    public class ViolatesTests
    {
        [Test]
        public void Precondition_HasViolatedPrecondition_ViolatesPrecondition() =>
            Assert.That(HasViolatedPrecondition, Violates.Precondition);

        private static void HasViolatedPrecondition() =>
            Contract.Requires(false);


        [Test]
        public void TypedPrecondition_HasViolatedTypedPrecondition_ViolatesPrecondition() =>
            Assert.That(HasViolatedTypedPrecondition, Violates.TypedPrecondition<ViolatesTestException>());

        private static void HasViolatedTypedPrecondition() =>
            Contract.Requires<ViolatesTestException>(false);


        [Test]
        public void ViolatePrecondition_HasNonViolatedPrecondition_ViolatesPrecondition() =>
            Assert.That(HasNonViolatedPrecondition, Does.Not.ViolatePrecondition());

        private static void HasNonViolatedPrecondition() =>
            Contract.Requires(true);


        [Test]
        public void ViolatePrecondition_HasNonViolatedTypedPrecondition_ViolatesPrecondition() =>
            Assert.That(HasNonViolatedTypedPrecondition, Does.Not.ViolatePrecondition());

        private static void HasNonViolatedTypedPrecondition() =>
            Contract.Requires<ArgumentException>(true);



        // ReSharper disable once ClassNeverInstantiated.Local
        private class ViolatesTestException : Exception { }
    }
}
