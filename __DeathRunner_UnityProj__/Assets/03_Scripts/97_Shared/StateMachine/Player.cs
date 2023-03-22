using HFSM;
using UnityEngine;

namespace DeathRunner.Shared.StateMachine
{
    public sealed class Player : MonoBehaviour
    {
        private State _stateMachine;

        private void Awake()
        {
            
        }

        private void Update()
        {
            _stateMachine.Update();
        }

        private void FixedUpdate()
        {
            _stateMachine.FixedUpdate();
        }

        private void LateUpdate()
        {
            _stateMachine.LateUpdate();
        }

        private void OnEnable()
        {
            //_stateMachine.OnEnable();
        }

        private void OnDisable()
        {
            //_stateMachine.OnDisable();
        }

        private void OnDestroy()
        {
            //_stateMachine.OnDestroy();
        }
    }
}
