using DeathRunner.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

using F32  = System.Single;
using U16  = System.UInt16;

namespace DeathRunner.Enemies
{
    /// <summary> An enemy encounter without waves. </summary>
    public sealed class Encounter : MonoBehaviour
    {
        [Probability(dataMember: nameof(enemyPrefabs))]
        [SerializeField] private F32[] spawnProbability;
        
        [AssetsOnly]
        [SerializeField] private Enemy[] enemyPrefabs;
        
        [BoxGroup("Spawn Settings", showLabel: false)]
        [SerializeField] private U16 totalAmountToSpawn = 4;
        
        [Space]
        [SerializeField] private Transform[] spawnPoints;
    }
}
