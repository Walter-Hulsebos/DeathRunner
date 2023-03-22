using System;
using System.Collections.Generic;

namespace HFSM
{
    /// <inheritdoc cref="EventTransitionBase" />
    internal class EventTransition : EventTransitionBase
    {
        private readonly Func<Boolean>[] _conditions;

        /// <summary>
        ///     Class constructor.
        /// </summary>
        /// <param name="from">
        ///     Origin <see cref="StateObject" /> of the transition.
        /// </param>
        /// <param name="to">
        ///     Target <see cref="StateObject" /> of the transition.
        /// </param>
        /// <param name="transitionAction">
        ///     Function executed (if defined) when the transition occurs.
        /// </param>
        /// <param name="processInstantly">
        ///     Whether to process this transition as soon as the event that it is subscribed to is fired or not.
        /// </param>
        /// <param name="conditions">
        ///     List of conditions that must be met (all of them) in order for the transition to occur.
        /// </param>
        public EventTransition(StateObject from, StateObject to, Action transitionAction = null, Boolean processInstantly = false, params Func<Boolean>[] conditions) 
            : base(from: from, to: to, transitionAction: transitionAction, processInstantly: processInstantly)
        {
            this._conditions = conditions;
        }

        /// <summary>
        ///     Listens to the event this transition is subscribed to. It only listens to the event if
        ///     <see cref="Transition.From" /> is active or if <see cref="Transition.From" /> is
        ///     "Any" and <see cref="Transition.From" />'s <see cref="State" /> is active.
        /// </summary>
        public void ListenEvent()
        {
            if (processInstantly)
            {
                From.State.ProcessInstantEvent(this);
            }
            else if (From.IsActive || (From.GetType() == typeof(StateLeaf.Any) && From.State.IsActive))
            {
                eventListened = true;
            }
        }

        /// <inheritdoc cref="EventTransitionBase.ConditionsMet" />
        protected override Boolean ConditionsMet()
        {
            Boolean __conditionsMet = true;
            foreach (Func<Boolean> __condition in conditions)
            {
                if (__condition()) continue;
                
                __conditionsMet = false;
                break;
            }

            return __conditionsMet;
        }
    }

    /// <inheritdoc cref="EventTransitionBase" />
    internal class EventTransition<T> : EventTransitionBase
    {
        private readonly List<T> _args = new List<T>(); // An event can be fired more than once per frame. Cache the args from every fired event.
        private T _currentArg;

        private readonly Func<T, Boolean>[] _conditions;
        private readonly Action<T> _transitionAction;

        /// <inheritdoc cref="EventTransition.EventTransition(StateObject, StateObject, Action, bool, Func{bool}[])" />
        public EventTransition(StateObject from, StateObject to, Action<T> transitionAction = null, Boolean processInstantly = false, params Func<T, Boolean>[] conditions) 
            : base(from: from, to: to, processInstantly: processInstantly)
        {
            this._conditions = conditions;
            this._transitionAction = transitionAction;
        }

        /// <inheritdoc cref="EventTransitionBase.ConsumeEvent" />
        public override void ConsumeEvent()
        {
            _args.Clear();
            //currentArg = default(T);
            base.ConsumeEvent();
        }

        /// <inheritdoc cref="EventTransition.ListenEvent" />
        public void ListenEvent(T arg)
        {
            if (processInstantly)
            {
                From.State.ProcessInstantEvent(this);
            }
            else if (From.IsActive ||
                     (From.GetType() == typeof(StateLeaf.Any) && From.State.IsActive))
            {
                eventListened = true;
                _args.Add(arg);
            }
        }

        /// <inheritdoc cref="EventTransitionBase.ConditionsMet" />
        protected override Boolean ConditionsMet()
        {
            Boolean __conditionsMet = true;
            _currentArg = default;
            foreach (T __arg in _args)
            {
                _currentArg = __arg;
                foreach (Func<T, Boolean> __condition in _conditions)
                    if (!__condition(__arg))
                    {
                        __conditionsMet = false;
                        break;
                    }

                if (__conditionsMet) break;
            }

            return __conditionsMet;
        }

        /// <inheritdoc cref="Transition.InvokeTransitionAction" />
        public override void InvokeTransitionAction()
        {
            _transitionAction?.Invoke(_currentArg); // Execute the transition action using the first event argument that met all the conditions
        }
    }

    /// <inheritdoc cref="EventTransitionBase" />
    internal class EventTransition<T1, T2> : EventTransitionBase
    {
        private readonly List<(T1, T2)> _args = new List<(T1, T2)>(); // An event can be fired more than once per frame. Cache the args from every fired event.
        private (T1, T2) _currentArgs;

        private readonly Func<T1, T2, Boolean>[] _conditions;
        private readonly Action<T1, T2> _transitionAction;

        /// <inheritdoc cref="EventTransition.EventTransition(StateObject, StateObject, Action, bool, Func{bool}[])" />
        public EventTransition(StateObject from, StateObject to, Action<T1, T2> transitionAction = null, Boolean processInstantly = false, params Func<T1, T2, Boolean>[] conditions) 
            : base(from: from, to: to, processInstantly: processInstantly)
        {
            this._transitionAction = transitionAction;
            this._conditions = conditions;
        }

        /// <inheritdoc cref="EventTransitionBase.ConsumeEvent" />
        public override void ConsumeEvent()
        {
            _args.Clear();
            base.ConsumeEvent();
        }

        /// <inheritdoc cref="EventTransition.ListenEvent" />
        public void ListenEvent(T1 arg1, T2 arg2)
        {
            if (processInstantly)
            {
                From.State.ProcessInstantEvent(this);
            }
            else if (From.IsActive || (From.GetType() == typeof(StateLeaf.Any) && From.State.IsActive))
            {
                eventListened = true;
                _args.Add((arg1, arg2));
            }
        }

        /// <inheritdoc cref="EventTransitionBase.ConditionsMet" />
        protected override Boolean ConditionsMet()
        {
            Boolean __conditionsMet = true;
            _currentArgs = default;
            foreach ((T1, T2) __argTuple in _args)
            {
                _currentArgs = __argTuple;
                foreach (Func<T1, T2, Boolean> __condition in _conditions)
                    if (!__condition(_currentArgs.Item1, _currentArgs.Item2))
                    {
                        __conditionsMet = false;
                        break;
                    }

                if (__conditionsMet) break;
            }

            return __conditionsMet;
        }

        /// <inheritdoc cref="Transition.InvokeTransitionAction" />
        public override void InvokeTransitionAction()
        {
            _transitionAction?.Invoke(_currentArgs.Item1, _currentArgs.Item2);
        }
    }

    /// <inheritdoc cref="EventTransitionBase" />
    internal class EventTransition<T1, T2, T3> : EventTransitionBase
    {
        private readonly List<(T1, T2, T3)> _args = new List<(T1, T2, T3)>();
        private (T1, T2, T3) _currentArgs;
        
        private readonly Func<T1, T2, T3, Boolean>[] _conditions;
        private readonly Action<T1, T2, T3> _transitionAction;

        /// <inheritdoc cref="EventTransition.EventTransition(StateObject, StateObject, Action, bool, Func{bool}[])" />
        public EventTransition(StateObject from, StateObject to, Action<T1, T2, T3> transitionAction = null, Boolean processInstantly = false, params Func<T1, T2, T3, Boolean>[] conditions) 
            : base(from: from, to: to, processInstantly: processInstantly)
        {
            this._conditions = conditions;
            this._transitionAction = transitionAction;
            
            //_currentArgs = default;
        }

        /// <inheritdoc cref="EventTransitionBase.ConsumeEvent" />
        public override void ConsumeEvent()
        {
            _args.Clear();
            base.ConsumeEvent();
        }

        /// <inheritdoc cref="EventTransition.ListenEvent" />
        public void ListenEvent(T1 arg1, T2 arg2, T3 arg3)
        {
            if (processInstantly)
            {
                From.State.ProcessInstantEvent(this);
            }
            else if (From.IsActive || (From.GetType() == typeof(StateLeaf.Any) && From.State.IsActive))
            {
                eventListened = true;
                _args.Add((arg1, arg2, arg3));
            }
        }

        /// <inheritdoc cref="EventTransitionBase.ConditionsMet" />
        protected override Boolean ConditionsMet()
        {
            Boolean __conditionsMet = true;
            _currentArgs = default;
            foreach ((T1, T2, T3) __argTuple in _args)
            {
                _currentArgs = __argTuple;
                foreach (Func<T1, T2, T3, Boolean> __condition in _conditions)
                {
                    if (__condition(__argTuple.Item1, __argTuple.Item2, __argTuple.Item3)) continue;
                    
                    __conditionsMet = false;
                    break;
                }

                if (__conditionsMet) break;
            }

            return __conditionsMet;
        }

        /// <inheritdoc cref="Transition.InvokeTransitionAction" />
        public override void InvokeTransitionAction()
        {
            _transitionAction?.Invoke(_currentArgs.Item1, _currentArgs.Item2, _currentArgs.Item3);
        }
    }

    /// <inheritdoc cref="EventTransitionBase" />
    internal class EventTransition<T1, T2, T3, T4> : EventTransitionBase
    {
        private readonly List<(T1, T2, T3, T4)> _args = new List<(T1, T2, T3, T4)>();
        private (T1, T2, T3, T4) _currentArgs;

        private readonly Func<T1, T2, T3, T4, Boolean>[] _conditions;
        private readonly Action<T1, T2, T3, T4> _transitionAction;

        /// <inheritdoc cref="EventTransition.EventTransition(StateObject, StateObject, Action, bool, Func{bool}[])" />
        public EventTransition(StateObject from, StateObject to, Action<T1, T2, T3, T4> transitionAction = null, Boolean processInstantly = false, params Func<T1, T2, T3, T4, Boolean>[] conditions)
            : base(from: from, to: to, processInstantly: processInstantly)
        {
            this._conditions = conditions;
            this._transitionAction = transitionAction;

            //_currentArgs = default;
        }

        /// <inheritdoc cref="EventTransitionBase.ConsumeEvent" />
        public override void ConsumeEvent()
        {
            _args.Clear();
            base.ConsumeEvent();
        }

        /// <inheritdoc cref="EventTransition.ListenEvent" />
        public void ListenEvent(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            if (processInstantly)
            {
                From.State.ProcessInstantEvent(this);
            }
            else if (From.IsActive || (From.GetType() == typeof(StateLeaf.Any) &&
                                                    From.State.IsActive))
            {
                eventListened = true;
                _args.Add((arg1, arg2, arg3, arg4));
            }
        }

        /// <inheritdoc cref="EventTransitionBase.ConditionsMet" />
        protected override Boolean ConditionsMet()
        {
            Boolean __conditionsMet = true;
            _currentArgs = default;
            foreach ((T1, T2, T3, T4) __argTuple in _args)
            {
                _currentArgs = __argTuple;
                foreach (Func<T1, T2, T3, T4, Boolean> __condition in _conditions)
                {
                    if (__condition(__argTuple.Item1, __argTuple.Item2, __argTuple.Item3, __argTuple.Item4)) continue;

                    __conditionsMet = false;
                    break;
                }

                if (__conditionsMet) break;
            }

            return __conditionsMet;
        }

        /// <inheritdoc cref="Transition.InvokeTransitionAction" />
        public override void InvokeTransitionAction()
        {
            _transitionAction?.Invoke(_currentArgs.Item1, _currentArgs.Item2, _currentArgs.Item3, _currentArgs.Item4);
        }
    }

    /// <inheritdoc cref="EventTransitionBase" />
    internal class EventTransition<T1, T2, T3, T4, T5> : EventTransitionBase
    {
        private readonly List<(T1, T2, T3, T4, T5)> _args = new List<(T1, T2, T3, T4, T5)>();
        private (T1, T2, T3, T4, T5) _currentArgs;

        private readonly Func<T1, T2, T3, T4, T5, Boolean>[] _conditions;
        private readonly Action<T1, T2, T3, T4, T5> _transitionAction;

        /// <inheritdoc cref="EventTransition.EventTransition(StateObject, StateObject, Action, bool, Func{bool}[])" />
        public EventTransition(StateObject from, StateObject to, Action<T1, T2, T3, T4, T5> transitionAction = null, Boolean processInstantly = false, params Func<T1, T2, T3, T4, T5, Boolean>[] conditions)
            : base(from: from, to: to, processInstantly: processInstantly)
        {
            this._conditions = conditions;
            this._transitionAction = transitionAction;

            //_currentArgs = default;
        }

        /// <inheritdoc cref="EventTransitionBase.ConsumeEvent" />
        public override void ConsumeEvent()
        {
            _args.Clear();
            base.ConsumeEvent();
        }

        /// <inheritdoc cref="EventTransition.ListenEvent" />
        public void ListenEvent(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            if (processInstantly)
            {
                From.State.ProcessInstantEvent(this);
            }
            else if (From.IsActive || (From.GetType() == typeof(StateLeaf.Any) &&
                                                    From.State.IsActive))
            {
                eventListened = true;
                _args.Add((arg1, arg2, arg3, arg4, arg5));
            }
        }

        /// <inheritdoc cref="EventTransitionBase.ConditionsMet" />
        protected override Boolean ConditionsMet()
        {
            Boolean __conditionsMet = true;
            _currentArgs = default;
            foreach ((T1, T2, T3, T4, T5) __argTuple in _args)
            {
                _currentArgs = __argTuple;
                foreach (Func<T1, T2, T3, T4, T5, Boolean> __condition in _conditions)
                {
                    if (__condition(__argTuple.Item1, __argTuple.Item2, __argTuple.Item3, __argTuple.Item4, __argTuple.Item5)) continue;

                    __conditionsMet = false;
                    break;
                }

                if (__conditionsMet) break;
            }

            return __conditionsMet;
        }

        /// <inheritdoc cref="Transition.InvokeTransitionAction" />
        public override void InvokeTransitionAction()
        {
            _transitionAction?.Invoke(_currentArgs.Item1, _currentArgs.Item2, _currentArgs.Item3, _currentArgs.Item4, _currentArgs.Item5);
        }
    }
}