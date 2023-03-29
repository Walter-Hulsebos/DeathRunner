using UnityEngine;
using static Unity.Mathematics.math;

using F32   = System.Single;
using F32x3 = Unity.Mathematics.float3;

using Bool = System.Boolean;

namespace DeathRunner.Utils
{
    public static class MathExtensions
    {
        public static F32x3 SmoothDamp(this F32x3 current, F32x3 target, ref F32x3 currentVelocity, F32 deltaTime, F32 smoothTime, F32 maxSpeed)
        {
            //F32 __deltaTime = Time.deltaTime;
            
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
    }
}
