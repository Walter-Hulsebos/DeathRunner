using System;

namespace HFSM
{
    /// <summary>
    ///     Thrown when a <see cref="State" /> has been executed without initializing it first, that is,
    ///     calling <see cref="State.Update" />, <see cref="State.FixedUpdate" /> or
    ///     <see cref="State.LateUpdate" /> without calling <see cref="State.Init" /> first.
    /// </summary>
    public class RootStateMachineNotInitializedException : Exception
    {
        public RootStateMachineNotInitializedException(String message) : base(message: message) { }
    }
}