using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DeathRunner
{
    public class InputSystemUpdater : MonoBehaviour
    {
        void Update()
        {
            InputSystem.Update();
        }
    }
}
