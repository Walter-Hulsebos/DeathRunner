using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;

using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;

using JetBrains.Annotations;

using F32     = System.Single;
using F32x2   = Unity.Mathematics.float2;
using F32x3   = Unity.Mathematics.float3;
using F32x3x3 = Unity.Mathematics.float3x3;

using F64     = System.Double;
using F64x2   = Unity.Mathematics.double2;
using F64x3   = Unity.Mathematics.double3;
using F64x3x3 = Unity.Mathematics.double3x3;

using I08     = System.SByte;
using I16     = System.Int16;
using I32     = System.Int32;
using I64     = System.Int64;

using U08     = System.Byte;
using U16     = System.UInt16;
using U32     = System.UInt32;
using U64     = System.UInt64;

using Bool    = System.Boolean;
using Boolx2  = Unity.Mathematics.bool2;
using Boolx3  = Unity.Mathematics.bool3;
using Boolx4  = Unity.Mathematics.bool4;

using quaternion = Unity.Mathematics.quaternion;

namespace ProjectDawn.Mathematics
{
    /// <summary>
    /// A static class to contain various math functions.
    /// </summary>
    [PublicAPI]
    public static partial class math2
    {
        /// <summary>
        /// PI multiplied by two.
        /// </summary>
        public const F32 TAU_F32 = 6.28318530718f;
        /// <summary>
        /// PI multiplied by two.
        /// </summary>
        public const F64 TAU_F64 = 6.2831853071795864769d;
        
        /// <summary> Clamps the given vector's with its length clamped to <param name="maxLength"></param>. </summary>
        /// <summary> Returns a copy of given vector with its length clamped to <param name="maxLength"></param>. </summary>
        [MethodImpl(AggressiveInlining)]
        public static F32x3 SetMaxLength(ref this F32x3 vector, F32 maxLength)
        {
            // F32 __sqrMagnitude = lengthsq(vector);
            // if (__sqrMagnitude <= (maxLength * maxLength)) return vector;
            //
            // vector = normalize(vector);
            // return vector *= maxLength;
            
            F32 __sqrMagnitude = dot(vector, vector);
            if (__sqrMagnitude <= (maxLength * maxLength)) return vector;
            
            F32   __magnitude  = sqrt(__sqrMagnitude);
            vector /= __magnitude;
            vector *= maxLength;
            
            return vector;
        }

        /// <summary> Sets the given vector's length to <param name="length"></param>. </summary>
        [MethodImpl(AggressiveInlining)]
        public static F32x3 SetLength(ref this F32x3 vector, F32 length)
        {
            vector = normalize(vector);
            return vector *= length;
        }
        
        /// <summary> Returns a copy of given vector with its length clamped to <param name="maxLength"></param>. </summary>
        [MethodImpl(AggressiveInlining)]
        public static F32x3 WithMaxLength(this F32x3 vector, F32 maxLength)
        {
            // F32 __sqrMagnitude = lengthsq(vector);
            // if (__sqrMagnitude <= (maxLength * maxLength)) return vector;
            //
            // F32x3 __normalized = normalize(vector);
            // return __normalized * maxLength;
            
            F32 __sqrMagnitude = dot(vector, vector);
            if (__sqrMagnitude <= (maxLength * maxLength)) return vector;

            F32   __magnitude  = sqrt(__sqrMagnitude);
            
            F32x3 __normalized = vector / __magnitude;
            return __normalized * maxLength;
        }
        
        // public static F32x3 WithMaxLengthSqr(this F32x3 vector, F32 maxLengthSqr)
        // {
        //     F32 __sqrMagnitude = dot(vector, vector);
        //     if (__sqrMagnitude <= maxLengthSqr) return vector;
        //
        //     F32   __magnitude  = sqrt(__sqrMagnitude);
        //     
        //     F32x3 __normalized = vector / __magnitude;
        //     return __normalized * sqrt(maxLengthSqr);
        // }
        
        /// <summary> Returns a copy of given vector with its length set to <param name="length"></param>. </summary>
        [MethodImpl(AggressiveInlining)]
        public static F32x3 WithLength(ref this F32x3 vector, F32 length)
        {
            F32x3 __normalized = normalize(vector);
            return __normalized * length;
        }

        /// <summary> Returns a copy of given vector projected onto a plane defined by a normal orthogonal to the plane. </summary>
        public static F32x3 ProjectedOnPlane(this F32x3 vector, F32x3 planeNormal)
        {
            F32x3 __projection = dot(vector, planeNormal) / dot(planeNormal, planeNormal) * planeNormal;
            return vector - __projection;
        }
        
        /// <summary> Returns a copy of given vector projected onto a plane defined by a normal orthogonal to the plane. </summary>
        public static F32x3 ProjectedOnPlane(this F32x3 vector, Plane plane)
        {
            F32x3 __projection = dot(vector, plane.normal) / dot(plane.normal, plane.normal) * plane.normal;
            return vector - __projection;
        }
        
        /// <summary> Returns a copy of given vector perpendicular to other vector. </summary>
        public static F32x3 PerpendicularTo(this F32x3 thisVector, F32x3 otherVector)
        {
            return normalize(math.cross(thisVector, otherVector));
        }

        /// <summary>
        /// Returns a copy of given vector adjusted to be tangent to a specified surface normal relatively to given up axis.
        /// </summary>
        public static F32x3 TangentTo(this F32x3 vector, F32x3 normal, F32x3 up)
        {
            F32x3 __r = vector.PerpendicularTo(up);
            F32x3 __t = normal.PerpendicularTo(__r);

            return __t * length(vector);
        }

        /// <summary>
        /// Transforms a vector to be relative to given transform.
        /// If isPlanar == true, the transform will be applied on the plane defined by world up axis.
        /// </summary>
        public static F32x3 RelativeTo(this F32x3 vector, Transform relativeToThis, Bool isPlanar = true)
        {
            return RelativeTo(vector: vector, relativeToThis: relativeToThis, upAxis: up(), isPlanar: isPlanar);
        }
        public static F32x3 InverseRelativeTo(this F32x3 vector, Transform relativeToThis, Bool isPlanar = true)
        {
            return InverseRelativeTo(vector: vector, relativeToThis: relativeToThis, upAxis: up(), isPlanar: isPlanar);
        }
        

        public static F32x3 RelativeToPlanar(this F32x3 vector, F32x3 relativeForward, F32x3 relativeUp, F32x3 upAxis)
        {
            relativeForward = relativeForward.ProjectedOnPlane(upAxis);

            if (all(relativeForward == F32x3.zero))
            {
                relativeForward = relativeUp.ProjectedOnPlane(upAxis);
            }
            
            quaternion __rotor = quaternion.LookRotation(forward: relativeForward, up: upAxis);
            return mul(__rotor, vector);
        }
        public static F32x3 InverseRelativeToPlanar(this F32x3 vector, F32x3 relativeForward, F32x3 relativeUp, F32x3 upAxis)
        {
            relativeForward = relativeForward.ProjectedOnPlane(upAxis);

            if (all(relativeForward == F32x3.zero))
            {
                relativeForward = relativeUp.ProjectedOnPlane(upAxis);
            }

            quaternion __rotor    = quaternion.LookRotation(forward: relativeForward, up: upAxis);
            quaternion __invRotor = inverse(__rotor);
            return mul(__invRotor, vector);
        }
        
        
        public static F32x3 RelativeToNonPlanar(this F32x3 vector, F32x3 relativeForward, F32x3 upAxis)
        {
            quaternion __rotor = quaternion.LookRotation(forward: relativeForward, up: upAxis);
            return mul(__rotor, vector);
        }
        public static F32x3 InverseRelativeToNonPlanar(this F32x3 vector, F32x3 relativeForward, F32x3 upAxis)
        {
            quaternion __rotor    = quaternion.LookRotation(forward: relativeForward, up: upAxis);
            quaternion __invRotor = inverse(__rotor);
            return mul(__invRotor, vector);
        }

        
        /// <summary>
        /// Transforms a vector to be relative to given transform.
        /// If isPlanar == true, the transform will be applied on the plane defined by upAxis.
        /// </summary>
        public static F32x3 RelativeTo(this F32x3 vector, Transform relativeToThis, F32x3 upAxis, Bool isPlanar = true)
        {
            if (isPlanar)
            {
                F32x3 __relativeForward = relativeToThis.forward;
                F32x3 __relativeUp      = relativeToThis.up;
                return RelativeToPlanar(vector: vector, relativeForward: __relativeForward, relativeUp: __relativeUp, upAxis: upAxis);
            }
            else
            {
                F32x3 __relativeForward = relativeToThis.forward;
                return RelativeToNonPlanar(vector: vector, relativeForward: __relativeForward, upAxis: upAxis);
            }
        }
        public static F32x3 InverseRelativeTo(this F32x3 vector, Transform relativeToThis, F32x3 upAxis, Bool isPlanar = true)
        {
            if (isPlanar)
            {
                F32x3 __relativeForward = relativeToThis.forward;
                F32x3 __relativeUp      = relativeToThis.up;
                return InverseRelativeToPlanar(vector: vector, relativeForward: __relativeForward, relativeUp: __relativeUp, upAxis: upAxis);
            }
            else
            {
                F32x3 __relativeForward = relativeToThis.forward;
                return InverseRelativeToNonPlanar(vector: vector, relativeForward: __relativeForward, upAxis: upAxis);
            }
        }
        
        // public static F32x3 DirectionRelativeToOtherDirection(this F32x3 direction, F32x3 relativeTo)
        // {
        //     var __rotor = quaternion.FromToRotation(from: relativeTo, to: direction);
        // }

        /// <summary>
        /// Returns cross product of two vectors.
        /// </summary>
        [MethodImpl(AggressiveInlining)]
        public static F32x2 cross(F32x2 a, F32x2 b) => new F32x2(a.x * b.y,  -(a.y * b.x));
        /// <summary>
        /// Returns cross product of two vectors.
        /// </summary>
        [MethodImpl(AggressiveInlining)]
        public static F64x2 cross(F64x2 a, F64x2 b) => new F64x2(a.x * b.y,  -(a.y * b.x));

        /// <summary>
        /// Returns determinant of two vectors.
        /// Sum of cross product elements.
        /// </summary>
        [MethodImpl(AggressiveInlining)]
        public static F32 determinant(F32x2 a, F32x2 b) => a.x * b.y - a.y * b.x;
        /// <summary>
        /// Returns determinant of two vectors.
        /// Sum of cross product elements.
        /// </summary>
        [MethodImpl(AggressiveInlining)]
        public static F64 determinant(F64x2 a, F64x2 b) => a.x * b.y - a.y * b.x;

        /// <summary>
        /// Returns determinant of two vectors.
        /// Sum of cross product elements.
        /// </summary>
        [MethodImpl(AggressiveInlining)]
        public static F32 determinant(F32x3 a, F32x3 b)
        {
            return ((a.y * b.z) - (a.z * b.y)) - ((a.z * b.x) - (a.x * b.z)) + ((a.x * b.y) - (a.y * b.x));
        }
        /// <summary>
        /// Returns determinant of two vectors.
        /// Sum of cross product elements.
        /// </summary>
        [MethodImpl(AggressiveInlining)]
        public static F64 determinant(F64x3 a, F64x3 b)
        {
            return ((a.y * b.z) - (a.z * b.y)) - ((a.z * b.x) - (a.x * b.z)) + ((a.x * b.y) - (a.y * b.x));
        }

        /// <summary>
        /// Returns true if points ordered counter clockwise.
        /// </summary>
        [MethodImpl(AggressiveInlining)]
        public static Bool iscclockwise(F32x2 a, F32x2 b, F32x2 c) => determinant(c - a, b - a) < 0;
        /// <summary>
        /// Returns true if points ordered counter clockwise.
        /// </summary>
        [MethodImpl(AggressiveInlining)]
        public static Bool iscclockwise(F32x3 a, F32x3 b, F32x3 c) => determinant(c - a, b - a) < 0;
        
        /// <summary>
        /// Returns true if points ordered counter clockwise.
        /// </summary>
        [MethodImpl(AggressiveInlining)]
        public static Bool iscclockwise(F64x2 a, F64x2 b, F64x2 c) => determinant(c - a, b - a) < 0;
        /// <summary>
        /// Returns true if points ordered counter clockwise.
        /// </summary>
        [MethodImpl(AggressiveInlining)]
        public static Bool iscclockwise(F64x3 a, F64x3 b, F64x3 c) => determinant(c - a, b - a) < 0;
        
        /// <summary>
        /// Returns true if points ordered clockwise.
        /// </summary>
        [MethodImpl(AggressiveInlining)]
        public static Bool isclockwise(F32x2 a, F32x2 b, F32x2 c) => determinant(c - a, b - a) > 0;
        /// <summary>
        /// Returns true if points ordered clockwise.
        /// </summary>
        [MethodImpl(AggressiveInlining)]
        public static Bool isclockwise(F32x3 a, F32x3 b, F32x3 c) => determinant(c - a, b - a) > 0;
        
        /// <summary>
        /// Returns true if points ordered clockwise.
        /// </summary>
        [MethodImpl(AggressiveInlining)]
        public static Bool isclockwise(F64x2 a, F64x2 b, F64x2 c) => determinant(c - a, b - a) > 0;
        /// <summary>
        /// Returns true if points ordered clockwise.
        /// </summary>
        [MethodImpl(AggressiveInlining)]
        public static Bool isclockwise(F64x3 a, F64x3 b, F64x3 c) => determinant(c - a, b - a) > 0;


        /// <summary>
        /// Returns true if valid triangle exists knowing three edge lengths.
        /// </summary>
        [MethodImpl(AggressiveInlining)]
        public static Bool istriangle(F32 a, F32 b, F32 c)
        {
            // Sum of two triangle edge is always lower than third
            return all(new Boolx3(
                a + b > c,
                a + c > b,
                b + c > a));
        }
        /// <summary>
        /// Returns true if valid triangle exists knowing three edge lengths.
        /// </summary>
        [MethodImpl(AggressiveInlining)]
        public static Bool istriangle(F64 a, F64 b, F64 c)
        {
            // Sum of two triangle edge is always lower than third
            return all(new Boolx3(
                a + b > c,
                a + c > b,
                b + c > a));
        }

        /// <summary>
        /// Returns if quad meets the Delaunay condition. Where a, b, c forms clockwise sorted triangle.
        /// Based on https://en.wikipedia.org/wiki/Delaunay_triangulation.
        /// </summary>
        [MethodImpl(AggressiveInlining)]
        public static Bool isdelaunay(F32x2 a, F32x2 b, F32x2 c, F32x2 d)
        {
            F32x2 ad = a - d;
            F32x2 bd = b - d;
            F32x2 cd = c - d;

            F32x2 d2 = d * d;

            F32x2 ad2 = a * a - d2;
            F32x2 bd2 = b * b - d2;
            F32x2 cd2 = c * c - d2;

            F32 determinant = math.determinant(new F32x3x3(
                new F32x3(ad.x, ad.y, ad2.x + ad2.y),
                new F32x3(bd.x, bd.y, bd2.x + bd2.y),
                new F32x3(cd.x, cd.y, cd2.x + cd2.y)
                ));

            return determinant >= 0;
        }
        /// <summary>
        /// Returns if quad meets the Delaunay condition. Where a, b, c forms clockwise sorted triangle.
        /// Based on https://en.wikipedia.org/wiki/Delaunay_triangulation.
        /// </summary>
        [MethodImpl(AggressiveInlining)]
        public static Bool isdelaunay(F64x2 a, F64x2 b, F64x2 c, F64x2 d)
        {
            F64x2 ad = a - d;
            F64x2 bd = b - d;
            F64x2 cd = c - d;

            F64x2 d2 = d * d;

            F64x2 ad2 = a * a - d2;
            F64x2 bd2 = b * b - d2;
            F64x2 cd2 = c * c - d2;

            F64 determinant = math.determinant(new F64x3x3(
                new F64x3(ad.x, ad.y, ad2.x + ad2.y),
                new F64x3(bd.x, bd.y, bd2.x + bd2.y),
                new F64x3(cd.x, cd.y, cd2.x + cd2.y)
            ));

            return determinant >= 0;
        }

        /// <summary>
        /// Returns factorial of the value (etc 0! = 1, 1! = 1, 2! = 2, 3! = 6, 4! = 24 ...)
        /// Based on https://en.wikipedia.org/wiki/Factorial.
        /// </summary>
        [MethodImpl(AggressiveInlining)]
        public static I32 factorial(I32 value)
        {
            I32 factorial = 1;
            I32 count = value + 1;
            for (I32 i = 1; i < count; ++i)
                factorial *= i;
            return factorial;
        }

        /// <summary>
        /// Exchanges the values of a and b.
        /// </summary>
        [MethodImpl(AggressiveInlining)]
        public static void swap<T>(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }
        
        [MethodImpl(AggressiveInlining)]
        public static U08 min(U08 a, U08 b) => a < b ? a : b;
        [MethodImpl(AggressiveInlining)]
        public static U16 min(U16 a, U16 b) => a < b ? a : b;
        
        [MethodImpl(AggressiveInlining)]
        public static U08 max(U08 a, U08 b) => a > b ? a : b;
        [MethodImpl(AggressiveInlining)]
        public static U16 max(U16 a, U16 b) => a > b ? a : b;
        
        [MethodImpl(AggressiveInlining)]
        public static U08 clamp(U08 target, U08 a, U08 b) => max(a, min(b, target));
        [MethodImpl(AggressiveInlining)]
        public static U16 clamp(U16 target, U16 a, U16 b) => max(a, min(b, target));
    

        
        
    }
}
