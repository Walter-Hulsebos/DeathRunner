using HFSM;

namespace DeathRunner.Shared.StateMachine
{
    public sealed class PlayerState_NormalTime : State
    {
        public PlayerState_NormalTime(params StateObject[] childStates) : base(childStates: childStates) { }
    }
}