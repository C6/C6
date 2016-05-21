// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Diagnostics.Contracts;


// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global


namespace C6.Tests.Contracts
{
    /// <summary>
    ///     Represents errors that occur when an precondition is violated during a test.
    /// </summary>
    internal class PreconditionException : Exception
    {
        /// <summary>
        ///     Gets the user-defined message for the precondition.
        /// </summary>
        /// <value>
        ///     The user-defined message for the precondition.
        /// </value>
        [Pure]
        public string UserMessage { get; }


        /// <summary>
        ///     Gets the string representation of the precondition.
        /// </summary>
        /// <value>
        ///     The string representation of the precondition.
        /// </value>
        [Pure]
        public string ConditionText { get; }


        /// <summary>
        ///     Gets the exception type if a custom exception was thrown.
        /// </summary>
        /// <value>
        ///     The exception type thrown by the precondition. <c>null</c>
        ///     if no custom exception type was specified.
        /// </value>
        public Type ExceptionType { get; }


        /// <summary>
        ///     Initializes a new instance of the
        ///     <see cref="PreconditionException"/> class with a specified user message, a string representation of the
        ///     precondition, and an optional exception type.
        /// </summary>
        /// <param name="userMessage">
        ///     The user-defined message for the precondition.
        /// </param>
        /// <param name="conditionText">
        ///     The string representation of the precondition.
        /// </param>
        /// <param name="exceptionType">
        ///     The exception type thrown by the precondition.
        /// </param>
        public PreconditionException(string userMessage, string conditionText, Type exceptionType = null)
        {
            UserMessage = userMessage;
            ConditionText = conditionText;
            ExceptionType = exceptionType;
        }
    }
}