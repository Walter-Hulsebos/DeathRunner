using UnityEngine;

using F32 = System.Single;

namespace DeathRunner.Shared
{
    public readonly struct AttackData
    {
        public readonly AnimationClip attackAnimation;
        public readonly F32 secondsToAllowNextAttack;
        
        public AttackData(AnimationClip attackAnimation, F32 secondsToAllowNextAttack)
        {
            this.attackAnimation = attackAnimation;
            this.secondsToAllowNextAttack = secondsToAllowNextAttack;
        }
    }
}
