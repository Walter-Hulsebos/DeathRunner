using JetBrains.Annotations;
using UnityEngine;

using U16 = System.UInt16;

namespace DeathRunner.Attributes.Modifiers
{
    [CreateAssetMenu(menuName = "DeathRunner/Attributes/Modifiers/Sub Value U16")]
    public sealed class ModSubValueU16 : ScriptableObject, IModSubValue<U16>
    {
        [field:SerializeField] public U16 Value { get; [UsedImplicitly] private set; }
        
        public U16 ApplyTo(U16 value)
        {
            return (U16)(value - Value);
        }
    }
}