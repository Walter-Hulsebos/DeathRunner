using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using GenericScriptableArchitecture;
using HFSM;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;
using static Unity.Mathematics.math;
using F32   = System.Single;
using F32x3 = Unity.Mathematics.float3;
using Bool  = System.Boolean;

namespace DeathRunner.Player
{
    public class PlayerStateLeaf_AttackMelee : StateLeaf
    {
        #region Variables

        private readonly MeleeAttackSettings _settings;
        private readonly PlayerReferences    _references;
        
        private CancellationTokenSource      _cancellationTokenSource;
        private CancellationToken            _cancellationToken;
        
        private readonly F32                 _secondsFromBeginningToAllowNextAttack;
        private readonly F32                 _scaledAttackAnimationDuration;
        
        private readonly F32                 _secondsFromBeginningToFadeOut;

        public Bool IsAttacking            { get; private set; } = false;
        public Bool IsDoneAttacking        => !IsAttacking;
        
        public Bool CanGoIntoNextAttack    { get; private set; } = false;
        public Bool CanNotGoIntoNextAttack => !CanGoIntoNextAttack;
        
        public Bool CanFadeOut             { get; private set; } = false;
        public Bool CanNotFadeOut          => !CanFadeOut;

        #endregion
        
        #region Constructors
        
        public PlayerStateLeaf_AttackMelee(MeleeAttackSettings settings, PlayerReferences references)
        {
            this._settings   = settings;
            this._references = references;
            
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
            
            _scaledAttackAnimationDuration = (_settings.AttackAnimation.length / _settings.AttackSpeedMultiplier.Value);
            
            _secondsFromBeginningToAllowNextAttack = clamp(_scaledAttackAnimationDuration - _settings.SecondsFromEndToAllowNextAttack.Value, 0.0001f, _settings.AttackAnimation.length);
            _secondsFromBeginningToFadeOut         = clamp(_scaledAttackAnimationDuration - _settings.SecondsFromEndToFadeOut.Value,         0.0001f, _settings.AttackAnimation.length);   
        }
        
        #endregion
        
        protected override void OnEnter()
        {
            base.OnEnter();

            RefreshCancellationToken();

            OrientTowardsMouse();
            
            IsAttacking = true;
            
            EnableCanGoIntoNextAttackAfterTime().Forget();
            StopAttackAfterFinishTime().Forget();
            FadeOutAfterFinishTime().Forget();
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

        private void OrientTowardsMouse()
        {
            F32x3 __lookPositionRelativeToPlayer = PlayerHelpers.LookPositionRelativeToPlayer(_references);
            F32x3 __direction = normalize(__lookPositionRelativeToPlayer);
            _settings.OrientationLookDirection.Value = __direction;
            _references.LookAt.position = (_references.WorldPos + __lookPositionRelativeToPlayer);
            
            PlayerHelpers.OrientTowardsDirInstant(references: _references, direction: __direction);

            if (_settings.OnAttackStarted != null)
            {
                _settings.OnAttackStarted.Invoke(_settings.AttackAnimation, _settings.AttackSpeedMultiplier);
            }
        }
        
        private async UniTask EnableCanGoIntoNextAttackAfterTime()
        {
            CanGoIntoNextAttack = false;
            await UniTask.Delay(TimeSpan.FromSeconds(_secondsFromBeginningToAllowNextAttack), ignoreTimeScale: true, cancellationToken: _cancellationToken);
            CanGoIntoNextAttack = true;
        }

        private async UniTask StopAttackAfterFinishTime()
        {
            IsAttacking = true;
            await UniTask.Delay(TimeSpan.FromSeconds(_scaledAttackAnimationDuration), ignoreTimeScale: true, cancellationToken: _cancellationToken);
            IsAttacking = false;
        }
        
        private async UniTask FadeOutAfterFinishTime()
        {
            CanFadeOut = false;
            await UniTask.Delay(TimeSpan.FromSeconds(_secondsFromBeginningToFadeOut), ignoreTimeScale: true, cancellationToken: _cancellationToken);
            CanFadeOut = true;
        }
        
        private void RefreshCancellationToken()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
        }
    }
    
    [Serializable]
    public struct MeleeAttackSettings
    {
        [field:SerializeField] public AnimationClip                       AttackAnimation                 { get; [UsedImplicitly] private set; }
        //[field:SerializeField] public Constant<F32>                       AttackDamage                    { get; [UsedImplicitly] private set; }
        [field:SerializeField] public Constant<F32>                       AttackSpeedMultiplier           { get; [UsedImplicitly] private set; }
        [field:SerializeField] public Constant<F32>                       SecondsFromEndToAllowNextAttack { get; [UsedImplicitly] private set; }
        [field:SerializeField] public Constant<F32>                       SecondsFromEndToFadeOut         { get; [UsedImplicitly] private set; }
        
        
        [field:SerializeField] public Variable<F32x3>                     OrientationLookDirection        { get; [UsedImplicitly] private set; }
        //[field:SerializeField] public Constant<F32>                       OrientationSpeed                { get; [UsedImplicitly] private set; }

        [field:SerializeField] public ScriptableEvent<AnimationClip, F32> OnAttackStarted                 { get; [UsedImplicitly] private set; }
        [field:SerializeField] public ScriptableEvent                     OnAttackStopped                 { get; [UsedImplicitly] private set; }
    }
}
