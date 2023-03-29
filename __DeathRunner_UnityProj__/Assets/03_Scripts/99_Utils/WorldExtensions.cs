using System;
using UnityEngine;
using Object = UnityEngine.Object;

using F32   = System.Single;
using F32x3 = Unity.Mathematics.float3;

using Bool  = System.Boolean;

namespace DeathRunner.Utils
{
    public static class WorldExtensions
    {
        public static Boolean TryFindObjectOfType<T>(out T result) where T : Component
        {
            result = Object.FindObjectOfType<T>();

            return (result != null);
        }
    }
}