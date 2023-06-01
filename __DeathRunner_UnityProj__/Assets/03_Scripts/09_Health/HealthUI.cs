using GenericScriptableArchitecture;
using UnityEngine;
using UnityEngine.UI;

using F32 = System.Single;
using U16 = System.UInt16;

namespace DeathRunner.Attributes
{
    public sealed class HealthUI : MonoBehaviour
    {
        [SerializeField] private Constant<U16> maxHealth;
        //[SerializeField] private Reference<U16> currentHealth;

        [SerializeField] private EventReference</*oldHealth*/U16, /*newHealth*/U16> OnHealthChanged;
        
        [SerializeField] private Image healthSlider;
        
        private void OnEnable()
        {
            OnHealthChanged += OnHealthChangedHandler;
        }
        
        private void OnDisable()
        {
            OnHealthChanged -= OnHealthChangedHandler;
        }

        public void OnHealthChangedHandler(U16 oldHealth, U16 newHealth)
        {
            //NOTE [Walter] Using the old health you could even tween to the new health, if you wanted to.
            
            F32 healthPrimantissa = (F32)newHealth / (F32)maxHealth.Value;
            healthSlider.fillAmount = healthPrimantissa;
        }
    }
}