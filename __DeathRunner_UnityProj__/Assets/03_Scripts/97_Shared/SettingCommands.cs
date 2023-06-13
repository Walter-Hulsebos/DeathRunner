using System.Collections;
using System.Collections.Generic;
using GenericScriptableArchitecture;
using JetBrains.Annotations;
using QFSW.QC;
using UnityEngine;

using Bool = System.Boolean;

namespace DeathRunner.Shared
{
    public sealed class SettingCommands : MonoBehaviour
    {
        [field:SerializeField] public Variable<Bool> OrientTowardsCursor { get; [UsedImplicitly] private set; }
        
        [Command(aliasOverride: "Player.OrientTowardsCursor")]
        public Bool OrientTowardsCursorCommand { get => OrientTowardsCursor.Value; set => OrientTowardsCursor.Value = value;}
    }
}
