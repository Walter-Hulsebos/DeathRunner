using HFSM;

namespace DeathRunner.Shared.StateMachine
{
    public sealed class PlayerState_Alive : State
    {
        public PlayerState_Alive(params StateObject[] childStates) : base(childStates: childStates) { }
    }
}
