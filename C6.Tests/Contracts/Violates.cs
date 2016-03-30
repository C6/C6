// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/lundmikkel/C6/blob/master/LICENSE.md for licensing details.

using System;

using NUnit.Framework;
using NUnit.Framework.Constraints;


namespace C6.Tests.Contracts
{
    /// <summary>
    /// Provides helper methods for writing tests that assert precondition
    /// violations.
    /// </summary>
    public static class Violates
    {
        /// <summary>
        /// Gets <see cref="Throws.TypeOf"/> with a
        /// <see cref="PreconditionException"/>.
        /// </summary>
        /// <value><see cref="Throws.TypeOf"/> with a
        /// <see cref="PreconditionException"/>.</value>
        /// <remarks>Allows a precondition violation to be asserted using
        /// <c>Assert.That(code, Violates.Precondition)</c>.</remarks>
        /// <remarks>Allows a precondition violation to be asserted:
        /// <code>Assert.That(code, Violates.Precondition);</code></remarks>
        public static TypeConstraint Precondition
        {
            get
            {
#if (!DEBUG)
                Assert.Ignore("Ignore preconditions in release.");
#endif

                return Throws.TypeOf<PreconditionException>();
            }
        }
        
        /// <summary>
        /// Gets <see cref="Throws.TypeOf"/> with a
        /// <see cref="PreconditionException"/> and a user message.
        /// </summary>
        /// <returns><see cref="Throws.TypeOf"/> with a
        /// <see cref="PreconditionException"/> and a user message.</returns>
        /// <remarks>Allows a precondition violation to be asserted:
        /// <code>Assert.That(code, Violates.PreconditionSaying("Argument must be non-null"));</code></remarks>
        public static EqualConstraint PreconditionSaying(string userMessage)
        {
#if (!DEBUG)
            Assert.Ignore("Ignore preconditions in release.");
#endif

            return Throws.TypeOf<PreconditionException>().With.Property("UserMessage").EqualTo(userMessage);
        }

        // TODO: Remove this once the issue is solved: https://github.com/Microsoft/CodeContracts/issues/392
        // TODO: Add one assessing the user message
        public static StartsWithConstraint ConstructorPrecondition => Throws.InstanceOf<Exception>().With.Message.StartsWith("Precondition failed");

        /// <summary>
        /// Returns an <see cref="EqualConstraint"/> that checks if a typed
        /// precondition was violated.
        /// </summary>
        /// <remarks>Allows a precondition violation to be asserted using
        /// <code>Assert.That(code, Violates.TypedPrecondition&lt;ExceptionType&gt;())</code>
        /// </remarks>
        public static EqualConstraint TypedPrecondition<TException>() where TException : Exception
        {
#if (!DEBUG)
            Assert.Ignore("Ignore preconditions in release.");
#endif

            return Throws.TypeOf<PreconditionException>().With.Property("ExceptionType").EqualTo(typeof(TException));
        }

        /// <summary>
        /// Returns a <see cref="Throws.Nothing"/> constraint.
        /// </summary>
        /// <param name="ignored">An ignored expression.</param>
        /// <returns>A <seealso cref="Throws.Nothing"/> constraint.</returns>
        /// <remarks>This should only be used as
        /// <c>Assert.That(code, Does.Not.ViolatePrecondition())</c>.</remarks>
        public static ThrowsNothingConstraint ViolatePrecondition(this ConstraintExpression ignored) => Throws.Nothing;
    }
}