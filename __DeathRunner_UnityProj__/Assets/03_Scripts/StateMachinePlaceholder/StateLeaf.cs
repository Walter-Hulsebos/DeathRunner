using System;

namespace HFSM
{
    /// <summary>
    ///     State behaviour of a hierarchical finite state machine pattern.
    /// </summary>
    [Serializable]
    public abstract class StateLeaf : StateObject
    {
        public event Action OnEnter;
        public event Action OnExit;
        
        /// <summary>
        ///     Consumes all the events listened by <see cref="EventTransition" />s that have
        ///     this <see cref="StateLeaf" /> as their <see cref="Transition.From" />.
        /// </summary>
        internal sealed override void ConsumeTransitionsEvents()
        {
            foreach (EventTransitionBase __eventTransition in eventTransitions) __eventTransition.ConsumeEvent();
        }

        /// <summary>
        ///     Finds the highest priority available <see cref="Transition"/> in this <see cref="StateLeaf"/>.
        ///     <see cref="Transition"/>s added first have higher priority. If no available <see cref="Transition"/> is found
        ///     it returns <see langword="null"/>.
        /// </summary>
        /// <returns>
        ///     The highest priority available <see cref="Transition"/>, <see langword="null"/> if none are available.
        /// </returns>
        internal sealed override Transition AvailableTransition
        {
            get
            {
                Transition __availableTransition = null;
                foreach (Transition __transition in transitions)
                {
                    if (!__transition.AllConditionsMet()) continue;
                
                    __availableTransition = __transition;
                    break;
                }

                return __availableTransition;   
            }
        }

        /// <summary>
        ///     Executes the code needed to implement the state behaviour of a
        ///     hierarchical finite state machine pattern.
        /// </summary>
        internal sealed override void UpdateInternal()
        {
            UpdateState();
        }

        /// <summary>
        ///     Executes the code needed to implement the state behaviour of a
        ///     hierarchical finite state machine pattern as well as the update cycle code defined
        ///     in the extended classes.
        /// </summary>
        public sealed override void Update()
        {
            UpdateInternal();
        }

        /// <summary>
        ///     Executes the code needed to implement the state behaviour of a
        ///     hierarchical finite state machine pattern as well as the fixed update cycle code defined
        ///     in the extended classes.
        /// </summary>
        public sealed override void FixedUpdate()
        {
            FixedUpdateState();
        }

        public sealed override void LateFixedUpdate()
        {
            LateFixedUpdateState();
        }

        /// <summary>
        ///     Executes the code needed to implement the state behaviour of a
        ///     hierarchical finite state machine pattern as well as the late update cycle code defined
        ///     in the extended classes.
        /// </summary>
        public sealed override void LateUpdate()
        {
            LateUpdateState();
        }

        /// <summary>
        ///     Executes the code needed to implement the state behaviour of a
        ///     hierarchical finite state machine pattern as well as the logic defined in the extended classes.
        ///     This function is called the first update cycle after this <see cref="StateLeaf"/> has become active.
        /// </summary>
        internal sealed override void Enter()
        {
            IsActive = true;
            OnEnter?.Invoke();
            EnterState();
        }

        /// <summary>
        ///     Executes the code needed to implement the state behaviour of a
        ///     hierarchical finite state machine pattern as well as the logic defined in the extended classes.
        ///     This function is called the last update cycle before this <see cref="StateLeaf"/> becomes inactive.
        /// </summary>
        internal sealed override void Exit()
        {
            ExitState();
            OnExit?.Invoke();
            IsActive = false;
        }

        /// <summary>
        ///     Returns the type of this <see cref="StateLeaf" /> converted to string.
        /// </summary>
        /// <returns>
        ///     The type of this <see cref="StateLeaf" /> converted to string.
        /// </returns>
        public sealed override String CurrentStateName => GetType().ToString();

        /// <summary>
        ///     Definition of "Any State" used in <see cref="Transition" />s from whose <see cref="Transition.From" />
        ///     can be any.
        /// </summary>
        internal class Any : StateLeaf { }
    }
}