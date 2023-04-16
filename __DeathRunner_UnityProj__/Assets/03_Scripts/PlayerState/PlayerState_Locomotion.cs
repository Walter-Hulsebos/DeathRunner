//System libraries first
using System;

//Unity-specific libraries next
using UnityEngine;
using static Unity.Mathematics.math;

//Third-party libraries next
using JetBrains.Annotations;
using HFSM;
using ProjectDawn.Mathematics;
using GenericScriptableArchitecture;

//Project-specific libraries last
using DeathRunner.Shared;
using Drawing;
using ProjectDawn.Geometry3D;

using F32   = System.Single;
using F32x2 = Unity.Mathematics.float2;
using F32x3 = Unity.Mathematics.float3;
using Bool  = System.Boolean;
using Rotor = Unity.Mathematics.quaternion;
using Ray   = UnityEngine.Ray;

namespace DeathRunner.PlayerState
{
    public sealed class PlayerState_Locomotion : State
    {
        #region Variables

        private readonly LocomotionSettings _settings;
        private readonly PlayerReferences   _references;
        
        #endregion

        #region Constructor

        public PlayerState_Locomotion(LocomotionSettings settings, PlayerReferences references,
            params StateObject[] childStates) : base(childStates: childStates)
        {
            this._settings   = settings;
            this._references = references;
        }
        
        #endregion

        #region Methods
        
        protected override void OnEnter()
        {
            base.OnEnter();
            
            Debug.Log("Locomotion.Enter");
        }
        
        protected override void OnExit()
        {
            base.OnExit();
            
            Debug.Log("Locomotion.Exit");
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            F32x3 __lookPosition = LookPosition;
            
            OrientTowardsPos(lookPosition: __lookPosition);
            _references.LookAt.position = __lookPosition;
        }

        private const F32 LOOK_DISTANCE = 5;
        private F32x3 _cachedLookPositionRelativeToPlayer = new(x: 0, y: 0, z: +LOOK_DISTANCE);
        public F32x3 LookPositionRelativeToPlayer
        {
            get
            {
                //Get Mouse Position Screen-Space
                if (Commands.PlayerIsUsingAGamepad)
                {
                    F32x2 __aimInput = _references.InputHandler.AimInput;
                    
                    F32 __aimInputSqrMagnitude = lengthsq(__aimInput);

                    const F32 MAGNITUDE_THRESHOLD = 0.2f;
                    const F32 SQR_MAGNITUDE_THRESHOLD = MAGNITUDE_THRESHOLD * MAGNITUDE_THRESHOLD;
                    
                    
                    Bool __hasAimInput = (__aimInputSqrMagnitude > SQR_MAGNITUDE_THRESHOLD); 
                    //any(_references.InputHandler.AimInput != F32x2.zero);

                    if (__hasAimInput)
                    {
                        F32x3 __targetLookDirection = normalize(new F32x3(x: _references.InputHandler.AimInput.x, y: 0, z: _references.InputHandler.AimInput.y));
                    
                        F32x3 __targetMoveDirectionRelativeToCamera = __targetLookDirection.RelativeTo(_references.Camera.transform);
                    
                        _cachedLookPositionRelativeToPlayer = __targetMoveDirectionRelativeToCamera * LOOK_DISTANCE;
                    }
                }
                //if (all(_references.InputHandler.AimInput == F32x2.zero))
                else
                {
                    Vector3 __mouseScreenPosition = _references.InputHandler.MouseScreenPosition;
                
                    //Create ray from the camera to the mouse position
                    Ray __ray = _references.Camera.ScreenPointToRay(pos: __mouseScreenPosition);
                
                    //Cast ray to the ground plane
                    Plane __groundPlane = new(inNormal: Vector3.up, inPoint: _references.WorldPos);
                    Boolean __rayHasHit = __groundPlane.Raycast(ray: __ray, enter: out F32 __hitDistance);

                    if (__rayHasHit)
                    {
                        _cachedLookPositionRelativeToPlayer = (F32x3)__ray.GetPoint(distance: __hitDistance) - _references.WorldPos;
                    }
                }
                // else
                // {
                //     //_cachedLookPositionRelativeToPlayer = new float3(_references.WorldPos.x + _references.InputHandler.AimInput.x, _references.WorldPos.y, _references.WorldPos.z + _references.InputHandler.AimInput.y);
                //     
                //     F32x3 __targetLookDirection = new(x: _references.InputHandler.AimInput.x, y: 0, z: _references.InputHandler.AimInput.y);
                //     
                //     F32x3 __targetMoveDirectionRelativeToCamera = __targetLookDirection.RelativeTo(_references.Camera.transform);
                //     
                //     _cachedLookPositionRelativeToPlayer = _references.WorldPos + __targetMoveDirectionRelativeToCamera * 5;
                //     
                //     Debug.Log("Using Gamepad, LookPosition = " + _cachedLookPositionRelativeToPlayer);
                // }


                #if UNITY_EDITOR
                
                F32x3 __lookPosition = _references.WorldPos + _cachedLookPositionRelativeToPlayer;
                
                Draw.SolidCircleXZ(center: _references.WorldPos, radius: 0.25f, color: Color.yellow);
                Draw.SolidCircleXZ(center: __lookPosition,       radius: 0.25f, color: Color.yellow);
                Draw.Line(a: _references.WorldPos, b: __lookPosition, color: Color.yellow);
                #endif
                
                //Debug.DrawLine(start: );

                return _cachedLookPositionRelativeToPlayer;
            }
        }
        
        public F32x3 LookPosition => _references.WorldPos + LookPositionRelativeToPlayer;
        
        //public F32x3 LookDirection => normalize(LookPosition - _references.WorldPos);

        private void OrientTowardsPos(F32x3 lookPosition)
        {
            _settings.OrientationLookDirection.Value = normalize(lookPosition - _references.WorldPos);
            
            OrientTowardsDir(lookDirection: _settings.OrientationLookDirection.Value);
        }
        
        public void OrientTowardsDir(F32x3 lookDirection)
        {
            Plane3D __plane3D = new(normal: up(), distance: 0);
            
            F32x3 __projectedLookDirection = normalize(__plane3D.Projection(point: lookDirection));
            
            if (lengthsq(__projectedLookDirection) == 0) return;

            Rotor __targetRotation = Rotor.LookRotation(forward: __projectedLookDirection, up: up());

            _references.Rot = slerp(q1: _references.Rot, q2: __targetRotation, t: (F32)_settings.OrientationSpeed * Commands.DeltaTime);
        }

        #endregion
    }
    
    [Serializable]
    public struct LocomotionSettings 
    {
        [field:SerializeField] public Constant<F32>   OrientationSpeed         { get; [UsedImplicitly] private set; }
        
        [field:SerializeField] public Variable<F32x3> OrientationLookDirection { get; [UsedImplicitly] private set; }
    }
}