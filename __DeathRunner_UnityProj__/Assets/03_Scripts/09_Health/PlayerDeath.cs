using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DeathRunner.Movement;
using DeathRunner.Shared;

namespace DeathRunner
{
    public class PlayerDeath : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        private Locomotion _locomotion;
        private Orientation _orientation;

        private void Start()
        {
            _locomotion = GetComponent<Locomotion>();
            _orientation = GetComponent<Orientation>();
        }

        public void OnPlayerDeath()
        {
            _animator.SetTrigger("Death");
            _locomotion.enabled = false;
            _orientation.enabled = false;
        }
    }
}
