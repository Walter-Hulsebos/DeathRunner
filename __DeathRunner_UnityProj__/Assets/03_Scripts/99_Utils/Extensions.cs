using System;
using UnityEngine;
using Object = UnityEngine.Object;

using static Unity.Mathematics.math;

using F32  = System.Single;
using F32x3 = Unity.Mathematics.float3;

namespace DeathRunner.Utils
{
    public static class Extensions
    {
        public static Boolean TryFindObjectOfType<T>(out T result) where T : Component
        {
            result = Object.FindObjectOfType<T>();

            return (result != null);
        }
        
        public static F32x3 SmoothDamp(this F32x3 current, F32x3 target, ref F32x3 currentVelocity, F32 smoothTime, F32 maxSpeed)
        {
            F32 __deltaTime = Time.deltaTime;
            
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

            F32x3 __temp = (currentVelocity + __omega * __change) * __deltaTime;
            currentVelocity = (currentVelocity - __omega * __temp) * __exp;
            
            F32x3 __output = target + (__change + __temp) * __exp;

            // Prevent overshooting
            if (all(__originalTo - current > 0.0f == __output > __originalTo))
            {
                __output = __originalTo;
                currentVelocity = (__output - __originalTo) / __deltaTime;
            }

            return __output;
            
            // F32x3 delta = current - target;
            // F32x3 targetAdjusted = target;
            // F32x3 maxDelta = maxSpeed * smoothTime;
            //
            // if (lengthsq(delta) > lengthsq(maxDelta))
            // {
            //     delta = normalize(delta) * maxDelta;
            // }
            //
            // targetAdjusted = current - delta;
            //
            // F32x3 temp = (currentVelocity + delta / smoothTime) * deltaTime;
            // currentVelocity = (currentVelocity - temp / smoothTime) * (1 / (1 + deltaTime / smoothTime));
            // F32x3 output = targetAdjusted + (delta + temp) * (1 / (1 + deltaTime / smoothTime));
            //
            // if (dot(target - current, output - target) > 0)
            // {
            //     output = target;
            //     currentVelocity = (output - target) / deltaTime;
            // }
            //
            // return output;
        }
    }
}
