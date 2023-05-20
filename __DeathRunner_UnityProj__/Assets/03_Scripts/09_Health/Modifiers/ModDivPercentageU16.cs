using JetBrains.Annotations;
using UnityEngine;

using U16 = System.UInt16;

namespace DeathRunner.Attributes.Modifiers
{
    [CreateAssetMenu(menuName = "DeathRunner/Attributes/Modifiers/Div Percentage U16")]
    public sealed class ModDivPercentageU16 : ScriptableObject, IModDivPercentage<U16>
    {
        [field:SerializeField] public U16 Percentage { get; [UsedImplicitly] private set; }
        
        public U16 ApplyTo(U16 value)
        {
            return (U16)(value / (value * Percentage));
        }
    }
}