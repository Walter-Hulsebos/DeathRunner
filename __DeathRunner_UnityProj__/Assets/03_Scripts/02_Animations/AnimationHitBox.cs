using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace DeathRunner.Animations
{
    public sealed class AnimationHitBox : MonoBehaviour
    {
        [SerializeField] private Collider colliderToEnable;
        [SerializeField] private MMF_Player feedbacks;

        
        private void Start()
        {
            colliderToEnable.enabled = false;   
        }
        [UsedImplicitly] // its used somewhere, source: Trust me bro
        public void EnableHitBox()
        {
            colliderToEnable.enabled = true;
            feedbacks.PlayFeedbacks();
        }
        
        [UsedImplicitly]
        public void DisableHitBox()
        {
            colliderToEnable.enabled = false;
        }
        
        
    }
}
