using System.Collections.Generic;
using UnityEngine;

using I32 = System.Int32;

namespace DeathRunner.Utils
{
    public static class CollectionExtensions
    {
        public static T GetRandom<T>(this T[] array)
        {
            return array[Random.Range(minInclusive: 0, maxExclusive: array.Length)];
        }
        public static T GetRandom<T>(this T[] array, out I32 index)
        {
            index = Random.Range(minInclusive: 0, maxExclusive: array.Length);
            return array[index];
        }

        public static T GetRandom<T>(this T[] array, Unity.Mathematics.Random rng)
        {
            return array[rng.NextInt(max: array.Length - 1)];
        }
        public static T GetRandom<T>(this T[] array, out I32 index, Unity.Mathematics.Random rng)
        {
            index = rng.NextInt(max: array.Length - 1);
            return array[index];
        }
        
        public static T GetRandom<T>(this List<T> list)
        {
            return list[Random.Range(minInclusive: 0, maxExclusive: list.Count)];
        }
        public static T GetRandom<T>(this List<T> list, out I32 index)
        {
            index = Random.Range(minInclusive: 0, maxExclusive: list.Count);
            return list[index];
        }
        
        public static T GetRandom<T>(this List<T> list, Unity.Mathematics.Random rng)
        {
            return list[rng.NextInt(max: list.Count - 1)];
        }
        public static T GetRandom<T>(this List<T> list, out I32 index, Unity.Mathematics.Random rng)
        {
            index = rng.NextInt(max: list.Count - 1);
            return list[index];
        }
        
        

    }
}