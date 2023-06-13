using UnityEngine;
using static Unity.Mathematics.math;

using Drawing;
using ProjectDawn.Mathematics;
using ProjectDawn.Geometry3D;

using DeathRunner.Shared;
using DeathRunner.Utils;
using UnityEngine.InputSystem;
using F32   = System.Single;
using F32x2 = Unity.Mathematics.float2;
using F32x3 = Unity.Mathematics.float3;

using Rotor = Unity.Mathematics.quaternion;

using Bool  = System.Boolean;

using Ray   = UnityEngine.Ray;

namespace DeathRunner.Player
{
    public static class PlayerHelpers
    {
        private const F32 LOOK_DISTANCE = 5;
        private static F32x3 _cachedLookPositionRelativeToPlayer = new(x: 0, y: 0, z: +LOOK_DISTANCE);
        public static F32x3 LookPositionRelativeToPlayer(PlayerReferences references, Bool useCursor = true)
        {
            if (Commands.PlayerIsUsingAGamepad || !useCursor) //Used for gamepads or when the player is not using the cursor
            {
                F32x2 __aimInput = (useCursor) ? references.InputHandler.AimInput : references.InputHandler.MoveInput;
                
                F32 __aimInputSqrMagnitude = lengthsq(__aimInput);

                const F32 MAGNITUDE_THRESHOLD = 0.2f;
                const F32 SQR_MAGNITUDE_THRESHOLD = MAGNITUDE_THRESHOLD * MAGNITUDE_THRESHOLD;
                
                Bool __hasAimInput = (__aimInputSqrMagnitude > SQR_MAGNITUDE_THRESHOLD); 
                //any(_references.InputHandler.AimInput != F32x2.zero);
                if (__hasAimInput)
                {
                    F32x3 __targetLookDirection = normalize(new F32x3(x: __aimInput.x, y: 0, z: __aimInput.y));
                
                    F32x3 __targetMoveDirectionRelativeToCamera = __targetLookDirection.RelativeTo(references.Camera.transform);
                
                    _cachedLookPositionRelativeToPlayer = __targetMoveDirectionRelativeToCamera * LOOK_DISTANCE;
                }
            }
            //if (all(_references.InputHandler.AimInput == F32x2.zero))
            else
            {
                //InputSystem.Update();
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
            
            /*
            F32x3 __lookPosition = references.WorldPos + _cachedLookPositionRelativeToPlayer;
            
            Draw.SolidCircleXZ(center: references.WorldPos, radius: 0.25f, color: Color.yellow);
            Draw.SolidCircleXZ(center: __lookPosition,      radius: 0.25f, color: Color.yellow);
            Draw.Line(a: references.WorldPos, b: __lookPosition, color: Color.yellow);
            */
            #endif
            
            //Debug.DrawLine(start: );

            return _cachedLookPositionRelativeToPlayer;
        }
        
        public static void OrientTowardsDir(PlayerReferences references, F32x3 direction, F32 orientationSpeed, F32 deltaTime) 
        {
            Plane3D __plane3D = new(normal: up(), distance: 0);
            
            F32x3 __projectedLookDirection = normalize(__plane3D.Projection(point: direction));
            
            // Early out if the direction is invalid
            if (lengthsq(__projectedLookDirection) == 0) return;

            Rotor __targetRotation = Rotor.LookRotation(forward: __projectedLookDirection, up: up());

            references.Rot = slerp(q1: references.Rot, q2: __targetRotation, t: orientationSpeed * deltaTime);
        }

        public static void OrientTowardsDirInstant(PlayerReferences references, F32x3 direction)
        {
            Plane3D __plane3D = new(normal: up(), distance: 0);
            
            F32x3 __projectedLookDirection = normalize(__plane3D.Projection(point: direction));
            
            // Early out if the direction is invalid
            if (lengthsq(__projectedLookDirection) == 0) return;

            Rotor __targetRotation = Rotor.LookRotation(forward: __projectedLookDirection, up: up());

            references.Rot = __targetRotation;
        }
        
        /// <summary>
        /// Move the character when falling or on not-walkable ground.
        /// </summary>
        public static F32x3 NotGroundedMovement(F32x3 velocity, F32x3 desiredVelocity, Bool isOnGround, F32x3 groundNormal, F32 maxAcceleration, F32 airControlPrimantissa, F32 airFriction, F32x3 gravity)
        {
            // If moving into non-walkable ground, limit its contribution.
            // Allow movement parallel, but not into it because that may push us up.
            if (isOnGround && dot(desiredVelocity, groundNormal) < 0.0f)
            {
                F32x3 __planeNormal  = normalize(new F32x3(x: groundNormal.x, y: 0, z: groundNormal.y));

                desiredVelocity = desiredVelocity.ProjectedOnPlane(planeNormal: __planeNormal);
            }
            
            F32x3 __flatVelocity = new(x: velocity.x, y: 0,          z: velocity.z);
            F32x3 __verVelocity  = new(x: 0,          y: velocity.y, z: 0);

            // Accelerate horizontal velocity towards desired velocity
            F32x3 __horizontalVelocity = __flatVelocity.MoveTowards(
                target:  desiredVelocity, 
                maxDistanceDelta: maxAcceleration * airControlPrimantissa * Time.deltaTime);

            // Update velocity preserving gravity effects (vertical velocity)
            velocity = __horizontalVelocity + __verVelocity;

            // Apply gravity
            velocity += gravity * Time.deltaTime;

            // Apply Air friction (Drag)
            velocity -= velocity * airFriction * Time.deltaTime;
            //__velocity -= clamp(1.0f - ((F32)_settings.AirFriction * Time.deltaTime), 0.0f, 1.0f);

            return velocity;
        }
    }
}
