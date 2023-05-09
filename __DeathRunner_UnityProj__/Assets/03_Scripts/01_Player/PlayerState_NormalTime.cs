using DeathRunner.Shared;
using HFSM;

namespace DeathRunner.Player
{
    public sealed class PlayerState_NormalTime : State
    {
        public PlayerState_NormalTime(params StateObject[] childStates) : base(childStates: childStates) { }
        
        protected override void OnEnter()
        {
            base.OnEnter();
            
            //Debug.Log("NormalTime.Enter");
            
            Commands.IsSlowMotionEnabled = false;
        }
        
        protected override void OnExit()
        {
            base.OnExit();
            
            //Debug.Log("NormalTime.Exit");
            
        }
    }
}