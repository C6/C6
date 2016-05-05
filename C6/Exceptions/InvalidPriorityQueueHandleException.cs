// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using System;


namespace C6
{
    /// <summary>
    ///     Represents errors that occur when an <see cref="IPriorityQueueHandle{T}"/> is used with a priority queue with which
    ///     it is not associated.
    /// </summary>
    [Serializable]
    public class InvalidPriorityQueueHandleException : Exception
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="InvalidPriorityQueueHandleException"/> class.
        /// </summary>
        public InvalidPriorityQueueHandleException() {}

        /// <summary>
        ///     Initializes a new instance of the <see cref="InvalidPriorityQueueHandleException"/> class with a specified error
        ///     message.
        /// </summary>
        /// <param name="message">
        ///     The message that describes the error.
        /// </param>
        public InvalidPriorityQueueHandleException(string message) : base(message) {}
    }
}