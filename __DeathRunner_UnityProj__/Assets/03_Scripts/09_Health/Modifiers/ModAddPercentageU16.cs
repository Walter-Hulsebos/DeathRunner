using JetBrains.Annotations;
using UnityEngine;

using U16 = System.UInt16;

namespace DeathRunner.Attributes.Modifiers
{
    [CreateAssetMenu(menuName = "DeathRunner/Attributes/Modifiers/Add Percentage U16")]
    public sealed class ModAddPercentageU16 : ScriptableObject, IModAddPercentage<U16>
    {
        [field:SerializeField] public U16 Percentage { get; [UsedImplicitly] private set; }
        
        public U16 ApplyTo(U16 value)
        {
            return (U16)(value + (value * Percentage));
        }
    }
}