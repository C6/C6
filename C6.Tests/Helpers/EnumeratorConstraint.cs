// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using System;

using NUnit.Framework;
using NUnit.Framework.Constraints;

using static C6.Collections.ExceptionMessages;

using SCG = System.Collections.Generic;


namespace C6.Tests.Helpers
{
    // TODO: Document
    // TODO: Write automated tests
    public class EnumeratorConstraint<T> : Constraint
    {
        private readonly SCG.IEnumerator<T> _enumerator;

        public EnumeratorConstraint(SCG.IEnumerable<T> enumerable)
        {
            _enumerator = enumerable.GetEnumerator();
            // Move next to get an actual enumerator object
            _enumerator.MoveNext();
        }


        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            var testDelegate = actual as TestDelegate;

            if (testDelegate == null) {
                throw new NotSupportedException($"{nameof(ConstraintResult)} only works with delegates.");
            }

            testDelegate.Invoke();
            return MoveNext();
        }

        public override ConstraintResult ApplyTo<TActual>(ActualValueDelegate<TActual> del)
        {
            del();
            return MoveNext();
        }

        private ConstraintResult MoveNext()
        {
            try {
                // Call enumerator to ensure it throws an exception
                _enumerator.MoveNext();
            }
            catch (InvalidOperationException exception) {
                // Correct exception, check message
                Description = $"<System.InvalidOperationException> and property Message equal to \"{CollectionWasModified}\"";
                return new ConstraintResult(this, exception.Message, exception.Message.Equals(CollectionWasModified));
            }
            catch (Exception ex) {
                // Wrong exception thrown
                Description = "<System.InvalidOperationException>";
                return new EnumeratorConstraintResult(this, ex);
            }

            // No exception thrown
            Description = "<System.InvalidOperationException>";
            return new EnumeratorConstraintResult(this, null);
        }


        private class EnumeratorConstraintResult : ConstraintResult
        {
            public EnumeratorConstraintResult(EnumeratorConstraint<T> constraint, Exception caughtException) : base(constraint, caughtException, false) {}

            public override void WriteActualValueTo(MessageWriter writer)
            {
                if (Status == ConstraintStatus.Failure) {
                    writer.Write(ActualValue == null ? "no exception thrown" : $"<{ActualValue.GetType().FullName}>");
                }
                else {
                    base.WriteActualValueTo(writer);
                }
            }
        }
    }
}