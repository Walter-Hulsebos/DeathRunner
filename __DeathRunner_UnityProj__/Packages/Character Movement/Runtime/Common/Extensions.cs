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

        public static I32 Square(this I32 value)
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
        /// Returns a copy of given vector with only X component of the vector.
        /// </summary>
        public static Vector3 OnlyX(this Vector3 vec)
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
        /// Returns a copy of given vector with only Y component of the vector.
        /// </summary>
        public static Vector3 OnlyY(this Vector3 vec)
        {
            vec.x = 0.0f;
            vec.z = 0.0f;

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
        /// Returns a copy of given vector with only Z component of the vector.
        /// </summary>
        public static Vector3 onlyZ(this Vector3 vec)
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
        /// Returns a copy of given vector with only X and Z components of the vector.
        /// </summary>
        public static Vector3 OnlyXZ(this Vector3 vec)
        {
            vec.y = 0f;

            return vec;
        }

        /// <summary>
        /// Checks whether value is near to zero within a tolerance.
        /// </summary>
        public static Bool IsZero(this F32 value)
        {
            const F32 K_TOLERANCE = 0.0000000001f;

            return abs(value) < K_TOLERANCE;
        }
        
        
        /// <summary>
        /// Checks whether vector is near to zero within a tolerance.
        /// </summary>
        public static Bool IsZero(this F32x2 vec)
        {
            return lengthsq(vec) < 9.99999943962493E-11;
        }
        /// <summary>
        /// Checks whether vector is near to zero within a tolerance.
        /// </summary>
        public static bool IsZero(this Vector2 vec)
        {
            return vec.sqrMagnitude < 9.99999943962493E-11;
        }
        
        /// <summary>
        /// Checks whether vector is near to zero within a tolerance.
        /// </summary>
        public static Bool IsZero(this F32x3 vec)
        {
            return lengthsq(vec) < 9.99999943962493E-11;
        }
        /// <summary>
        /// Checks whether vector is near to zero within a tolerance.
        /// </summary>
        public static Bool IsZero(this Vector3 vec)
        {
            return vec.sqrMagnitude < 9.99999943962493E-11;
        }

        /// <summary>
        /// Checks whether vector is exceeding the magnitude within a small error tolerance.
        /// </summary>
        public static Bool IsExceeding(this F32x3 vec, F32 magnitude)
        {
            // Allow 1% error tolerance, to account for numeric imprecision.

            const F32 K_ERROR_TOLERANCE = 1.01f;

            return lengthsq(vec) > (magnitude * magnitude) * K_ERROR_TOLERANCE;
        }
        /// <summary>
        /// Checks whether vector is exceeding the magnitude within a small error tolerance.
        /// </summary>
        public static Bool IsExceeding(this Vector3 vec, F32 magnitude)
        {
            // Allow 1% error tolerance, to account for numeric imprecision.
            const F32 K_ERROR_TOLERANCE = 1.01f;

            return vec.sqrMagnitude > (magnitude * magnitude) * K_ERROR_TOLERANCE;
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
            {
                return vec / magnitude;
            }

            magnitude = 0.0f;

            return F32x3.zero;
        }
        /// <summary>
        /// Returns a copy of given vector with a magnitude of 1,
        /// and outs its magnitude before normalization.
        /// 
        /// If the vector is too small to be normalized a zero vector will be returned.
        /// </summary>

        public static Vector3 Normalized(this Vector3 vec, out F32 magnitude)
        {
            magnitude = vec.magnitude;
            if (magnitude > 9.99999974737875E-06)
            {
                return vec / magnitude;
            }

            magnitude = 0.0f;

            return Vector3.zero;
        }
        
        // public static F32 dot(this F32x3 vec, F32x3 otherF32x3)
        // {
        //     return dot(vec, otherF32x3);
        // }

        /// <summary>
        /// Returns a copy of given vector projected onto normal vector.
        /// </summary>

        public static F32x3 ProjectedOn(this F32x3 vec, F32x3 normal)
        {
            //return dot(vec, normal) * normal;
            F32 normalLengthSq = lengthsq(normal);
            
            if (normalLengthSq <= EPSILON) return F32x3.zero;
            
            return (dot(vec, normal) * normal) / normalLengthSq;

        }
        /// <summary>
        /// Returns a copy of given vector projected onto normal vector.
        /// </summary>
        public static Vector3 ProjectedOn(this Vector3 vec, Vector3 normal)
        {
            F32 normalLengthSq = Vector3.Dot(normal, normal);
            
            if (normalLengthSq <= Mathf.Epsilon) return Vector3.zero;

            F32 num2 = Vector3.Dot(vec, normal);
            return new Vector3(normal.x * num2 / normalLengthSq, normal.y * num2 / normalLengthSq, normal.z * num2 / normalLengthSq);
        }


        /// <summary>
        /// Returns a copy of given vector projected onto a plane defined by a normal orthogonal to the plane.
        /// </summary>
        public static F32x3 ProjectedOnPlane(this F32x3 vec, F32x3 planeNormal)
        {
            //F32x3 __orthogonal = planeNormal * dot(vec, planeNormal);
            //return vec - __orthogonal;
            
            F32 normalLengthSq = lengthsq(planeNormal);
            
            if (normalLengthSq <= EPSILON) return vec;

            F32x3 __orthogonal = (planeNormal * dot(vec, planeNormal)) / normalLengthSq;
            return vec - __orthogonal;
        }
        /// <summary>
        /// Returns a copy of given vector projected onto a plane defined by a normal orthogonal to the plane.
        /// </summary>
        public static Vector3 ProjectedOnPlane(this Vector3 vec, Vector3 planeNormal)
        {
            //return Vector3.ProjectOnPlane(vec, planeNormal);
            
            F32 normalLengthSq = Vector3.Dot(planeNormal, planeNormal);
            
            if (normalLengthSq <= Mathf.Epsilon) return vec;
            
            F32 num2 = Vector3.Dot(vec, planeNormal);
            return new Vector3(vec.x - planeNormal.x * num2 / normalLengthSq, vec.y - planeNormal.y * num2 / normalLengthSq, vec.z - planeNormal.z * num2 / normalLengthSq);
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
        /// Returns a copy of given vector with its magnitude clamped to maxLength.
        /// </summary>
        public static Vector3 ClampedTo(this Vector3 vec, F32 maxLength)
        {
            return Vector3.ClampMagnitude(vec, maxLength);
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
        /// Returns a copy of given vector perpendicular to other vector.
        /// </summary>
        public static Vector3 PerpendicularTo(this Vector3 thisVector, Vector3 otherVector)
        {
            return Vector3.Cross(thisVector, otherVector).normalized;
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
        /// Returns a copy of given vector adjusted to be tangent to a specified surface normal relatively to given up axis.
        /// </summary>
        public static Vector3 TangentTo(this Vector3 thisVector, Vector3 normal, Vector3 up)
        {
            Vector3 __r = thisVector.PerpendicularTo(up);
            Vector3 __t = normal.PerpendicularTo(__r);

            return __t * thisVector.magnitude;
        }

        /// <summary>
        /// Transforms a vector to be relative to given transform.
        /// If isPlanar == true, the transform will be applied on the plane defined by world up axis.
        /// </summary>
        // public static F32x3 RelativeTo(this F32x3 vec, Transform relativeToThis, Bool isPlanar = true)
        // {
        //     F32x3 __forward = relativeToThis.forward;
        //
        //     if (isPlanar)
        //     {
        //         F32x3 __upAxis = up();
        //         __forward = __forward.ProjectedOnPlane(__upAxis);
        //
        //         if (__forward.IsZero())
        //         {
        //             __forward = ((F32x3)relativeToThis.up).ProjectedOnPlane(__upAxis);
        //         }
        //     }
        //     
        //     Rotor __q = Rotor.LookRotation(forward: __forward, up: up());
        //     return mul(__q, vec);
        // }
        /// <summary>
        /// Transforms a vector to be relative to given transform.
        /// If isPlanar == true, the transform will be applied on the plane defined by world up axis.
        /// </summary>
        public static Vector3 RelativeTo(this Vector3 vector3, Transform relativeToThis, Bool isPlanar = true)
        {
            Vector3 __forward = relativeToThis.forward;

            if (isPlanar)
            {
                Vector3 __upAxis = Vector3.up;
                __forward = __forward.ProjectedOnPlane(__upAxis);

                if (__forward.IsZero())
                {
                    __forward = Vector3.ProjectOnPlane(relativeToThis.up, __upAxis);
                }
            }
            
            Quaternion __q = Quaternion.LookRotation(forward: __forward, upwards: Vector3.up);
            return __q * vector3;
        }


        /// <summary>
        /// Transforms a vector to be relative to given transform.
        /// If isPlanar == true, the transform will be applied on the plane defined by upAxis.
        /// </summary>
        public static F32x3 RelativeTo(this F32x3 vec, Transform relativeToThis, F32x3 upAxis, Bool isPlanar = true)
        {
            F32x3 __forward = relativeToThis.forward;

            if (isPlanar)
            {
                __forward = __forward.ProjectedOnPlane(upAxis);

                if (__forward.IsZero())
                {
                    __forward = ((F32x3)relativeToThis.up).ProjectedOnPlane(upAxis);
                }
            }

            Rotor __q = Rotor.LookRotation(__forward, upAxis);
            return mul(__q, vec);
        }
        /// <summary>
        /// Transforms a vector to be relative to given transform.
        /// If isPlanar == true, the transform will be applied on the plane defined by upAxis.
        /// </summary>
        public static Vector3 RelativeTo(this Vector3 vector3, Transform relativeToThis, Vector3 upAxis, Bool isPlanar = true)
        {
            Vector3 __forward = relativeToThis.forward;

            if (isPlanar)
            {
                __forward = Vector3.ProjectOnPlane(__forward, upAxis);

                if (__forward.IsZero())
                {
                    __forward = Vector3.ProjectOnPlane(relativeToThis.up, upAxis);
                }
            }

            Quaternion __q = Quaternion.LookRotation(__forward, upAxis);
            return __q * vector3;
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
        /// Clamps the given quaternion pitch rotation between the given minPitchAngle and maxPitchAngle.
        /// </summary>

        public static Quaternion ClampPitch(this Quaternion quaternion, F32 minPitchAngle, F32 maxPitchAngle)
        {
            quaternion.x /= quaternion.w;
            quaternion.y /= quaternion.w;
            quaternion.z /= quaternion.w;
            quaternion.w = 1.0f;

            F32 __pitch = Mathf.Clamp(2.0f * Mathf.Rad2Deg * Mathf.Atan(quaternion.x), minPitchAngle, maxPitchAngle);

            quaternion.x = Mathf.Tan(__pitch * 0.5f * Mathf.Deg2Rad);

            return quaternion;
        }
        
        /// <summary>
        ///   <para>Rotates a rotation from towards to.</para>
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="maxDegreesDelta"></param>
        public static Rotor RotateTowards(this Rotor from, Rotor to, F32 maxDegreesDelta)
        {
            F32 num = angleDegrees(from, to);
            
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