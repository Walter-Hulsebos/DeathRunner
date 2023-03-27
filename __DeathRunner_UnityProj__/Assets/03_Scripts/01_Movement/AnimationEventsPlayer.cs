using System;
using System.Collections;
using System.Collections.Generic;
using EasyCharacterMovement;
using Game.Movement;
using UnityEngine;
namespace Game
{
    public class AnimationEventsPlayer : MonoBehaviour
    {
        [SerializeField] GameObject scytheHitbox;
        [SerializeField] private CharacterMotor _characterMotor;

         private Rigidbody rb;
        [SerializeField] private Locomotion _locomotion;
        
        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            scytheHitbox.SetActive(false);
            _characterMotor = GetComponent<CharacterMotor>();
            
        }

        public void DisableInputs()
        {
            _locomotion.enabled = false;
        }
        
        public void EnableInputs()
        {
            _locomotion.enabled = true;
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
        
        public void StartMoving()
        {
            //rb.MovePosition();
        }
        
        public void StopMoving()
        {
            _locomotion.enabled = true;
        }

        private void Update()
        {
          //  if (isMoving){ }
        }
    }
}
