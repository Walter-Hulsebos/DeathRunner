using System;

namespace HFSM
{
    /// <summary>
    ///     Transition behaviour of a hierarchical finite state machine pattern.
    /// </summary>
    internal class Transition
    {
        protected readonly Func<Boolean>[] conditions;

        /// <summary>
        ///     Transition class constructor.
        /// </summary>
        /// <param name="from">
        ///     Origin <see cref="StateObject" /> of the transition.
        /// </param>
        /// <param name="to">
        ///     Target <see cref="StateObject" /> of the transition.
        /// </param>
        /// <param name="transitionAction">
        ///     Function executed when the transition occurs.
        /// </param>
        /// <param name="conditions">
        ///     List of conditions that must be met (all of them) in order for the transition to occur.
        /// </param>
        public Transition(StateObject from, StateObject to, Action transitionAction = null, params Func<Boolean>[] conditions)
        {
            this.From = from;
            this.To   = to;
            this.TransitionAction  = transitionAction;
            this.conditions        = conditions;
        }

        internal StateObject From { get; }
        internal StateObject To   { get; }
        internal Action TransitionAction { get; }

        /// <summary>
        ///     Checks whether all <see cref="conditions" /> are met or not.
        /// </summary>
        /// <returns>
        ///     <see langword="true" /> if all <see cref="conditions" /> are met, <see langword="false" /> otherwise.
        /// </returns>
        public virtual Boolean AllConditionsMet()
        {
            foreach (Func<Boolean> __condition in conditions)
            {
                if (!__condition())
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        ///     Executes <see cref="Transition.TransitionAction" />, if defined.
        /// </summary>
        public virtual void InvokeTransitionAction()
        {
            TransitionAction?.Invoke();
        }
    }
}