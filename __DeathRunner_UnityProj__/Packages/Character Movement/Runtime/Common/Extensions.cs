using UnityEngine;

using static Unity.Mathematics.math;

using F32   = System.Single;
using F32x2 = Unity.Mathematics.float2;
using F32x3 = Unity.Mathematics.float3;
using F32x4 = Unity.Mathematics.float4;

using I32   = System.Int32;
using I32x2 = Unity.Mathematics.int2;
using I32x3 = Unity.Mathematics.int3;
using I32x4 = Unity.Mathematics.int4;

using Rotor = Unity.Mathematics.quaternion;

using Bool  = System.Boolean;

namespace EasyCharacterMovement
{
    public static class Extensions
    {
        /// <summary>
        /// Return the square of the given value.
        /// </summary>

        public static int Square(this int value)
        {
            return value * value;
        }

        /// <summary>
        /// Return the square of the given value.
        /// </summary>

        public static F32 Square(this F32 value)
        {
            return value * value;
        }

        /// <summary>
        /// Returns a copy of given vector with only X component of the vector.
        /// </summary>

        public static F32x3 OnlyX(this F32x3 vec)
        {
            vec.y = 0f;
            vec.z = 0f;

            return vec;
        }

        /// <summary>
        /// Returns a copy of given vector with only Y component of the vector.
        /// </summary>

        public static F32x3 OnlyY(this F32x3 vec)
        {
            vec.x = 0f;
            vec.z = 0f;

            return vec;
        }

        /// <summary>
        /// Returns a copy of given vector with only Z component of the vector.
        /// </summary>

        public static F32x3 OnlyZ(this F32x3 vec)
        {
            vec.x = 0.0f;
            vec.y = 0.0f;

            return vec;
        }

        /// <summary>
        /// Returns a copy of given vector with only X and Z components of the vector.
        /// </summary>

        public static F32x3 OnlyXZ(this F32x3 vec)
        {
            vec.y = 0.0f;

            return vec;
        }

        /// <summary>
        /// Checks whether value is near to zero within a tolerance.
        /// </summary>
        public static bool IsZero(this F32 value)
        {
            const F32 K_TOLERANCE = 0.0000000001f;

            return abs(value) < K_TOLERANCE;
        }
        /// <summary>
        /// Checks whether vector is near to zero within a tolerance.
        /// </summary>
        public static bool IsZero(this F32x2 vector2)
        {
            return lengthsq(vector2) < 9.99999943962493E-11;
        }

        /// <summary>
        /// Checks whether vector is near to zero within a tolerance.
        /// </summary>
        public static bool IsZero(this F32x3 vec)
        {
            return lengthsq(vec) < 9.99999943962493E-11;
        }

        /// <summary>
        /// Checks whether vector is exceeding the magnitude within a small error tolerance.
        /// </summary>
        public static bool IsExceeding(this F32x3 vec, F32 magnitude)
        {
            // Allow 1% error tolerance, to account for numeric imprecision.

            const F32 K_ERROR_TOLERANCE = 1.01f;

            return lengthsq(vec) > magnitude * magnitude * K_ERROR_TOLERANCE;
        }

        /// <summary>
        /// Returns a copy of given vector with a magnitude of 1,
        /// and outs its magnitude before normalization.
        /// 
        /// If the vector is too small to be normalized a zero vector will be returned.
        /// </summary>

        public static F32x3 Normalized(this F32x3 vec, out F32 magnitude)
        {
            magnitude = length(vec);
            if (magnitude > 9.99999974737875E-06)
                return vec / magnitude;

            magnitude = 0.0f;

            return F32x3.zero;
        }
        
        // public static F32 dot(this F32x3 vec, F32x3 otherF32x3)
        // {
        //     return dot(vec, otherF32x3);
        // }

        /// <summary>
        /// Returns a copy of given vector projected onto normal vector.
        /// </summary>

        public static F32x3 projectedOn(this F32x3 vec, F32x3 normal)
        {
            //return F32x3.Project(thisVector, normal);
            
            return dot(vec, normal) * normal;
        }

        /// <summary>
        /// Returns a copy of given vector projected onto a plane defined by a normal orthogonal to the plane.
        /// </summary>

        public static F32x3 projectedOnPlane(this F32x3 vec, F32x3 planeNormal)
        {
            F32x3 __orthogonal = planeNormal * dot(vec, planeNormal);
            return vec - __orthogonal;
        }

        /// <summary>
        /// Returns a copy of given vector with its magnitude clamped to maxLength.
        /// </summary>
        public static F32x3 ClampedTo(this F32x3 vec, F32 maxLength)
        {
            //return F32x3.ClampMagnitude(vec, maxLength);
            
            F32 __lengthScale = length(vec) / maxLength;
            if (__lengthScale > 1f)
            {
                vec *= (1f / __lengthScale);
            }
            
            return vec;
        }

        /// <summary>
        /// Returns a copy of given vector perpendicular to other vector.
        /// </summary>

        public static F32x3 PerpendicularTo(this F32x3 thisVector, F32x3 otherVector)
        {
            //return F32x3.Cross(thisVector, otherVector).normalized;
            
            return normalize(cross(thisVector, otherVector));
        }

        /// <summary>
        /// Returns a copy of given vector adjusted to be tangent to a specified surface normal relatively to given up axis.
        /// </summary>

        public static F32x3 TangentTo(this F32x3 thisVector, F32x3 normal, F32x3 up)
        {
            F32x3 __r = thisVector.PerpendicularTo(up);
            F32x3 __t = normal.PerpendicularTo(__r);

            return __t * length(thisVector);
        }

        /// <summary>
        /// Transforms a vector to be relative to given transform.
        /// If isPlanar == true, the transform will be applied on the plane defined by world up axis.
        /// </summary>

        public static F32x3 RelativeTo(this F32x3 vec, Transform relativeToThis, bool isPlanar = true)
        {
            F32x3 __forward = relativeToThis.forward;

            if (isPlanar)
            {
                F32x3 __upAxis = up();
                __forward = __forward.projectedOnPlane(__upAxis);

                if (__forward.IsZero())
                {
                    __forward = ((F32x3)relativeToThis.up).projectedOnPlane(__upAxis);
                }
            }
            
            Rotor __q = Rotor.LookRotation(__forward, up: up());

            return mul(__q, vec);
        }

        /// <summary>
        /// Transforms a vector to be relative to given transform.
        /// If isPlanar == true, the transform will be applied on the plane defined by upAxis.
        /// </summary>

        public static F32x3 RelativeTo(this F32x3 vec, Transform relativeToThis, F32x3 upAxis, bool isPlanar = true)
        {
            F32x3 __forward = relativeToThis.forward;

            if (isPlanar)
            {
                __forward = __forward.projectedOnPlane(upAxis);

                if (__forward.IsZero())
                {
                    __forward = ((F32x3)relativeToThis.up).projectedOnPlane(upAxis);
                }
            }

            Rotor __q = Rotor.LookRotation(__forward, upAxis);

            return mul(__q, vec);
        }

        /// <summary>
        /// Clamps the given quaternion pitch rotation between the given minPitchAngle and maxPitchAngle.
        /// </summary>
        public static Rotor ClampPitch(this Rotor quaternion, F32 minPitchAngle, F32 maxPitchAngle)
        {
            quaternion.value.x /= quaternion.value.w;
            quaternion.value.y /= quaternion.value.w;
            quaternion.value.z /= quaternion.value.w;
            quaternion.value.w = 1.0f;

            F32 __pitch = clamp(2.0f * Mathf.Rad2Deg * atan(quaternion.value.x), minPitchAngle, maxPitchAngle);

            quaternion.value.x = tan(__pitch * 0.5f * Mathf.Deg2Rad);

            return quaternion;
        }
        
        /// <summary>
        ///   <para>Rotates a rotation from towards to.</para>
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="maxDegreesDelta"></param>
        public static Rotor RotateTowards(this Rotor from, Rotor to, float maxDegreesDelta)
        {
            float num = angleDegrees(from, to);
            
            return (num == 0.0) ? to : slerp(from, to, min(1f, maxDegreesDelta / num));
        }
        
        public static F32 angleDegrees(this Rotor from, Rotor to)
        {
            return degrees(angleRadians(from, to));
        }
        
        public static F32 angleRadians(this Rotor from, Rotor to)
        {
            return acos(dot(normalize(from), normalize(to)));
        }
        
        public static F32x3 eulerAnglesDegrees(this Rotor q)
        {
            return degrees(eulerAnglesRadians(q));
        }
        
        public static F32x3 eulerAnglesRadians(this Rotor q)
        {
            F32x3 eulerAngles;

            F32 test = q.value.x * q.value.y + q.value.z * q.value.w;

            if (test > 0.499f)
            {
                // Singularity at north pole
                eulerAngles.y = 2f * atan2(q.value.x, q.value.w);
                eulerAngles.x = PI / 2f;
                eulerAngles.z = 0f;
            }
            else if (test < -0.499f)
            {
                // Singularity at south pole
                eulerAngles.y = -2f * atan2(q.value.x, q.value.w);
                eulerAngles.x = -PI / 2f;
                eulerAngles.z = 0f;
            }
            else
            {
                F32 sqx = q.value.x * q.value.x;
                F32 sqy = q.value.y * q.value.y;
                F32 sqz = q.value.z * q.value.z;

                eulerAngles.y = atan2(2f * q.value.y * q.value.w - 2f * q.value.x * q.value.z, 1f - 2f * sqy - 2f * sqz);
                eulerAngles.x = asin(2f * test);
                eulerAngles.z = atan2(2f * q.value.x * q.value.w - 2f * q.value.y * q.value.z, 1f - 2f * sqx - 2f * sqz);
            }

            return eulerAngles;
        }
    }
}