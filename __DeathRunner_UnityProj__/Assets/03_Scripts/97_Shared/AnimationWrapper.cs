using System;
using UnityEngine;

namespace DeathRunner.Shared
{
    public readonly struct AnimationWrapper
    {
        public readonly AnimationClip animationClip;
        
        public AnimationWrapper(AnimationClip animationClip)
        {
            this.animationClip = animationClip;
        }
    }
}
