using DeathRunner.Shared;
using Drawing;
using ProjectDawn.Mathematics;
using ProjectDawn.Geometry3D;
using UnityEngine;

using static Unity.Mathematics.math;

using DeathRunner.Shared;
using DeathRunner.Utils;

using F32   = System.Single;
using F32x2 = Unity.Mathematics.float2;
using F32x3 = Unity.Mathematics.float3;

using Rotor = Unity.Mathematics.quaternion;

using Bool  = System.Boolean;

using Ray   = UnityEngine.Ray;

namespace DeathRunner.PlayerState
{
    public static class PlayerHelpers
    {
        private const F32 LOOK_DISTANCE = 5;
        private static F32x3 _cachedLookPositionRelativeToPlayer = new(x: 0, y: 0, z: +LOOK_DISTANCE);
        public static F32x3 LookPositionRelativeToPlayer(PlayerReferences references)
        {
            //Get Mouse Position Screen-Space
            if (Commands.PlayerIsUsingAGamepad)
            {
                F32x2 __aimInput = references.InputHandler.AimInput;
                
                F32 __aimInputSqrMagnitude = lengthsq(__aimInput);

                const F32 MAGNITUDE_THRESHOLD = 0.2f;
                const F32 SQR_MAGNITUDE_THRESHOLD = MAGNITUDE_THRESHOLD * MAGNITUDE_THRESHOLD;
                
                
                Bool __hasAimInput = (__aimInputSqrMagnitude > SQR_MAGNITUDE_THRESHOLD); 
                //any(_references.InputHandler.AimInput != F32x2.zero);

                if (__hasAimInput)
                {
                    F32x3 __targetLookDirection = normalize(new F32x3(x: references.InputHandler.AimInput.x, y: 0, z: references.InputHandler.AimInput.y));
                
                    F32x3 __targetMoveDirectionRelativeToCamera = __targetLookDirection.RelativeTo(references.Camera.transform);
                
                    _cachedLookPositionRelativeToPlayer = __targetMoveDirectionRelativeToCamera * LOOK_DISTANCE;
                }
            }
            //if (all(_references.InputHandler.AimInput == F32x2.zero))
            else
            {
                F32x3 __mouseScreenPosition = new(xy: references.InputHandler.MouseScreenPosition, z: 0);

                //Check if __mouseScreenPosition.xy is the same as Mouse.current.position.ReadValue()
                //Create ray from the camera to the mouse position
                Ray __ray = references.Camera.ScreenPointToRay(pos: __mouseScreenPosition);
            
                //Cast ray to the ground plane
                Plane __groundPlane = new(inNormal: Vector3.up, inPoint: references.WorldPos);
                Bool __rayHasHit = __groundPlane.Raycast(ray: __ray, enter: out F32 __hitDistance);

                if (__rayHasHit)
                {
                    _cachedLookPositionRelativeToPlayer = (F32x3)__ray.GetPoint(distance: __hitDistance) - references.WorldPos;
                }
            }

            #if UNITY_EDITOR
            
            F32x3 __lookPosition = references.WorldPos + _cachedLookPositionRelativeToPlayer;
            
            Draw.SolidCircleXZ(center: references.WorldPos, radius: 0.25f, color: Color.yellow);
            Draw.SolidCircleXZ(center: __lookPosition,      radius: 0.25f, color: Color.yellow);
            Draw.Line(a: references.WorldPos, b: __lookPosition, color: Color.yellow);
            #endif
            
            //Debug.DrawLine(start: );

            return _cachedLookPositionRelativeToPlayer;
        }
        
        public static void OrientTowardsDir(PlayerReferences references, F32x3 direction, F32 orientationSpeed) 
        {
            Plane3D __plane3D = new(normal: up(), distance: 0);
            
            F32x3 __projectedLookDirection = normalize(__plane3D.Projection(point: direction));
            
            if (lengthsq(__projectedLookDirection) == 0) return;

            Rotor __targetRotation = Rotor.LookRotation(forward: __projectedLookDirection, up: up());

            references.Rot = slerp(q1: references.Rot, q2: __targetRotation, t: orientationSpeed * Commands.DeltaTime);
        }
    }
}
