//System libraries first

//Unity-specific libraries next
using System;
using GenericScriptableArchitecture;
using HFSM;
using JetBrains.Annotations;
using UnityEngine;
using static Unity.Mathematics.math;

//Third-party libraries next

//Project-specific libraries last
using F32x3 = Unity.Mathematics.float3;

namespace DeathRunner.Player
{
    public sealed class PlayerState_Locomotion : State
    {
        #region Variables

        private readonly LocomotionSettings _settings;
        private readonly PlayerReferences   _references;
        
        #endregion

        #region Constructor

        public PlayerState_Locomotion(LocomotionSettings settings, PlayerReferences references,
            params StateObject[] childStates) : base(childStates: childStates)
        {
            this._settings   = settings;
            this._references = references;
        }
        
        #endregion

        #region Methods

        protected override void LateFixedUpdateState()
        {
            base.LateFixedUpdateState();

            UpdateLookDirection();
        }

        private void UpdateLookDirection()
        {
            F32x3 __lookPositionRelativeToPlayer = PlayerHelpers.LookPositionRelativeToPlayer(_references);

            _settings.OrientationLookDirection.Value = normalize(__lookPositionRelativeToPlayer); 
            //normalize(__lookPosition - _references.WorldPos);
            
            //OrientTowardsPos(lookPosition: __lookPosition);
            _references.LookAt.position = (_references.WorldPos + __lookPositionRelativeToPlayer);
        }

        #endregion
    }
    
    [Serializable]
    public struct LocomotionSettings 
    {
        //[field:SerializeField] public Constant<F32>   OrientationSpeed         { get; [UsedImplicitly] private set; }
        [field:SerializeField] public Variable<F32x3> OrientationLookDirection { get; [UsedImplicitly] private set; }
    }
}