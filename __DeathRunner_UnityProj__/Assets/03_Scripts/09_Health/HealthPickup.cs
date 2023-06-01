using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

using static DeathRunner.Attributes.ChangeLogic;

using F32 = System.Single;
using U16 = System.UInt16;

namespace DeathRunner.Attributes
{
    public sealed class HealthPickup : MonoBehaviour
    {
        #if ODIN_INSPECTOR
        [EnumToggleButtons]
        #endif
        [SerializeField] private ChangeLogic logic = Instant;
        
        [SerializeField] private String tagToHeal = "Player";
        
        [SerializeField] private U16 amount = 3;
        
        #if ODIN_INSPECTOR
        [ShowIf(condition: "logic", optionalValue: OverTime)]
        #endif
        [SerializeField] private F32 duration = 1f;
        
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(tagToHeal)) return;
            
            if (!other.TryGetComponent(out HealthComponent __healthComponent)) return;
                
            switch (logic)
            {
                case Instant:
                    HealInstantly(__healthComponent);
                    break;
                case OverTime:
                    HealOverTime(__healthComponent).Forget();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private void HealInstantly(HealthComponent healthComponent)
        {
            healthComponent.health.Value += amount;
            
            Destroy(gameObject);
        }

        private async UniTask HealOverTime(HealthComponent healthComponent)
        {
            F32 __timeElapsed = 0f;
            F32 __healthValueAsFloat = healthComponent.health.Value;
            
            while (__timeElapsed < duration)
            {
                __healthValueAsFloat += amount * (Time.deltaTime / duration);
                healthComponent.health.Value = (U16)__healthValueAsFloat;
                
                __timeElapsed += Time.deltaTime;
                await UniTask.Yield();
            }
            
            Destroy(gameObject);
        }
    }
}
