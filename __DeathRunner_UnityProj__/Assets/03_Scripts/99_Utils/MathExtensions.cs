using UnityEngine;
using static Unity.Mathematics.math;

using F32   = System.Single;
using F32x3 = Unity.Mathematics.float3;
using F32x4 = Unity.Mathematics.float4;

using F64   = System.Double;
using F64x2 = Unity.Mathematics.double2;
using F64x3 = Unity.Mathematics.double3;

using Rotor = Unity.Mathematics.quaternion;

using Bool = System.Boolean;

namespace DeathRunner.Utils
{
    public static class MathExtensions
    {
        public const F32 TAU_F32 = 2f * PI;
        public const F64 TAU_F64 = 2d * PI_DBL;
        
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
        
        
        //NOTE: [Walter] 
        //Derivative is a measure of how a function changes as its input changes. It's used to determine the rate of change of the rotation, which is needed to smoothly interpolate between the current rotation and the target rotation.
        //Specifically, the SmoothDamp function uses the derivative to calculate the current velocity of the rotation, which is then used to update the current rotation in a way that smoothly approaches the target rotation.
        //The SmoothDamp function also updates the derivative to ensure that it remains tangent to the interpolated rotation, which helps to maintain smooth and continuous motion.
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="current">The current rotation.</param>
        /// <param name="target">The target rotation.</param>
        /// <param name="derivative">The current derivative of the rotation.</param>
        /// <param name="smoothTime">Approximately the time it will take to reach the target. A smaller value will reach the target faster.</param>
        /// <returns></returns>
        public static Rotor SmoothDamp(this Rotor current, Rotor target, ref Rotor derivative, F32 smoothTime) 
        {
            // account for double-cover
            F32 __dot = dot(current, target);
            
            F32 __multi = (__dot > 0f) ? 1f : -1f;
            
            target.value *= __multi;

            // smooth damp (nlerp approx)
            F32x4 __result = normalize(SmoothDamp(current: current.value, target: target.value, currentVelocity: ref derivative.value, smoothTime: smoothTime, maxSpeed: 1f));
		
            // ensure derivative is tangent
            F32x4 __derivativeError = project(derivative.value, __result);
            derivative.value -= __derivativeError;

            return new Rotor(__result);
        }
        
        public static F32 AngleRadiansF32(this Rotor a, Rotor b)
        {
            //float num = Mathf.Min(Mathf.Abs(Quaternion.Dot(a, b)), 1f);
            //return Quaternion.IsEqualUsingDot(num) ? 0.0f : (float) ((double) Mathf.Acos(num) * 2.0 * 57.295780181884766);
            
            F32 __dot = dot(a, b);
            
            return acos(__dot) * 2.0f;
        }
        
        public static F64 AngleRadiansF64(this Rotor a, Rotor b)
        {
            F64 __dot = dot(a, b);
            
            return acos(__dot) * 2.0;
        }
        
        public static F32 AngleDegreesF32(this Rotor a, Rotor b)
        {
            return degrees(AngleRadiansF32(a, b));
        }
        
        public static F64 AngleDegreesF64(this Rotor a, Rotor b)
        {
            return degrees(AngleRadiansF64(a, b));
        }
        
        public static Rotor RotateTowards(this Rotor current, Rotor target, F32 maxRadiansDelta)
        {
            F32 __angleRadians = AngleRadiansF32(current, target);
            
            return (__angleRadians <= EPSILON) ? target : slerp(current, target, t: min(maxRadiansDelta / __angleRadians, 1.0f));
            
            // F32 __angle = angle(current, target);
            //
            // if (__angle == 0.0f) return target;
            //
            // F32 __magnitude = min(__angle / Time.deltaTime, maxMagnitudeDelta);
            //
            // return slerp(current, target, __magnitude / __angle);
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
