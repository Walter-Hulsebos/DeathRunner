using UnityEngine;
using static Unity.Mathematics.math;

using F32   = System.Single;
using F32x3 = Unity.Mathematics.float3;
using F32x4 = Unity.Mathematics.float4;

using Rotor = Unity.Mathematics.quaternion;

using Bool = System.Boolean;

namespace DeathRunner.Utils
{
    public static class MathExtensions
    {
        public static F32x3 SmoothDamp(this F32x3 current, F32x3 target, ref F32x3 currentVelocity, F32 deltaTime, F32 smoothTime, F32 maxSpeed)
        {
            // Based on Game Programming Gems 4 Chapter 1.10
            smoothTime = max(0.0001f, smoothTime);
            
            F32 __omega = 2f / smoothTime;
            
            F32   __x = __omega * Time.deltaTime;
            F32   __exp = 1f / (1f + __x + 0.48f * __x * __x + 0.235f * __x * __x * __x);
            F32x3 __change = current - target;
            F32x3 __originalTo = target;

            // Clamp maximum speed
            F32 __maxChange = maxSpeed * smoothTime;
            __change = clamp(__change, -__maxChange, __maxChange);
            target = current - __change;

            F32x3 __temp = (currentVelocity + __omega * __change) * deltaTime;
            currentVelocity = (currentVelocity - __omega * __temp) * __exp;
            
            F32x3 __output = target + (__change + __temp) * __exp;

            // Prevent overshooting
            if (all(__originalTo - current > 0.0f == __output > __originalTo))
            {
                __output = __originalTo;
                currentVelocity = (__output - __originalTo) / deltaTime;
            }

            return __output;
        }
        
        public static F32x4 SmoothDamp(this F32x4 current, F32x4 target, ref F32x4 currentVelocity, F32 smoothTime, F32 maxSpeed)
        {
            // Based on Game Programming Gems 4 Chapter 1.10
            smoothTime = max(0.0001f, smoothTime);
            
            F32 __omega = 2f / smoothTime;
            
            F32   __x = __omega * Time.deltaTime;
            F32   __exp = 1f / (1f + __x + 0.48f * __x * __x + 0.235f * __x * __x * __x);
            F32x4 __change = current - target;
            F32x4 __originalTo = target;

            // Clamp maximum speed
            F32 __maxChange = maxSpeed * smoothTime;
            __change = clamp(__change, -__maxChange, __maxChange);
            target = current - __change;

            F32x4 __temp = (currentVelocity + __omega * __change) * Time.deltaTime;
            currentVelocity = (currentVelocity - __omega * __temp) * __exp;
            
            F32x4 __output = target + (__change + __temp) * __exp;

            // Prevent overshooting
            if (all(__originalTo - current > 0.0f == __output > __originalTo))
            {
                __output = __originalTo;
                currentVelocity = (__output - __originalTo) / Time.deltaTime;
            }

            return __output;
        }
        
        public static Rotor SmoothDamp(this Rotor current, Rotor target, ref Rotor deriv, float smoothTime) 
        {
            // account for double-cover
            F32 __dot = dot(current, target);
            
            F32 __multi = (__dot > 0f) ? 1f : -1f;
            
            target.value *= __multi;

            // smooth damp (nlerp approx)
            F32x4 __result = normalize(SmoothDamp(current: current.value, target: target.value, currentVelocity: ref deriv.value, smoothTime: smoothTime, maxSpeed: 1f));
		
            // ensure deriv is tangent
            F32x4 __derivError = project(deriv.value, __result);
            deriv.value -= __derivError;

            return new Rotor(__result);
        }
        
        public static F32x3 MoveTowards(this F32x3 current, F32x3 target, F32 maxDistanceDelta)
        {
            F32 x = target.x - current.x;
            F32 y = target.y - current.y;
            F32 z = target.z - current.z;
            
            F32 __d = (x * x + y * y + z * z);
            
            if (__d == 0.0 || maxDistanceDelta >= 0.0 && __d <= maxDistanceDelta * maxDistanceDelta) return target;
            
            F32 __num4 = sqrt(__d);
            
            return new F32x3(
                x: current.x + x / __num4 * maxDistanceDelta, 
                y: current.y + y / __num4 * maxDistanceDelta, 
                z: current.z + z / __num4 * maxDistanceDelta);
        }
    }
}
