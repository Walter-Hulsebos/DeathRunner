using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace DeathRunner.Animations
{
    public sealed class AnimationHitBox : MonoBehaviour
    {
        [SerializeField] private Collider colliderToEnable;
        
        [UsedImplicitly]
        public void EnableHitBox()
        {
            colliderToEnable.enabled = true;
        }
        
        [UsedImplicitly]
        public void DisableHitBox()
        {
            colliderToEnable.enabled = false;
        }
        
        
    }
}
