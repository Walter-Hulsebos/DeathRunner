using GenericScriptableArchitecture;
using UnityEngine;
using UnityEngine.UI;

using F32 = System.Single;
using U16 = System.UInt16;

namespace DeathRunner.Attributes
{
    public sealed class HealthUI : MonoBehaviour
    {
        [SerializeField] private Constant<F32> maxHealth;
        //[SerializeField] private Reference<U16> currentHealth;

        [SerializeField] private EventReference</*oldHealth*/F32, /*newHealth*/F32> OnHealthChanged;
        
        [SerializeField] private Image healthSlider;
        
        private void OnEnable()
        {
            OnHealthChanged += OnHealthChangedHandler;
        }
        
        private void OnDisable()
        {
            OnHealthChanged -= OnHealthChangedHandler;
        }

        public void OnHealthChangedHandler(F32 oldHealth, F32 newHealth)
        {
            //NOTE [Walter] Using the old health you could even tween to the new health, if you wanted to.
            
            F32 __healthPrimantissa = newHealth / maxHealth.Value;
            healthSlider.fillAmount = __healthPrimantissa;
        }
    }
}