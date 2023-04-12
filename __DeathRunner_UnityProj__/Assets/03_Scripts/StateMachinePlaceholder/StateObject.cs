using System;
using System.Collections.Generic;

namespace HFSM
{
    /// <summary>
    ///     Base class for <see cref="StateLeaf" /> and <see cref="State" /> objects.
    /// </summary>
    public abstract class StateObject
    {
        private protected readonly List<EventTransitionBase> eventTransitions;
        private protected readonly List<Transition> transitions;

        /// <summary>
        ///     <see cref="StateObject" /> class constructor.
        /// </summary>
        public StateObject()
        {
            transitions      = new List<Transition>();
            eventTransitions = new List<EventTransitionBase>();
            IsActive         = false;
        }

        internal State State { get; set; }
        internal Boolean IsActive { get; private protected set; }

        /// <summary>
        ///     Indicates whether some other <see cref="StateObject" /> is "equal to" this one.
        /// </summary>
        /// <param name="otherStateObject">
        ///     The <see cref="StateObject" /> with which to compare.
        /// </param>
        /// <returns>
        ///     <see langword="true" /> if the the <see cref="StateObject" />s are of the same subclass, <see langword="false" />
        ///     otherwise.
        /// </returns>
        public Boolean Equals(StateObject otherStateObject)
        {
            return GetType() == otherStateObject.GetType();
        }

        /// <summary>
        ///     Indicates whether this <see cref="StateObject" /> is at the top of the <see cref="StateObject" />s hierarchy.
        /// </summary>
        /// <returns>
        ///     <see langword="true" /> if the <see cref="StateObject" /> is not inside a state machine, <see langword="false" />
        ///     otherwise.
        /// </returns>
        public Boolean IsRoot => (State == null);

        /// <summary>
        ///     Adds a <see cref="Transition" /> to <paramref name="to" />.
        ///     In order to change to <paramref name="to" />, all <paramref name="conditions" />
        ///     (if any) must return <see langword="true" />.
        /// </summary>
        /// <param name="to">
        ///     The next <see cref="StateObject" /> to be executed after the transition is completed.
        /// </param>
        /// <param name="conditions">
        ///     The list of conditions that must be met in order to change to a new <see cref="StateObject" />.
        /// </param>
        public void AddTransition(StateObject to, params Func<Boolean>[] conditions)
        {
            Transition __transition = new Transition(from: this, to: to, transitionAction: null, conditions: conditions);
            TryRegisterTransition(transition: __transition);
        }
        
        public void AddTransitionTo(StateObject to, params Func<Boolean>[] conditions)
        {
            AddTransition(to: to, conditions: conditions);
        }
        
        public void AddTransitionFrom(StateObject from, params Func<Boolean>[] conditions)
        {
            from.AddTransition(to: this, conditions: conditions);
        }
        
        // //override > operator
        // public static Boolean operator > (StateObject from, StateObject to)
        // {
        //     from.AddTransition(to: to);
        //     return true;
        // }
        //
        // //override < operator
        // public static Boolean operator < (StateObject from, StateObject to)
        // {
        //     to.AddTransition(to: from);
        //     return true;
        // }

        /// <summary>
        ///     Adds a <see cref="Transition" /> to <paramref name="to" />.
        ///     In order to change to <paramref name="to" />, all <paramref name="conditions" />
        ///     (if any) must return true. Transitions added first have higher priority. <paramref name="transitionAction" />
        ///     is executed after the transition occurs.
        /// </summary>
        /// <param name="to">
        ///     The next <see cref="StateObject" /> to be executed after the transition is completed.
        /// </param>
        /// <param name="transitionAction">
        ///     Function to be executed if the transition occurs. It is executed after current <see cref="StateObject" />
        ///     is exited and before the new one is entered.
        /// </param>
        /// <param name="conditions">
        ///     The list of conditions that must be met in order to change to a new <see ref="StateObject" />.
        /// </param>
        public void AddTransition(StateObject to, Action transitionAction,
            params Func<Boolean>[] conditions)
        {
            Transition __transition = new Transition(from: this, to: to, transitionAction: transitionAction, conditions: conditions);
            TryRegisterTransition(transition: __transition);
        }

        /// <summary>
        ///     Tries to store a transition. Throws a <paramref name="NoCommonParentStateMachineException" />
        ///     exception if the <see cref="StateObject" />s of the transition are not inside a common state machine
        ///     of the state machine hierarchy tree.
        /// </summary>
        /// <param name="transition">
        ///     The transition.
        /// </param>
        /// <exception cref="NoCommonParentStateMachineException">
        /// </exception>
        private void TryRegisterTransition(Transition transition)
        {
            if (!HaveCommonStateMachineAncestor(stateObject1: transition.From, stateObject2: transition.To))
                throw new NoCommonParentStateMachineException(
                    message: typeof(StateObject) + "s " + transition.From.GetType() +
                             " and " + transition.To.GetType() + " don't have " +
                             " a common parent " + typeof(State) + " state machine."
                );

            transitions.Add(item: transition);
        }

        /// <summary>
        ///     Tries to store an event transition. Throws a <see cref="NoCommonParentStateMachineException" />
        ///     exception if the <see cref="StateObject" />s of the transition are not inside a common state machine
        ///     of the state machine hierarchy tree.
        /// </summary>
        /// <param name="eventTransition">
        ///     The event transition.
        /// </param>
        /// <exception cref="NoCommonParentStateMachineException">
        /// </exception>
        private void TryRegisterEventTransition(EventTransitionBase eventTransition)
        {
            TryRegisterTransition(transition: eventTransition);
            eventTransitions.Add(item: eventTransition);
        }

        /// <summary>
        ///     Checks whether <paramref name="stateObject1" /> and <paramref name="stateObject2" /> have
        ///     a common state machine ancestor in the state machine hierarchy tree.
        /// </summary>
        /// <param name="stateObject1"></param>
        /// <param name="stateObject2"></param>
        /// <returns>
        ///     <see langword="true" /> if <paramref name="stateObject1" /> and <paramref name="stateObject2" /> have
        ///     a common state machine ancestor in the state machine hierarchy tree, <see langword="false" /> otherwise.
        /// </returns>
        private protected Boolean HaveCommonStateMachineAncestor(StateObject stateObject1, StateObject stateObject2)
        {
            Boolean __haveCommonStateMachineAncestor = false;
            State __stateMachine1 = stateObject1.State;
            State __stateMachine2 = stateObject2.State;

            if (__stateMachine1 != null && __stateMachine2 != null)
            {
                __haveCommonStateMachineAncestor = FindLowestCommonStateMachine(sm1: __stateMachine1, sm2: __stateMachine2) != null;
            }

            return __haveCommonStateMachineAncestor;
        }


        /// <summary>
        ///     Lowest Common Ancestor tree algorithm. Finds the lowest common <see cref="State" /> ancestor
        ///     of <paramref name="sm1" /> and <paramref name="sm2" /> in the <see cref="State" /> hierarchy tree.
        /// </summary>
        /// <param name="sm1">
        ///     A <see cref="State" /> object.
        /// </param>
        /// <param name="sm2">
        ///     A <see cref="State" /> object.
        /// </param>
        /// <returns>
        ///     The lowest common <see cref="State" /> ancestor or <see langword="null" /> if <paramref name="sm1" />
        ///     and <paramref name="sm2" />
        ///     don't have a common <see cref="State" /> ancestor.
        /// </returns>
        private protected State FindLowestCommonStateMachine(State sm1, State sm2)
        {
            LinkedListNode<State> __currentAncestor1 = sm1.PathFromRoot.First;
            LinkedListNode<State> __currentAncestor2 = sm2.PathFromRoot.First;
            State __lowestCommonState = null;

            while (__currentAncestor1 != null && __currentAncestor2 != null &&
                   __currentAncestor1.Value.Equals(otherStateObject: __currentAncestor2.Value))
            {
                __lowestCommonState = __currentAncestor1.Value;
                __currentAncestor1 = __currentAncestor1.Next;
                __currentAncestor2 = __currentAncestor2.Next;
            }

            return __lowestCommonState;
        }

        /// <summary>
        ///     Returns the name of the current <see cref="StateObject" /> including the names of all the
        ///     parent <see cref="StateObject" />s starting from the root of the hierarchy.
        /// </summary>
        /// <returns>
        ///     The full name of the <see cref="StateObject" />.
        /// </returns>
        public abstract String CurrentStateName { get; }

        /// <summary>
        ///     Consumes the events listened by the <see cref="EventTransition" />s.
        /// </summary>
        internal abstract void ConsumeTransitionsEvents();

        /// <summary>
        ///     Tries to find a transition that can be performed in this update call.
        /// </summary>
        /// <returns>
        ///     An available <see cref="Transition" />, if there is any, <see langword="null" /> otherwise.
        /// </returns>
        internal abstract Transition AvailableTransition { get; }

        /// <summary>
        ///     Executes the logic needed to implement the hierarchical finite state machine pattern.
        ///     It should be called every frame.
        /// </summary>
        internal abstract void UpdateInternal();

        /// <summary>
        ///     Executes the logic needed to implement the hierarchical finite state machine pattern
        ///     as well as the custom logic defined in concrete <see cref="StateObject" />s. It should be called from
        ///     MonoBehaviour.Update function.
        /// </summary>
        public abstract void Update();

        /// <summary>
        ///     Executes the custom logic defined in concrete <see cref="StateObject" />s that should be executed with
        ///     frame-rate independece. It should be called from MonoBehaviour.FixedUpdate function.
        /// </summary>
        public abstract void FixedUpdate();

        /// <summary>
        ///     Executes the custom logic defined in concrete <see cref="StateObject" />s that needs be executed after
        ///     the regular Update cycle. It should be called from MonoBehaviour.LateUpdate function.
        /// </summary>
        public abstract void LateUpdate();

        /// <summary>
        ///     Executes the logic needed to implement the hierarchical finite state machine pattern
        ///     as well as the custom logic defined in concrete <see cref="StateObject" /> classes. It is executed when a
        ///     transition is performed; it is executed when entering a new <see cref="StateObject" />.
        /// </summary>
        internal abstract void Enter();

        /// <summary>
        ///     Executes the logic needed to implement the hierarchical finite state machine pattern
        ///     as well as the custom logic defined in concrete <see cref="StateObject" />s. It is executed when a
        ///     transition is performed; it is executed when leaving the current <see cref="StateObject" />.
        /// </summary>
        internal abstract void Exit();

        /// <summary>
        ///     Custom logic defined in concrete <see cref="StateObject" /> classes that gets executed every update cycle.
        /// </summary>
        protected virtual void OnUpdate() { }

        /// <summary>
        ///     Custom logic defined in concrete <see cref="StateObject" /> <see cref="StateObject" />s that gets executed every
        ///     fixed update cycle.
        /// </summary>
        protected virtual void OnFixedUpdate() { }

        /// <summary>
        ///     Custom logic defined in concrete <see cref="StateObject" /> classes that gets executed every late update cycle.
        /// </summary>
        protected virtual void OnLateUpdate() { }

        /// <summary>
        ///     Custom logic defined in concrete <see cref="StateObject" /> classes that gets executed when a new
        ///     <see cref="StateObject" /> is entered.
        /// </summary>
        protected virtual void OnEnter() { }

        /// <summary>
        ///     Custom logic defined in concrete <see cref="StateObject" />s that gets executed when the current
        ///     <see cref="StateObject" /> is exited.
        /// </summary>
        protected virtual void OnExit() { }

        #region Add Event Transition Methods

        /// <summary>
        ///     Adds an <see cref="EventTransition" /> to <paramref name="to" />.
        ///     In order to change to <paramref name="to" />, the event must have been fired and
        ///     all <paramref name="conditions" /> (if any) must return <see langword="true" />. Transitions added first have
        ///     higher priority.
        ///     All <see cref="EventTransition" />s are processed together with polling transitions at the same time in the
        ///     execution flow.
        ///     Set <paramref name="processInstantly" /> parameter to <see langword="true" /> if you want to process transition
        ///     events as soon as
        ///     the event is listened. Events are only listened if the origin <see cref="StateObject" /> is active.
        ///     <paramref name="transitionAction" />
        ///     is executed after the transition occurs.
        /// </summary>
        /// <param name="to">
        ///     The next <see cref="StateObject" /> to be executed after the transition is completed.
        /// </param>
        /// <param name="transitionAction">
        ///     Function to be executed if the transition occurs. It is executed after current <see cref="StateObject" />
        ///     is exited and before the new one is entered.
        /// </param>
        /// <param name="processInstantly">
        ///     Indicates whether <see cref="Transition" />s should be evaluated as soon as the event is listened or not.
        /// </param>
        /// <param name="conditions">
        ///     The list of conditions that must be met in order to change to a new <see cref="StateObject" />.
        /// </param>
        /// <returns>
        ///     An event listener function that must be subscribed to an event.
        /// </returns>
        public Action AddEventTransitionTo(StateObject to, Action transitionAction = null, Boolean processInstantly = false, params Func<Boolean>[] conditions)
        {
            EventTransition __transition = new EventTransition(from: this, to: to, transitionAction: transitionAction, processInstantly: processInstantly, conditions: conditions);
            TryRegisterEventTransition(eventTransition: __transition);
            return __transition.ListenEvent;
        }
        
        public Action AddEventTransitionFrom(StateObject from, Action transitionAction = null, Boolean processInstantly = false, params Func<Boolean>[] conditions)
        {
            EventTransition __transition = new EventTransition(from: from, to: this, transitionAction: transitionAction, processInstantly: processInstantly, conditions: conditions);
            TryRegisterEventTransition(eventTransition: __transition);
            return __transition.ListenEvent;
        }

        /// <inheritdoc cref="AddEventTransition(StateObject, Func{bool}[])" />
        public Action<T> AddEventTransition<T>(StateObject to, params Func<T, Boolean>[] conditions)
        {
            EventTransition<T> __transition = new EventTransition<T>(from: this, to: to, transitionAction: null, processInstantly: false, conditions: conditions);
            TryRegisterEventTransition(eventTransition: __transition);
            return __transition.ListenEvent;
        }

        /// <inheritdoc cref="AddEventTransition(StateObject, bool, Func{bool}[])" />
        public Action<T> AddEventTransition<T>(StateObject to, Boolean processInstantly, params Func<T, Boolean>[] conditions)
        {
            EventTransition<T> __transition = new EventTransition<T>(from: this, to: to, transitionAction: null, processInstantly: processInstantly, conditions: conditions);
            TryRegisterEventTransition(eventTransition: __transition);
            return __transition.ListenEvent;
        }

        /// <inheritdoc cref="AddEventTransition(StateObject, Action, Func{bool}[])" />
        public Action<T> AddEventTransition<T>(StateObject to, Action<T> transitionAction, params Func<T, Boolean>[] conditions)
        {
            EventTransition<T> __transition = new EventTransition<T>(from: this, to: to, transitionAction: transitionAction, processInstantly: false, conditions: conditions);
            TryRegisterEventTransition(eventTransition: __transition);
            return __transition.ListenEvent;
        }

        /// <inheritdoc cref="AddEventTransition(StateObject, Action, bool, Func{bool}[])" />
        public Action<T> AddEventTransition<T>(StateObject to, Action<T> transitionAction, Boolean processInstantly, params Func<T, Boolean>[] conditions)
        {
            EventTransition<T> __transition = new EventTransition<T>(from: this, to: to, transitionAction: transitionAction, processInstantly: processInstantly, conditions: conditions);
            TryRegisterEventTransition(eventTransition: __transition);
            return __transition.ListenEvent;
        }

        /// <inheritdoc cref="AddEventTransition(StateObject, Func{bool}[])" />
        public Action<T1, T2> AddEventTransition<T1, T2>(StateObject to, params Func<T1, T2, Boolean>[] conditions)
        {
            EventTransition<T1, T2> __transition = new EventTransition<T1, T2>(from: this, to: to, transitionAction: null, processInstantly: false, conditions: conditions);
            TryRegisterEventTransition(eventTransition: __transition);
            return __transition.ListenEvent;
        }

        /// <inheritdoc cref="AddEventTransition(StateObject, bool, Func{bool}[])" />
        public Action<T1, T2> AddEventTransition<T1, T2>(StateObject to, Boolean processInstantly, params Func<T1, T2, Boolean>[] conditions)
        {
            EventTransition<T1, T2> __transition = new EventTransition<T1, T2>(from: this, to: to, transitionAction: null, processInstantly: processInstantly, conditions: conditions);
            TryRegisterEventTransition(eventTransition: __transition);
            return __transition.ListenEvent;
        }

        /// <inheritdoc cref="AddEventTransition(StateObject, Action, Func{bool}[])" />
        public Action<T1, T2> AddEventTransition<T1, T2>(StateObject to, Action<T1, T2> transitionAction, params Func<T1, T2, Boolean>[] conditions)
        {
            EventTransition<T1, T2> __transition = new EventTransition<T1, T2>(from: this, to: to, transitionAction: transitionAction, processInstantly: false, conditions: conditions);
            TryRegisterEventTransition(eventTransition: __transition);
            return __transition.ListenEvent;
        }

        /// <inheritdoc cref="AddEventTransition(StateObject, Action, bool, Func{bool}[])" />
        public Action<T1, T2> AddEventTransition<T1, T2>(StateObject to, Action<T1, T2> transitionAction, Boolean processInstantly = false, params Func<T1, T2, Boolean>[] conditions)
        {
            EventTransition<T1, T2> __transition = new EventTransition<T1, T2>(from: this, to: to, transitionAction: transitionAction, processInstantly: processInstantly, conditions: conditions);
            TryRegisterEventTransition(eventTransition: __transition);
            return __transition.ListenEvent;
        }

        /// <inheritdoc cref="AddEventTransition(StateObject, Func{bool}[])" />
        public Action<T1, T2, T3> AddEventTransition<T1, T2, T3>(StateObject to, params Func<T1, T2, T3, Boolean>[] conditions)
        {
            EventTransition<T1, T2, T3> __transition = new EventTransition<T1, T2, T3>(from: this, to: to, transitionAction: null, processInstantly: false, conditions: conditions);
            TryRegisterEventTransition(eventTransition: __transition);
            return __transition.ListenEvent;
        }

        /// <inheritdoc cref="AddEventTransition(StateObject, bool, Func{bool}[])" />
        public Action<T1, T2, T3> AddEventTransition<T1, T2, T3>(StateObject to, Boolean processInstantly, params Func<T1, T2, T3, Boolean>[] conditions)
        {
            EventTransition<T1, T2, T3> __transition = new EventTransition<T1, T2, T3>(from: this, to: to, transitionAction: null, processInstantly: processInstantly, conditions: conditions);
            TryRegisterEventTransition(eventTransition: __transition);
            return __transition.ListenEvent;
        }

        /// <inheritdoc cref="AddEventTransition(StateObject, Action, Func{bool}[])" />
        public Action<T1, T2, T3> AddEventTransition<T1, T2, T3>(StateObject to, Action<T1, T2, T3> transitionAction, params Func<T1, T2, T3, Boolean>[] conditions)
        {
            EventTransition<T1, T2, T3> __transition = new EventTransition<T1, T2, T3>(from: this, to: to, transitionAction: transitionAction, processInstantly: false, conditions: conditions);
            TryRegisterEventTransition(eventTransition: __transition);
            return __transition.ListenEvent;
        }

        /// <inheritdoc cref="AddEventTransition(StateObject, Action, bool, Func{bool}[])" />
        public Action<T1, T2, T3> AddEventTransition<T1, T2, T3>(StateObject to, Action<T1, T2, T3> transitionAction, Boolean processInstantly = false, params Func<T1, T2, T3, Boolean>[] conditions)
        {
            EventTransition<T1, T2, T3> __transition = new EventTransition<T1, T2, T3>(from: this, to: to, transitionAction: transitionAction, processInstantly: processInstantly, conditions: conditions);
            TryRegisterEventTransition(eventTransition: __transition);
            return __transition.ListenEvent;
        }

        #endregion
    }
}