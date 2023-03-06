using UnityEngine;
using UnityEngine.InputSystem;

using UltEvents;

namespace Game.Weapons
{
    public sealed class Attack : MonoBehaviour
    {
        [SerializeField] private InputActionReference attackInput;
        
        [SerializeField] private UltEvent onAttack;
        
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