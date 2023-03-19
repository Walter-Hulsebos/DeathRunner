using System;
using System.Collections;
using System.Collections.Generic;
using EasyCharacterMovement;
using UnityEngine;

namespace Game
{
    public class AnimationEventsPlayer : MonoBehaviour
    {
        [SerializeField] GameObject scytheHitbox;
        [SerializeField] private CharacterMotor _characterMotor;
        private void Start()
        {
            scytheHitbox.SetActive(false);
            _characterMotor = GetComponent<CharacterMotor>();
        }

        // Start is called before the first frame update
        public void EnableHitbox()
        {
            scytheHitbox.SetActive(true);
            //TODO disable player movement when state machine is there
        }

        // Update is called once per frame
        public void DisableHitbox()
        {
            scytheHitbox.SetActive(false);
        }
    }
}
