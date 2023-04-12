using System;
using System.Collections.Generic;
using UnityEngine;

namespace HFSM
{
    /// <summary>
    ///     Hierarchical finite state machine.
    /// </summary>
    [Serializable]
    public abstract class State : StateObject
    {
        private readonly List<EventTransitionBase> _anyEventTransitions;
        private readonly StateLeaf _anyStateLeaf;
        private readonly List<Transition> _anyTransitions;
        private Boolean _changedState;
        private Boolean _initialized;

        private LinkedList<State> _pathFromRoot;

        /// <summary>
        ///     Class constructor. Creates a <see cref="State" /> and initializes it. Throws
        ///     <see cref="StatelessStateMachineException" /> if no <see cref="StateObject" /> is passed as argument.
        /// </summary>
        /// <param name="childStates">
        ///     List of state objects inside this <see cref="State" />. <see cref="StateObject" />s inherit from
        ///     <see cref="State" />State
        ///     or <see cref="StateLeaf" /> classes.
        /// </param>
        /// <exception cref="StatelessStateMachineException">
        ///     Thrown when no <see cref="StateObject" /> is passed as argument.
        /// </exception>
        public State(params StateObject[] childStates)
        {
            if (childStates == null)
            {
                throw new ArgumentNullException(paramName: nameof(childStates));
            }
            
            if (childStates.Length == 0)
            {
                throw new StatelessStateMachineException(
                    message: "A State Machine must have at least one state object." +
                             " State machine of type '" + GetType() + "' does not have any state objects."
                );
            }

            _anyTransitions = new List<Transition>();
            _anyEventTransitions = new List<EventTransitionBase>();
            _anyStateLeaf = new StateLeaf.Any();
            _anyStateLeaf.State = this;
            _initialized = false;
            _changedState = false;

            DefaultStateObject = childStates[0];
            
            foreach (StateObject __childState in childStates)
            {
                __childState.State = this;
            }
        }

        public StateObject DefaultStateObject { get; }
        public StateObject CurrentStateObject { get; private set; }

        /// <summary>
        ///     A linked list of <see cref="State" /> starting from the root <see cref="State" />.
        /// </summary>
        /// <returns>
        /// </returns>
        internal LinkedList<State> PathFromRoot
        {
            get
            {
                if (_pathFromRoot == null)
                {
                    PathFromRoot = IsRoot ? new LinkedList<State>() : new LinkedList<State>(collection: State.PathFromRoot);
                    
                    _pathFromRoot.AddLast(value: this);
                }

                return _pathFromRoot;
            }

            private set => _pathFromRoot = value;
        }

        /// <summary>
        ///     Initializes the root <see cref="State" /> by calling <see cref="Enter" /> method.
        /// </summary>
        /// <seealso cref="Enter" />
        public void Init()
        {
            if (IsRoot)
            {
                Enter();
                _initialized = true;
            }
        }

        /// <summary>
        ///     Checks whether <see cref="Init" /> has not been on called on the root <see cref="State" /> object
        /// </summary>
        /// <exception cref="RootStateMachineNotInitializedException">
        ///     Thrown if <see cref="Init" /> has not been on called on the root <see cref="State" /> object.
        /// </exception>
        private void CheckInitialization()
        {
            if (IsRoot && !_initialized)
                throw new RootStateMachineNotInitializedException(message: "Root State Machine has not been initialized." +
                                                                           " Call rootStateMachine.Init() to initialize it");
        }

        /// <summary>
        ///     Tries to find an available <see cref="Transition" /> and then use it to change to a new
        ///     <see cref="StateObject" />.
        /// </summary>
        /// <returns>
        ///     <see langword="true" /> if the current <see cref="StateObject" /> was changed, <see langword="false" /> otherwise.
        /// </returns>
        private Boolean TryChangeState()
        {
            Boolean __changedState = false;
            Transition __availableTransition = null;

            // Check any state object's transitions
            foreach (Transition __anyTransition in _anyTransitions)
            {
                if (__anyTransition.AllConditionsMet())
                {
                    __availableTransition = __anyTransition;
                    break;
                }
            }

            // Check current state object's transitions
            __availableTransition ??= CurrentStateObject.AvailableTransition;

            foreach (EventTransitionBase __anyEventTransition in _anyEventTransitions)
            {
                __anyEventTransition.ConsumeEvent();
            }
            ConsumeTransitionsEvents();

            if (__availableTransition != null)
            {
                ChangeState(availableTransition: __availableTransition);
                __changedState = true;
            }
            //CurrentStateObject.ConsumeTransitionsEvents();
            //previousStateObject.ConsumeTransitionsEvents(); 

            return __changedState;
        }

        /// <summary>
        ///     Consumes all the events listened by <see cref="EventTransition" />s that have
        ///     this <see cref="State" /> as their <see cref="Transition.From" /> and
        ///     all the events listened by <see cref="EventTransition" />s whose <see cref="Transition.From" /> is any
        ///     of the <see cref="StateObject" />s nested in this <see cref="State" />.
        /// </summary>
        /// <seealso cref="AddAnyEventTransition(StateObject, Func{bool}[])" />
        internal sealed override void ConsumeTransitionsEvents()
        {
            foreach (EventTransitionBase __anyEventTransition in _anyEventTransitions)
            {
                __anyEventTransition.ConsumeEvent();
            }

            foreach (EventTransitionBase __eventTransition in eventTransitions)
            {
                __eventTransition.ConsumeEvent();
            }

            CurrentStateObject.ConsumeTransitionsEvents();
        }

        /// <summary>
        ///     Finds the highest priority available <see cref="Transition" /> in this <see cref="State" />.
        ///     <see cref="Transition" />s added first have higher priority. If no available <see cref="Transition" /> is found
        ///     it returns the available <see cref="Transition" />s from its current <see cref="StateObject" />.
        /// </summary>
        /// <returns>
        ///     The highest priority available <see cref="Transition" /> if any, <see langword="null" /> otherwise.
        /// </returns>
        internal sealed override Transition AvailableTransition
        {
            get
            {
                Transition __availableTransition = null;

                // Check this state machine's normal and event transitions
                foreach (Transition __transition in transitions)
                {
                    if (__transition.AllConditionsMet())
                    {
                        __availableTransition = __transition;
                        break;
                    }
                }

                // Check any state object's transitions
                foreach (Transition __anyTransition in _anyTransitions)
                {
                    if (__anyTransition.AllConditionsMet())
                    {
                        __availableTransition = __anyTransition;
                        break;
                    }
                }

                // Check current state object's transitions
                // ReSharper disable once RedundantAssignment
                return __availableTransition ??= CurrentStateObject.AvailableTransition;
            }
        }

        /// <summary>
        ///     Sets <paramref name="availableTransition" />'s <see cref="Transition.To" /> as the current
        ///     <see cref="StateObject" /> of this <see cref="State" />. <see cref="StateObject.Exit" />,
        ///     <see cref="Transition.TransitionAction" /> (if specified) and <see cref="StateObject.Enter" /> are executed
        ///     in that order.
        /// </summary>
        /// <param name="availableTransition"></param>
        private void ChangeState(Transition availableTransition)
        {
            StateObject __originStateObject = availableTransition.From;
            StateObject __targetStateObject = availableTransition.To;

            State __origin = __originStateObject.State;
            State __target = __targetStateObject.State;
            
            //Debug.Log($"Changing state from {__origin} to {__target}");

            State __lowestCommonState = FindLowestCommonStateMachine(sm1: __origin, sm2: __target);

            __lowestCommonState.CurrentStateObject.Exit();

            __targetStateObject.State.CurrentStateObject = __targetStateObject;
            State __currentState = __targetStateObject.State;
            while (__currentState != null && !__currentState.Equals(otherStateObject: __lowestCommonState))
            {
                State __parentState = __currentState.State;
                __parentState.CurrentStateObject = __currentState;
                __currentState = __parentState;
            }

            availableTransition.InvokeTransitionAction();
            __lowestCommonState.CurrentStateObject.Enter();
        }

        /// <summary>
        ///     Evaluates the conditions of <paramref name="eventTransition" /> instantly without waiting
        ///     for the next update cycle and performs the transition if all the conditions are met.
        /// </summary>
        /// <param name="eventTransition">
        ///     The transtion to be performed.
        /// </param>
        internal void ProcessInstantEvent(EventTransitionBase eventTransition)
        {
            StateObject __originStateObject = eventTransition.From;
            if (__originStateObject.IsActive || (__originStateObject.GetType() == typeof(StateLeaf.Any) &&
                                                 __originStateObject.State.IsActive &&
                                                 eventTransition.AllConditionsMet()))
            {
                ChangeState(availableTransition: eventTransition);
            }
        }

        /// <summary>
        ///     Executes the code needed to implement the state machine beahviour of a
        ///     hierarchical finite state machine pattern as well as the update cycle code defined
        ///     in the extended classes.
        /// </summary>
        public sealed override void Update()
        {
            CheckInitialization();
            _changedState = TryChangeState();
            
            if (_changedState) return;
            
            OnUpdate();
            CurrentStateObject.UpdateInternal();
        }

        /// <summary>
        ///     Executes the code needed to implement the state machine behaviour of a
        ///     hiearchical finite state machine pattern.
        /// </summary>
        internal sealed override void UpdateInternal()
        {
            OnUpdate();
            CurrentStateObject.UpdateInternal();
        }

        /// <summary>
        ///     Executes the code needed to implement the state machine beahviour of a
        ///     hierarchical finite state machine pattern as well as the fixed update cycle code defined
        ///     in the extended classes.
        /// </summary>
        public sealed override void FixedUpdate()
        {
            CheckInitialization();
            OnFixedUpdate();
            CurrentStateObject.FixedUpdate();
        }
        
        public sealed override void LateFixedUpdate()
        {
            CheckInitialization();
            OnLateFixedUpdate();
            CurrentStateObject.LateFixedUpdate();
        }

        /// <summary>
        ///     Executes the code needed to implement the state machine beahviour of a
        ///     hierarchical finite state machine pattern as well as the late update cycle code defined
        ///     in the extended classes.
        /// </summary>
        public sealed override void LateUpdate()
        {
            CheckInitialization();
            if (!_changedState)
            {
                OnLateUpdate();
                CurrentStateObject.LateUpdate();
            }
        }

        /// <summary>
        ///     Executes the code needed to implement the state machine beahviour of a
        ///     hierarchical finite state machine pattern as well as the logic defined in the extended classes.
        ///     This function is called the first update cycle after this <see cref="State" /> has become active.
        ///     The hierarchical execution of <see cref="Enter" /> is performed in a top-down fashion.
        /// </summary>
        internal sealed override void Enter()
        {
            IsActive = true;
            CurrentStateObject ??= DefaultStateObject;

            OnEnter();
            CurrentStateObject.Enter();
        }

        /// <summary>
        ///     Executes the code needed to implement the state machine beahviour of a
        ///     hierarchical finite state machine pattern as well as the logic defined in the extended classes.
        ///     This function is called the last update cycle before this <see cref="State" /> becomes inactive.
        ///     The hierarchical execution of <see cref="Exit" /> is performed in a bottom-up fashion.
        /// </summary>
        internal sealed override void Exit()
        {
            CurrentStateObject.Exit();
            OnExit();
            
            IsActive = false;
            CurrentStateObject = null;
        }

        /// <summary>
        ///     Returns the hierarchy of active <see cref="StateObject" />s starting from the root state machine.
        /// </summary>
        /// <returns>
        ///     The hierarchy of active <see cref="StateObject" />s converted to string.
        /// </returns>
        public sealed override String CurrentStateName
        {
            get
            {
                String __name = GetType() + ".";
                if (CurrentStateObject == null)
                {
                    __name += "None";
                }
                else
                {
                    __name += CurrentStateObject.CurrentStateName;
                }
                return __name;   
            }
        }

        #region AddAnyTransition methods

        /// <summary>
        ///     Adds a <see cref="Transition" /> from any <see cref="StateObject" /> inside this <see cref="State" />
        ///     to <paramref name="targetStateObject" />. In order to change to <paramref name="targetStateObject" />,
        ///     all <paramref name="conditions" /> (if any) must return <see langword="true" />. Transitions added first
        ///     have higher priority.
        /// </summary>
        /// <param name="targetStateObject">
        ///     The next <see cref="StateObject" /> to be executed after the transition is completed.
        /// </param>
        /// <param name="conditions">
        ///     The list of conditions that must be met in order to change to <paramref name="targetStateObject" />.
        /// </param>
        public void AddAnyTransition(StateObject targetStateObject, params Func<Boolean>[] conditions)
        {
            Transition __transition = new Transition(from: _anyStateLeaf, to: targetStateObject, transitionAction: null, conditions: conditions);
            TryRegisterAnyTransition(anyTransition: __transition);
        }

        /// <summary>
        ///     Adds a <see cref="Transition" /> from any <see cref="StateObject" /> inside this <see cref="State" />
        ///     to <paramref name="targetStateObject" />. In order to change to <paramref name="targetStateObject" />,
        ///     all <paramref name="conditions" /> (if any) must return <see langword="true" />. Transitions added first
        ///     have higher priority.
        /// </summary>
        /// <param name="targetStateObject">
        ///     The next <see cref="StateObject" /> to be executed after the transition is completed.
        /// </param>
        /// <param name="transitionAction">
        ///     Function to be executed if the transition occurs. It is executed after current <see cref="StateObject" />
        ///     is exited and before the new one is entered.
        /// </param>
        /// <param name="conditions">
        ///     The list of conditions that must be met in order to change to <paramref name="targetStateObject" />.
        /// </param>
        public void AddAnyTransition(StateObject targetStateObject, Action transitionAction,
            params Func<Boolean>[] conditions)
        {
            Transition __transition = new Transition(from: _anyStateLeaf, to: targetStateObject, transitionAction: transitionAction, conditions: conditions);
            TryRegisterAnyTransition(anyTransition: __transition);
        }

        /// <summary>
        ///     Adds an <see cref="EventTransition" /> to <paramref name="targetStateObject" />.
        ///     In order to change to <paramref name="targetStateObject" />, the event must have been fired and
        ///     all <paramref name="conditions" /> (if any) must return <see langword="true" />. Transitions added first have
        ///     higher priority.
        ///     All <see cref="EventTransition" />s are processed together with <see cref="Transition" />s (polling transitions)
        ///     at the same time in the execution flow.
        ///     Set <paramref name="processInstantly" /> parameter to <see langword="true" /> if you want to process transition
        ///     events as soon as
        ///     the event is listened. Events are only listened if the origin <see cref="StateObject" /> is active.
        /// </summary>
        /// <param name="targetStateObject">
        ///     The next <see cref="StateObject" /> to be executed after the transition is completed.
        /// </param>
        /// <param name="conditions">
        ///     The list of conditions that must be met in order to change to a new <see cref="StateObject" />.
        /// </param>
        /// <returns>
        ///     An event listener function that must be subscribed to an event.
        /// </returns>
        public Action AddAnyEventTransition(StateObject targetStateObject, params Func<Boolean>[] conditions)
        {
            EventTransition __transition = new EventTransition(from: _anyStateLeaf, to: targetStateObject, transitionAction: null, processInstantly: false, conditions: conditions);

            TryRegisterAnyTransition(anyTransition: __transition);
            _anyEventTransitions.Add(item: __transition);
            return __transition.ListenEvent;
        }

        /// <summary>
        ///     Adds an <see cref="EventTransition" /> to <paramref name="targetStateObject" />.
        ///     In order to change to <paramref name="targetStateObject" />, the event must have been fired and
        ///     all <paramref name="conditions" /> (if any) must return <see langword="true" />. Transitions added first have
        ///     higher priority.
        ///     All <see cref="EventTransition" />s are processed together with <see cref="Transition" />s (polling transitions)
        ///     at the same time in the execution flow.
        ///     Set <paramref name="processInstantly" /> parameter to <see langword="true" /> if you want to process transition
        ///     events as soon as
        ///     the event is listened. Events are only listened if the origin <see cref="StateObject" /> is active.
        /// </summary>
        /// <param name="targetStateObject">
        ///     The next <see cref="StateObject" /> to be executed after the transition is completed.
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
        public Action AddAnyEventTransition(StateObject targetStateObject, Boolean processInstantly, params Func<Boolean>[] conditions)
        {
            EventTransition __transition = new EventTransition(from: _anyStateLeaf, to: targetStateObject, transitionAction: null, processInstantly: processInstantly, conditions: conditions);

            TryRegisterAnyTransition(anyTransition: __transition);
            _anyEventTransitions.Add(item: __transition);
            return __transition.ListenEvent;
        }

        /// <summary>
        ///     Adds an <see cref="EventTransition" /> to <paramref name="targetStateObject" />.
        ///     In order to change to <paramref name="targetStateObject" />, the event must have been fired and
        ///     all <paramref name="conditions" /> (if any) must return <see langword="true" />. Transitions added first have
        ///     higher priority.
        ///     All <see cref="EventTransition" />s are processed together with <see cref="Transition" />s (polling transitions)
        ///     at the same time in the execution flow.
        ///     Set <paramref name="processInstantly" /> parameter to <see langword="true" /> if you want to process transition
        ///     events as soon as
        ///     the event is listened. Events are only listened if the origin <see cref="StateObject" /> is active.
        /// </summary>
        /// <param name="targetStateObject">
        ///     The next <see cref="StateObject" /> to be executed after the transition is completed.
        /// </param>
        /// <param name="transitionAction">
        ///     Function to be executed if the transition occurs. It is executed after current <see cref="StateObject" />
        ///     is exited and before the new one is entered.
        /// </param>
        /// <param name="conditions">
        ///     The list of conditions that must be met in order to change to a new <see cref="StateObject" />.
        /// </param>
        /// <returns>
        ///     An event listener function that must be subscribed to an event.
        /// </returns>
        public Action AddAnyEventTransition(StateObject targetStateObject, Action transitionAction, params Func<Boolean>[] conditions)
        {
            EventTransition __transition = new EventTransition(from: _anyStateLeaf, to: targetStateObject, transitionAction: transitionAction, processInstantly: false, conditions: conditions);

            TryRegisterAnyTransition(anyTransition: __transition);
            _anyEventTransitions.Add(item: __transition);
            return __transition.ListenEvent;
        }

        /// <summary>
        ///     Adds an <see cref="EventTransition" /> to <paramref name="targetStateObject" />.
        ///     In order to change to <paramref name="targetStateObject" />, the event must have been fired and
        ///     all <paramref name="conditions" /> (if any) must return <see langword="true" />. Transitions added first have
        ///     higher priority.
        ///     All <see cref="EventTransition" />s are processed together with <see cref="Transition" />s (polling transitions)
        ///     at the same time in the execution flow.
        ///     Set <paramref name="processInstantly" /> parameter to <see langword="true" /> if you want to process transition
        ///     events as soon as
        ///     the event is listened. Events are only listened if the origin <see cref="StateObject" /> is active.
        /// </summary>
        /// <param name="targetStateObject">
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
        public Action AddAnyEventTransition(StateObject targetStateObject, Action transitionAction,
            Boolean processInstantly, params Func<Boolean>[] conditions)
        {
            EventTransition __transition = new EventTransition(from: _anyStateLeaf, to: targetStateObject, transitionAction: transitionAction, processInstantly: processInstantly, conditions: conditions);

            TryRegisterAnyTransition(anyTransition: __transition);
            _anyEventTransitions.Add(item: __transition);
            return __transition.ListenEvent;
        }

        /// <inheritdoc cref="AddAnyEventTransition(StateObject, Func{bool}[])" />
        public Action<T> AddAnyEventTransition<T>(StateObject targetStateObject, params Func<T, Boolean>[] conditions)
        {
            EventTransition<T> __transition = new EventTransition<T>(from: _anyStateLeaf, to: targetStateObject, transitionAction: null, processInstantly: false, conditions: conditions);

            TryRegisterAnyTransition(anyTransition: __transition);
            _anyEventTransitions.Add(item: __transition);
            return __transition.ListenEvent;
        }

        /// <inheritdoc cref="AddAnyEventTransition(StateObject, bool, Func{bool}[])" />
        public Action<T> AddAnyEventTransition<T>(StateObject targetStateObject, Boolean processInstantly, params Func<T, Boolean>[] conditions)
        {
            EventTransition<T> __transition = new EventTransition<T>(from: _anyStateLeaf, to: targetStateObject, transitionAction: null, processInstantly: processInstantly, conditions: conditions);

            TryRegisterAnyTransition(anyTransition: __transition);
            _anyEventTransitions.Add(item: __transition);
            return __transition.ListenEvent;
        }

        /// <inheritdoc cref="AddAnyEventTransition(StateObject, Action, Func{bool}[])" />
        public Action<T> AddAnyEventTransition<T>(StateObject targetStateObject, Action<T> transitionAction, params Func<T, Boolean>[] conditions)
        {
            EventTransition<T> __transition = new EventTransition<T>(from: _anyStateLeaf, to: targetStateObject, transitionAction: transitionAction, processInstantly: false, conditions: conditions);
            TryRegisterAnyTransition(anyTransition: __transition);
            _anyEventTransitions.Add(item: __transition);
            return __transition.ListenEvent;
        }

        /// <inheritdoc cref="AddAnyEventTransition(StateObject, Action, bool, Func{bool}[])" />
        public Action<T> AddAnyEventTransition<T>(StateObject targetStateObject, Action<T> transitionAction, Boolean processInstantly, params Func<T, Boolean>[] conditions)
        {
            EventTransition<T> __transition = new EventTransition<T>(from: _anyStateLeaf, to: targetStateObject, transitionAction: transitionAction, processInstantly: processInstantly, conditions: conditions);
            TryRegisterAnyTransition(anyTransition: __transition);
            _anyEventTransitions.Add(item: __transition);
            return __transition.ListenEvent;
        }

        /// <inheritdoc cref="AddAnyEventTransition(StateObject, Func{bool}[])" />
        public Action<T1, T2> AddAnyEventTransition<T1, T2>(StateObject targetStateObject, params Func<T1, T2, Boolean>[] conditions)
        {
            EventTransition<T1, T2> __transition = new EventTransition<T1, T2>(from: _anyStateLeaf, to: targetStateObject, transitionAction: null, processInstantly: false, conditions: conditions);
            TryRegisterAnyTransition(anyTransition: __transition);
            _anyEventTransitions.Add(item: __transition);
            return __transition.ListenEvent;
        }

        /// <inheritdoc cref="AddAnyEventTransition(StateObject, bool, Func{bool}[])" />
        public Action<T1, T2> AddAnyEventTransition<T1, T2>(StateObject targetStateObject, Boolean processInstantly, params Func<T1, T2, Boolean>[] conditions)
        {
            EventTransition<T1, T2> __transition = new EventTransition<T1, T2>(from: _anyStateLeaf, to: targetStateObject, transitionAction: null, processInstantly: processInstantly, conditions: conditions);
            TryRegisterAnyTransition(anyTransition: __transition);
            _anyEventTransitions.Add(item: __transition);
            return __transition.ListenEvent;
        }

        /// <inheritdoc cref="AddAnyEventTransition(StateObject, Action, Func{bool}[])" />
        public Action<T1, T2> AddAnyEventTransition<T1, T2>(StateObject targetStateObject, Action<T1, T2> transitionAction, params Func<T1, T2, Boolean>[] conditions)
        {
            EventTransition<T1, T2> __transition = new EventTransition<T1, T2>(from: _anyStateLeaf, to: targetStateObject, transitionAction: transitionAction, processInstantly: false, conditions: conditions);
            TryRegisterAnyTransition(anyTransition: __transition);
            _anyEventTransitions.Add(item: __transition);
            return __transition.ListenEvent;
        }

        /// <inheritdoc cref="AddAnyEventTransition(StateObject, Action, bool, Func{bool}[])" />
        public Action<T1, T2> AddAnyEventTransition<T1, T2>(StateObject targetStateObject, Action<T1, T2> transitionAction, Boolean processInstantly, params Func<T1, T2, Boolean>[] conditions)
        {
            EventTransition<T1, T2> __transition = new EventTransition<T1, T2>(from: _anyStateLeaf, to: targetStateObject, transitionAction: transitionAction, processInstantly: processInstantly, conditions: conditions);
            TryRegisterAnyTransition(anyTransition: __transition);
            _anyEventTransitions.Add(item: __transition);
            return __transition.ListenEvent;
        }

        /// <inheritdoc cref="AddAnyEventTransition(StateObject, Func{bool}[])" />
        public Action<T1, T2, T3> AddAnyEventTransition<T1, T2, T3>(StateObject targetStateObject, params Func<T1, T2, T3, Boolean>[] conditions)
        {
            EventTransition<T1, T2, T3> __transition = new EventTransition<T1, T2, T3>(from: _anyStateLeaf, to: targetStateObject, transitionAction: null, processInstantly: false, conditions: conditions);
            TryRegisterAnyTransition(anyTransition: __transition);
            _anyEventTransitions.Add(item: __transition);
            return __transition.ListenEvent;
        }

        /// <inheritdoc cref="AddAnyEventTransition(StateObject, bool, Func{bool}[])" />
        public Action<T1, T2, T3> AddAnyEventTransition<T1, T2, T3>(StateObject targetStateObject, Boolean processInstantly, params Func<T1, T2, T3, Boolean>[] conditions)
        {
            EventTransition<T1, T2, T3> __transition = new EventTransition<T1, T2, T3>(from: _anyStateLeaf, to: targetStateObject, transitionAction: null, processInstantly: processInstantly, conditions: conditions);
            TryRegisterAnyTransition(anyTransition: __transition);
            _anyEventTransitions.Add(item: __transition);
            return __transition.ListenEvent;
        }

        /// <inheritdoc cref="AddAnyEventTransition(StateObject, Action, bool, Func{bool}[])" />
        public Action<T1, T2, T3> AddAnyEventTransition<T1, T2, T3>(StateObject targetStateObject, Action<T1, T2, T3> transitionAction, params Func<T1, T2, T3, Boolean>[] conditions)
        {
            EventTransition<T1, T2, T3> __transition =new EventTransition<T1, T2, T3>(from: _anyStateLeaf, to: targetStateObject, transitionAction: transitionAction, processInstantly: false, conditions: conditions);
            TryRegisterAnyTransition(anyTransition: __transition);
            _anyEventTransitions.Add(item: __transition);
            return __transition.ListenEvent;
        }

        /// <inheritdoc cref="AddAnyEventTransition(StateObject, Action, bool, Func{bool}[])" />
        public Action<T1, T2, T3> AddAnyEventTransition<T1, T2, T3>(StateObject targetStateObject, Action<T1, T2, T3> transitionAction, Boolean processInstantly, params Func<T1, T2, T3, Boolean>[] conditions)
        {
            EventTransition<T1, T2, T3> __transition = new EventTransition<T1, T2, T3>(from: _anyStateLeaf, to: targetStateObject, transitionAction: transitionAction, processInstantly: processInstantly, conditions: conditions);
            TryRegisterAnyTransition(anyTransition: __transition);
            _anyEventTransitions.Add(item: __transition);
            return __transition.ListenEvent;
        }

        /// <summary>
        ///     Tries to store a <see cref="Transition" /> from any <see cref="StateObject" />. It will throw
        ///     <see cref="NoCommonParentStateMachineException" /> if the <see cref="StateObject" />s inside
        ///     this <see cref="State" /> and <see cref="Transition.To" /> don't have
        ///     a common <see cref="State" /> ancestor.
        /// </summary>
        /// <param name="anyTransition">
        ///     The <see cref="Transition" /> to be registered.
        /// </param>
        private void TryRegisterAnyTransition(Transition anyTransition)
        {
            if (!HaveCommonStateMachineAncestor(stateObject1: anyTransition.From, stateObject2: anyTransition.To))
            {
                throw new NoCommonParentStateMachineException(
                    message: "States inside " + typeof(State) + " of type " + GetType() +
                             " and state object of " +
                             "type " + anyTransition.To.GetType() + " don't have a common " +
                             "parent state machine."
                );
            }

            _anyTransitions.Add(item: anyTransition);
        }

        #endregion
    }
}