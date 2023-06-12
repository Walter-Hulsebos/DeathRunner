using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace DeathRunner.Animations
{
    public sealed class AnimationHitBox : MonoBehaviour
    {
        [SerializeField] private Collider colliderToEnable;
        
        private void Start()
        {
            colliderToEnable.enabled = false;   
        }
        [UsedImplicitly] // its used somewhere, source: Trust me bro
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
