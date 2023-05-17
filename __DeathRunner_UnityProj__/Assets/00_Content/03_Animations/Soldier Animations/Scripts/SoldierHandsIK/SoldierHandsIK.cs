﻿///////////////////////////////////////////////////////////////////////////
//  SoldierHandsIK - MonoBehaviour Script				         		 //
//  Kevin Iglesias - https://www.keviniglesias.com/     			     //
//  Contact Support: support@keviniglesias.com                           //
///////////////////////////////////////////////////////////////////////////


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KevinIglesias {
    
    public enum SoldierIKGoal {LeftHand = 0, RightHand = 1};
    
	public class SoldierHandsIK : MonoBehaviour
	{
		public Transform retargeter;
        public Transform handEffector;
        
        public SoldierIKGoal hand;
        
		private Animator animator;
		private float weight;

		private void Awake()
		{
			animator = GetComponent<Animator>();
			weight = 0f;
		}

		private void Update()
		{

            weight = Mathf.Lerp(0, 1, 1f - Mathf.Cos(retargeter.localPosition.y * Mathf.PI * 0.5f));

		}

		private void OnAnimatorIK(int layerIndex)
		{
            if(hand == SoldierIKGoal.LeftHand)
            {
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, weight);
                animator.SetIKPosition(AvatarIKGoal.LeftHand, handEffector.position);
                
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, weight);
                animator.SetIKRotation(AvatarIKGoal.LeftHand, handEffector.rotation);
            }else{
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, weight);
                animator.SetIKPosition(AvatarIKGoal.RightHand, handEffector.position);
                
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, weight);
                animator.SetIKRotation(AvatarIKGoal.RightHand, handEffector.rotation);
            }
		}
	}
}
