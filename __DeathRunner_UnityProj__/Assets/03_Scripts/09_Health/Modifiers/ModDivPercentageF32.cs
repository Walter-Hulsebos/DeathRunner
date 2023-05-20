using JetBrains.Annotations;
using UnityEngine;

using F32 = System.Single;

namespace DeathRunner.Attributes.Modifiers
{
    [CreateAssetMenu(menuName = "DeathRunner/Attributes/Modifiers/Div Percentage F32")]
    public sealed class ModDivPercentageF32 : ScriptableObject, IModDivPercentage<F32>
    {
        [field:SerializeField] public F32 Percentage { get; [UsedImplicitly] private set; }
        
        public F32 ApplyTo(F32 value)
        {
            return value / (value * Percentage);
        }
    }
}