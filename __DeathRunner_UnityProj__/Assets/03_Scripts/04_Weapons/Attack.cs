using ExtEvents;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DeathRunner.Weapons
{
    public sealed class Attack : MonoBehaviour
    {
        [SerializeField] private InputActionReference attackInput;
        
        [SerializeField] private ExtEvent onAttack;
        
        private void OnEnable()
        {
            attackInput.action.Enable();
            
            attackInput.action.performed += OnAttack;
        }
        
        private void OnDisable()
        {
            attackInput.action.Disable();
            
            attackInput.action.performed -= OnAttack;
        }

        private void OnAttack(InputAction.CallbackContext obj)
        {
            onAttack.Invoke();
        }
    }
}