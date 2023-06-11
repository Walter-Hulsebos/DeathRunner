#if !UNITY_DOTSPLAYER
using UnityEngine;

namespace ProjectDawn.Geometry2D
{
    public partial struct Rectangle
    {
        public static implicit operator Rect(Rectangle v) => new(v.Position, v.Size);
        public static implicit operator Rectangle(Rect v) => new(v.position, v.size);
    }
}
#endif