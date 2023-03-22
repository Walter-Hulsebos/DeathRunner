using HFSM;

namespace DeathRunner.Shared.StateMachine
{
    public sealed class PlayerState_Root : State
    {
        public PlayerState_Root(params StateObject[] stateObjects)
            : base(stateObjects)
        {
            
        }
    }
}
