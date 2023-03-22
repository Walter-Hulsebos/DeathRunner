using HFSM;

namespace DeathRunner.Shared.StateMachine
{
    public class PlayerState_BulletTime : State
    {
        public PlayerState_BulletTime(params StateObject[] childStates) : base(childStates: childStates) { }
    }
}
