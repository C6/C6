using System;

using NUnit.Framework;

using static System.Diagnostics.Contracts.Contract;

using Assert = NUnit.Framework.Assert;


namespace C6.Tests.Contracts
{
    [TestFixture]
    public class ViolatesTests
    {
        [Test]
        public void Precondition_HasViolatedPrecondition_ViolatesPrecondition() =>
            Assert.That(HasViolatedPrecondition, Violates.Precondition);

        private static void HasViolatedPrecondition() =>
            Requires(false);


        [Test]
        public void TypedPrecondition_HasViolatedTypedPrecondition_ViolatesPrecondition() =>
            Assert.That(HasViolatedTypedPrecondition, Violates.TypedPrecondition<ViolatesTestException>());

        private static void HasViolatedTypedPrecondition() =>
            Requires<ViolatesTestException>(false);


        [Test]
        public void ViolatePrecondition_HasNonViolatedPrecondition_ViolatesPrecondition() =>
            Assert.That(HasNonViolatedPrecondition, Does.Not.ViolatePrecondition());

        private static void HasNonViolatedPrecondition() =>
            Requires(true);


        [Test]
        public void ViolatePrecondition_HasNonViolatedTypedPrecondition_ViolatesPrecondition() =>
            Assert.That(HasNonViolatedTypedPrecondition, Does.Not.ViolatePrecondition());

        private static void HasNonViolatedTypedPrecondition() =>
            Requires<ArgumentException>(true);



        // ReSharper disable once ClassNeverInstantiated.Local
        private class ViolatesTestException : Exception { }
    }
}
