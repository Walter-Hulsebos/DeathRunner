using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class AnimationEventsPlayer : MonoBehaviour
    {
        [SerializeField] GameObject scytheHitbox;

        private void Start()
        {
            scytheHitbox.SetActive(false);
        }

        // Start is called before the first frame update
        public void EnableHitbox()
        {
            scytheHitbox.SetActive(true);
        }

        // Update is called once per frame
        public void DisableHitbox()
        {
            scytheHitbox.SetActive(false);
        }
    }
}
