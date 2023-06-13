using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DeathRunner
{
    public class Death : MonoBehaviour
    {
        public async UniTaskVoid OnDeath()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1.5f));
            SceneManager.LoadScene(0);
        }

    }
}
