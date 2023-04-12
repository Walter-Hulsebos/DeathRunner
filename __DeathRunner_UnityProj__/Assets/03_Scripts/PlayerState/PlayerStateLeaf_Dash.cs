using System;
using DeathRunner.Inputs;
using EasyCharacterMovement;
using GenericScriptableArchitecture;
using HFSM;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;
using F32   = System.Single;
using F32x3 = Unity.Mathematics.float3;
using Object = UnityEngine.Object;

namespace DeathRunner.Shared.StateMachine
{
    public class PlayerStateLeaf_Dash : StateLeaf
    {
        protected override void OnEnter()
        {
            base.OnEnter();
            
            Debug.Log("Dash.Enter");
        }
        
        protected override void OnExit()
        {
            base.OnExit();
            
            Debug.Log("Dash.Exit");
        }
    }
    
    [Serializable]
    public struct DashSettings
    {
        [field:Tooltip(tooltip: "The max dash speed (m/s)")]
        [field:SerializeField] public Constant<F32> MaxSpeed { get; [UsedImplicitly] private set; }
        
        [field:Tooltip(tooltip: "The max dash distance (m)")]
        [field:SerializeField] public Constant<F32> MaxDistance { get; [UsedImplicitly] private set; }
        
        [field:Tooltip(tooltip: "The duration between possible dashes (s)")]
        [field:SerializeField] public Constant<F32> DashCooldown { get; [UsedImplicitly] private set; }
    }
}
