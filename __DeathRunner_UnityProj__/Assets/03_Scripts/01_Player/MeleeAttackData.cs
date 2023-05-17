using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DeathRunner.Player
{
    #if ODIN_INSPECTOR
    [InlineEditor]
    #endif
    [CreateAssetMenu(fileName = "MeleeAttack", menuName = "DeathRunner/Player/MeleeAttack")]
    public sealed class MeleeAttackData : ScriptableObject
    {
        [field:SerializeField] public MeleeAttackSettings Settings { get; [UsedImplicitly] private set; }
    }
}