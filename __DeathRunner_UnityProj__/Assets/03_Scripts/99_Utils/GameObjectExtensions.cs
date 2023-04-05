using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;

using UnityEngine;

using Bool = System.Boolean;

using F32 = System.Single;

using I32 = System.Int32;

namespace DeathRunner.Utils
{
    public static class GameObjectExtensions
    {
        [MethodImpl(AggressiveInlining)]
        public static GameObject AddComponents<T>(this GameObject gameObject, out T c1)
            where T : Component
        {
            c1 = gameObject.AddComponent<T>();
            
            return gameObject;
        }
        
        [MethodImpl(AggressiveInlining)]
        public static GameObject AddComponents<T1, T2>(this GameObject gameObject, out T1 c1, out T2 c2) 
            where T1 : Component 
            where T2 : Component
        {
            c1 = gameObject.AddComponent<T1>();
            c2 = gameObject.AddComponent<T2>();
            
            return gameObject;
        }
        
        [MethodImpl(AggressiveInlining)]
        public static GameObject AddComponents<T1, T2, T3>(this GameObject gameObject, out T1 c1, out T2 c2, out T3 c3) 
            where T1 : Component 
            where T2 : Component 
            where T3 : Component
        {
            c1 = gameObject.AddComponent<T1>();
            c2 = gameObject.AddComponent<T2>();
            c3 = gameObject.AddComponent<T3>();
            
            return gameObject;
        }
        
        [MethodImpl(AggressiveInlining)]
        public static GameObject AddComponents<T1, T2, T3, T4>(this GameObject gameObject, out T1 c1, out T2 c2, out T3 c3, out T4 c4) 
            where T1 : Component 
            where T2 : Component 
            where T3 : Component 
            where T4 : Component
        {
            c1 = gameObject.AddComponent<T1>();
            c2 = gameObject.AddComponent<T2>();
            c3 = gameObject.AddComponent<T3>();
            c4 = gameObject.AddComponent<T4>();
            
            return gameObject;
        }
        
        [MethodImpl(AggressiveInlining)]
        public static GameObject AddComponents<T1, T2, T3, T4, T5>(this GameObject gameObject, out T1 c1, out T2 c2, out T3 c3, out T4 c4, out T5 c5) 
            where T1 : Component 
            where T2 : Component 
            where T3 : Component 
            where T4 : Component 
            where T5 : Component
        {
            c1 = gameObject.AddComponent<T1>();
            c2 = gameObject.AddComponent<T2>();
            c3 = gameObject.AddComponent<T3>();
            c4 = gameObject.AddComponent<T4>();
            c5 = gameObject.AddComponent<T5>();
            
            return gameObject;
        }
        
        [MethodImpl(AggressiveInlining)]
        public static GameObject AddComponents<T1, T2, T3, T4, T5, T6>(this GameObject gameObject, out T1 c1, out T2 c2, out T3 c3, out T4 c4, out T5 c5, out T6 c6) 
            where T1 : Component 
            where T2 : Component 
            where T3 : Component 
            where T4 : Component 
            where T5 : Component 
            where T6 : Component
        {
            c1 = gameObject.AddComponent<T1>();
            c2 = gameObject.AddComponent<T2>();
            c3 = gameObject.AddComponent<T3>();
            c4 = gameObject.AddComponent<T4>();
            c5 = gameObject.AddComponent<T5>();
            c6 = gameObject.AddComponent<T6>();
            
            return gameObject;
        }

        [MethodImpl(AggressiveInlining)]
        public static GameObject AddComponents<T1, T2, T3, T4, T5, T6, T7>(this GameObject gameObject, out T1 c1, out T2 c2, out T3 c3, out T4 c4, out T5 c5, out T6 c6, out T7 c7)
            where T1 : Component
            where T2 : Component
            where T3 : Component
            where T4 : Component
            where T5 : Component
            where T6 : Component
            where T7 : Component
        {
            c1 = gameObject.AddComponent<T1>();
            c2 = gameObject.AddComponent<T2>();
            c3 = gameObject.AddComponent<T3>();
            c4 = gameObject.AddComponent<T4>();
            c5 = gameObject.AddComponent<T5>();
            c6 = gameObject.AddComponent<T6>();
            c7 = gameObject.AddComponent<T7>();
            
            return gameObject;
        }
        
        [MethodImpl(AggressiveInlining)]
        public static GameObject AddComponents<T1, T2, T3, T4, T5, T6, T7, T8>(this GameObject gameObject, out T1 c1, out T2 c2, out T3 c3, out T4 c4, out T5 c5, out T6 c6, out T7 c7, out T8 c8)
            where T1 : Component
            where T2 : Component
            where T3 : Component
            where T4 : Component
            where T5 : Component
            where T6 : Component
            where T7 : Component
            where T8 : Component
        {
            c1 = gameObject.AddComponent<T1>();
            c2 = gameObject.AddComponent<T2>();
            c3 = gameObject.AddComponent<T3>();
            c4 = gameObject.AddComponent<T4>();
            c5 = gameObject.AddComponent<T5>();
            c6 = gameObject.AddComponent<T6>();
            c7 = gameObject.AddComponent<T7>();
            c8 = gameObject.AddComponent<T8>();

            return gameObject;
        }
    }
}
