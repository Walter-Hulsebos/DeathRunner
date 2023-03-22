using System;

namespace HFSM
{
    /// <summary>
    ///     Thrown when a <see cref="State" /> has been created without passsing any
    ///     <see cref="StateObject" /> as argument.
    /// </summary>
    public class StatelessStateMachineException : Exception
    {
        public StatelessStateMachineException(String message) : base(message: message) { }
    }
}