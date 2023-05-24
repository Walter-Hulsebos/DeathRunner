using System;
using Animancer;
using EasyCharacterMovement;
using UnityEngine;

namespace DeathRunner.Player
{
    public class RedirectRootMotionToMotor : RedirectRootMotion<CharacterMotor>
    {
        //private Vector3    _startPosition;
        //private Quaternion _startRotation;
        
        private Vector3    _positionOffset;
        private Quaternion _rotationOffset;
        
        // private Vector3    _endPosition;
        // private Quaternion _endRotation;
        
        //private Bool _hadNoRootMotionLastFrame = false;
        
        /// <inheritdoc/>
        protected override void OnAnimatorMove()
        {
            if (!ApplyRootMotion) return;
            //{
                //_hadNoRootMotionLastFrame = true;
                //return;
            //}
            
            //Debug.Log("HasRootMotion = " + Animator.hasRootMotion);
            
            //if(_hadNoRootMotionLastFrame) // If we just started root motion
            //{
                //_hadNoRootMotionLastFrame = false;
                
                //Transform __targetTransform = Target.transform;
                //_startPosition = __targetTransform.position;
                //_startRotation = __targetTransform.rotation;
            //}
            
            // if(Animator.deltaPosition != Vector3.zero)
            // {
            //     Debug.Log("Animator.deltaPosition: " + Animator.deltaPosition);
            // }
            //
            // if(Animator.deltaRotation != Quaternion.identity)
            // {
            //     Debug.Log("Animator.deltaRotation: " + Animator.deltaRotation.eulerAngles);
            // }
            
            Vector3 __deltaPosition  = Animator.deltaPosition;
            Vector3 __sweepDirection = __deltaPosition.normalized;

            Target.rigidbody.interpolation = RigidbodyInterpolation.None;
            //Target.SetPosition(Target.position + Animator.deltaPosition);
            //Target.SetRotation(Target.rotation * Animator.deltaRotation);
            
            Boolean __hitSomethingInSweep = Target.MovementSweepTest
            (
                characterPosition: Target.position, 
                sweepDirection:    __sweepDirection, 
                sweepDistance:     __deltaPosition.magnitude, 
                collisionResult:   out CollisionResult __collisionResult
            );
            
            Vector3 __displacement = (__hitSomethingInSweep) 
                ? __collisionResult.displacementToHit 
                : __deltaPosition;
            
            
            Target.SetPositionAndRotation(newPosition: Target.position + __displacement, newRotation: Target.rotation * Animator.deltaRotation, updateGround: true);
            Target.rigidbody.interpolation = RigidbodyInterpolation.Interpolate;

            // if (!ApplyRootMotion)
            // {
            //     if(_hadNoRootMotionLastFrame == false) // If we just stopped root motion
            //     {
            //         //Debug.Log("Set end position and rotation");
            //         Target.SetPositionAndRotation(position: _endPosition, rotation: _endRotation);
            //     }
            //
            //     _hadNoRootMotionLastFrame = true;
            //     return;
            // }
            //
            // if(_hadNoRootMotionLastFrame) // If we just started root motion
            // {
            //     _hadNoRootMotionLastFrame = false;
            //     
            //     Transform __targetTransform = Target.transform;
            //     _startPosition = __targetTransform.position;
            //     _startRotation = __targetTransform.rotation;
            // }
            //
            // _endPosition = _startPosition + Animator.deltaPosition;
            // _endRotation = _startRotation * Animator.deltaRotation;
            //
            // if(Animator.deltaPosition != Vector3.zero)
            // {
            //     Debug.Log("Animator.deltaPosition: " + Animator.deltaPosition);
            // }
            //
            // if(Animator.deltaRotation != Quaternion.identity)
            // {
            //     Debug.Log("Animator.deltaRotation: " + Animator.deltaRotation.eulerAngles);
            // }
        }
    }
}