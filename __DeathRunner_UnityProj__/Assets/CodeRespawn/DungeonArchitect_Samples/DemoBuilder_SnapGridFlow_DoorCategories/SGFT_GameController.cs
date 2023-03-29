using System.Collections;
using System.Collections.Generic;
using DungeonArchitect;
using UnityEngine;

public class SGFT_GameController : MonoBehaviour
{
    public Dungeon dungeon;

    private void Start()
    {
        if (dungeon != null)
        {
            dungeon.Build();
        }
    }

}
