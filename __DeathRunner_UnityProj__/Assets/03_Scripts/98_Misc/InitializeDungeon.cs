using System;
using System.Collections;
using System.Collections.Generic;
using DungeonArchitect;
using DungeonArchitect.Builders.Snap;
using UnityEngine;
using Random = UnityEngine.Random;


namespace Game
{
    public class InitializeDungeon : MonoBehaviour
    {
        [SerializeField] private Dungeon _dungeon;
        [SerializeField] private SnapConfig _snap;

        private void Start()
        {
            uint seed = Convert.ToUInt32(Random.Range(0, 100000));

            _snap.Seed = seed;

            _dungeon.Build();
        }
    }
}
