using System;
using System.Threading;

using UnityEngine;
using static Unity.Mathematics.math;

using Cysharp.Threading.Tasks;
using GenericScriptableArchitecture;
using HFSM;
using JetBrains.Annotations;

using Sirenix.OdinInspector;

using F32   = System.Single;
using F32x3 = Unity.Mathematics.float3;
using Bool  = System.Boolean;

namespace DeathRunner.PlayerState
{
    public class PlayerStateLeaf_PrimaryAttack : StateLeaf
    {
        #region Variables

        private readonly PrimaryAttackSettings _settings;
        private readonly PlayerReferences      _references;
        
        private CancellationTokenSource        _cancellationTokenSource;
        private CancellationToken              _cancellationToken;
        
        private readonly F32                   _secondsToAllowNextAttack;
        private readonly F32                   _scaledDuration;

        public Bool IsAttacking         { get; private set; } = false;
        public Bool IsDoneAttacking     => !IsAttacking;
        public Bool CanGoIntoNextAttack { get; private set; } = false;

        #endregion
        
        #region Constructors
        
        public PlayerStateLeaf_PrimaryAttack(PrimaryAttackSettings settings, PlayerReferences references)
        {
            this._settings   = settings;
            this._references = references;
            
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
            
            _secondsToAllowNextAttack = clamp((_settings.AttackAnimation.length / _settings.AttackSpeedMultiplier.Value) - _settings.SecondsFromEndToAllowNextAttack, 0.0001f, _settings.AttackAnimation.length);
            _scaledDuration           = _settings.AttackAnimation.length / _settings.AttackSpeedMultiplier.Value;
        }
        
        #endregion
        
        protected override void OnEnter()
        {
            base.OnEnter();

            RefreshCancellationToken();

            F32x3 __lookPositionRelativeToPlayer = PlayerHelpers.LookPositionRelativeToPlayer(_references);
            F32x3 __direction = normalize(__lookPositionRelativeToPlayer);
            _settings.OrientationLookDirection.Value = __direction;
            _references.LookAt.position = (_references.WorldPos + __lookPositionRelativeToPlayer);
            
            PlayerHelpers.OrientTowardsDir(references: _references, direction: __direction, orientationSpeed: _settings.OrientationSpeed);

            if (_settings.OnAttackStarted != null)
            {
                _settings.OnAttackStarted.Invoke(_settings.AttackAnimation, _settings.AttackSpeedMultiplier);
            }
            IsAttacking = true;
            
            EnableCanGoIntoNextAttackAfterTime().Forget();
            StopAttackAfterFinishTime().Forget();
        }

        protected override void OnExit()
        {
            base.OnExit();

            _cancellationTokenSource.Cancel();
            
            IsAttacking = false;
            if (_settings.OnAttackStopped != null)
            {
                _settings.OnAttackStopped.Invoke();
            }
        }
        
        private async UniTask EnableCanGoIntoNextAttackAfterTime()
        {
            CanGoIntoNextAttack = false;
            await UniTask.Delay(TimeSpan.FromSeconds(_secondsToAllowNextAttack), ignoreTimeScale: true, cancellationToken: _cancellationToken);
            CanGoIntoNextAttack = true;
        }

        private async UniTask StopAttackAfterFinishTime()
        {
            IsAttacking = true;
            await UniTask.Delay(TimeSpan.FromSeconds(_scaledDuration), ignoreTimeScale: true, cancellationToken: _cancellationToken);
            IsAttacking = false;
        }
        
        private void RefreshCancellationToken()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
        }
    }
    
    [Serializable]
    public struct PrimaryAttackSettings
    {
        [field:BoxGroup(group: "Attack Settings", showLabel: false)]
        [field:SerializeField] public AnimationClip                       AttackAnimation                 { get; [UsedImplicitly] private set; }
        [field:SerializeField] public Constant<F32>                       AttackSpeedMultiplier           { get; [UsedImplicitly] private set; }
        [field:SerializeField] public Constant<F32>                       SecondsFromEndToAllowNextAttack { get; [UsedImplicitly] private set; }
        
        [field:SerializeField] public Variable<F32x3>                     OrientationLookDirection        { get; [UsedImplicitly] private set; }
        [field:SerializeField] public Constant<F32>                       OrientationSpeed                { get; [UsedImplicitly] private set; }

        [field:SerializeField] public ScriptableEvent<AnimationClip, F32> OnAttackStarted                 { get; [UsedImplicitly] private set; }
        [field:SerializeField] public ScriptableEvent                     OnAttackStopped                 { get; [UsedImplicitly] private set; }
    }
}
