using System;

namespace HFSM
{
    /// <summary>
    ///     Base class for <see cref="EventTransition" /> classes. Implements the same behaviour of a
    ///     <see cref="Transition" /> and listens to events.
    /// </summary>
    internal abstract class EventTransitionBase : Transition
    {
        private protected Boolean eventListened;
        private protected readonly Boolean processInstantly;

        /// <summary>
        ///     Transition class constructor.
        /// </summary>
        /// <param name="from">
        ///     Origin <see cref="StateObject" /> of the transition.
        /// </param>
        /// <param name="to">
        ///     Target <see cref="StateObject" /> of the transition.
        /// </param>
        /// <param name="processInstantly">
        ///     If <see langword="true" />, the transition will be processed immediately. If <see langword="false" />,
        /// </param>
        /// <param name="transitionAction">
        ///     Function executed when the transition occurs.
        /// </param>
        public EventTransitionBase(StateObject from, StateObject to, Action transitionAction = null, Boolean processInstantly = false) :
            base(from: from, to: to, transitionAction: transitionAction)
        {
            this.processInstantly = processInstantly;
        }

        public virtual void ConsumeEvent()
        {
            eventListened = false;
        }

        /// <summary>
        ///     Checks whether all <see cref="Transition.conditions"/> (if any) are met and whether the event this transition
        ///     is subscribed to has been fired.
        /// </summary>
        /// <returns>
        ///     <see langword="true" /> if all <see cref="Transition.conditions"/> (if any) are met and the event was fired,
        ///     <see langword="false" /> otherwise.
        /// </returns>
        public override Boolean AllConditionsMet()
        {
            return eventListened && ConditionsMet();
        }

        /// <summary>
        ///     Checks whether all <see cref="Transition.conditions"/> (if any) are met.
        /// </summary>
        /// <returns>
        ///     <see langword="true" /> if all <see cref="Transition.conditions"/> (if any) are met, <see langword="false"/>
        ///     otherwise.
        /// </returns>
        protected abstract Boolean ConditionsMet();
    }
}