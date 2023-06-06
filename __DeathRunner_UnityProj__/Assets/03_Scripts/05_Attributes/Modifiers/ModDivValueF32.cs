using JetBrains.Annotations;
using UnityEngine;

using F32 = System.Single;

namespace DeathRunner.Attributes.Modifiers
{
    [CreateAssetMenu(menuName = "DeathRunner/Attributes/Modifiers/Div Value F32")]
    public sealed class ModDivValueF32 : ScriptableObject, IModDivValue<F32>
    {
        [field:SerializeField] public F32 Value { get; [UsedImplicitly] private set; }
        
        public F32 ApplyTo(F32 value)
        {
            return value / Value;
        }
    }
}