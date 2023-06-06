using GenericScriptableArchitecture;

using UnityEngine;
using UnityEngine.UI;

using F32 = System.Single;
using U16 = System.UInt16;

namespace DeathRunner.Attributes
{
    public sealed class ChangeableUI : MonoBehaviour
    {
        [SerializeField] private Constant<U16>  max;
        [SerializeField] private Reference<U16> current;

        [SerializeField] private EventReference</*oldValue*/U16, /*newValue*/U16> OnChanged;
        
        [SerializeField] private Slider healthSlider;
        
        private void OnEnable()
        {
            OnChanged += OnChangedHandler;
        }
        
        private void OnDisable()
        {
            OnChanged -= OnChangedHandler;
        }

        public void OnChangedHandler(U16 oldValue, U16 newValue)
        {
            //NOTE [Walter] Using the `old value` you could even tween to the `new value`, if you wanted to.
            
            F32 __healthPrimantissa = (F32)newValue / (F32)max.Value;
            healthSlider.value = __healthPrimantissa;
        }
    }
}