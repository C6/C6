// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using System;

using NUnit.Framework;

using static System.Diagnostics.Contracts.Contract;

using Assert = NUnit.Framework.Assert;


namespace C6.Tests.Contracts
{
    [TestFixture]
    public class ViolatesTests
    {
        private static string UserMessage => "DhH4GjZT7z";

        [Test]
        public void Precondition_HasViolatedPrecondition_ViolatesPrecondition() =>
            Assert.That(HasViolatedPrecondition, Violates.Precondition);

        private static void HasViolatedPrecondition() =>
            Requires(false);

        [Test]
        public void Precondition_HasViolatedPreconditionWithUserMessage_ViolatesPrecondition() =>
            Assert.That(HasViolatedPreconditionWithUserMessage, Violates.PreconditionSaying(UserMessage));

        private static void HasViolatedPreconditionWithUserMessage() =>
            Requires(false, UserMessage);


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
        private class ViolatesTestException : Exception {}
    }
}