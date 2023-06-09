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
    public sealed class PlayerStateLeaf_AttackMelee : StateLeaf
    {
        #region Variables

        private readonly MeleeAttackSettings _settings;
        private readonly PlayerReferences    _references;
        
        private CancellationTokenSource      _cancellationTokenSource;
        private CancellationToken            _cancellationToken;
        
        private readonly F32                 _scaledSecondsFromBeginningToAllowNextAttack;
        private readonly F32                 _scaledAttackAnimationDuration;
        
        private readonly F32                 _scaledSecondsFromBeginningToFadeOut;

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
            
            _scaledSecondsFromBeginningToAllowNextAttack = clamp(_scaledAttackAnimationDuration - _settings.SecondsFromEndToAllowNextAttack.Value, 0.0001f, _settings.AttackAnimation.length);
            _scaledSecondsFromBeginningToFadeOut         = clamp(_scaledAttackAnimationDuration - _settings.SecondsFromEndToFadeOut.Value,         0.0001f, _settings.AttackAnimation.length);
            
            //Debug everything.
            Debug.Log
            (message:
                "<b>--- Melee Attack Settings ---</b>\n" +
                $"Attack Animation Duration: {_settings.AttackAnimation.length}\n" +
                $"Attack Speed Multiplier: {_settings.AttackSpeedMultiplier.Value}\n" +
                $"Attack Animation Duration Scaled: {_scaledAttackAnimationDuration}\n" +
                "\n" +
                $"Seconds From End To Allow Next Attack: {_settings.SecondsFromEndToAllowNextAttack.Value}\n" +
                $"Seconds From End To Fade Out: {_settings.SecondsFromEndToFadeOut.Value}\n" +
                "\n" +
                $"Scaled Seconds From Beginning To Allow Next Attack: {_scaledSecondsFromBeginningToAllowNextAttack}\n" +
                $"Scaled Seconds From Beginning To Fade Out: {_scaledSecondsFromBeginningToFadeOut}\n" +
                "<b>----------------------------</b>\n"
            );
        }
        
        #endregion
        
        protected override void OnEnter()
        {
            base.OnEnter();

            RefreshCancellationToken();

            //OrientTowardsCursor();
            
            UpdateLookDirection();
            
            PlayerHelpers.OrientTowardsDirInstant(references: _references, direction: _settings.OrientationLookDirection.Value);
            
            if (_settings.OnAttackStarted != null)
            {
                _settings.OnAttackStarted.Invoke(_settings.AttackAnimation, _settings.AttackSpeedMultiplier);
            }
            
            IsAttacking = true;
            
            TrackTimeSpentInAnimation().Forget();
            
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
                _settings.OnAttackStopped.Invoke(/*timeLeft*/ TimeRemainingInAnimation);
            }
        }
        
        private void UpdateLookDirection()
        {
            F32x3 __lookPositionRelativeToPlayer = PlayerHelpers.LookPositionRelativeToPlayer(_references, useCursor: _settings.OrientTowardsCursor.Value);
            
            _settings.OrientationLookDirection.Value = normalize(__lookPositionRelativeToPlayer);
            _references.LookAt.position = (_references.WorldPos + __lookPositionRelativeToPlayer);
        }
        
        public F32 TimeRemainingInAnimation => (_scaledAttackAnimationDuration - _timeSpentInAnimation);

        private F32 _timeSpentInAnimation = 0;

        private async UniTask TrackTimeSpentInAnimation()
        {
            _timeSpentInAnimation = 0;
            while (IsAttacking)
            {
                await UniTask.Yield();
                _timeSpentInAnimation += Time.deltaTime;
            }
        }

        private async UniTask EnableCanGoIntoNextAttackAfterTime()
        {
            CanGoIntoNextAttack = false;
            await UniTask.Delay(TimeSpan.FromSeconds(_scaledSecondsFromBeginningToAllowNextAttack), ignoreTimeScale: true, cancellationToken: _cancellationToken);
            if (_settings.OnAllowNextAttack != null)
            {
                _settings.OnAllowNextAttack.Invoke();
            }
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
            await UniTask.Delay(TimeSpan.FromSeconds(_scaledSecondsFromBeginningToFadeOut), ignoreTimeScale: true, cancellationToken: _cancellationToken);
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
        
        [field:SerializeField] public Variable<Bool>                      OrientTowardsCursor             { get; [UsedImplicitly] private set; }        
        [field:SerializeField] public Variable<F32x3>                     OrientationLookDirection        { get; [UsedImplicitly] private set; }
        //[field:SerializeField] public Constant<F32>                       OrientationSpeed                { get; [UsedImplicitly] private set; }

        [field:SerializeField] public ScriptableEvent<AnimationClip, F32> OnAttackStarted                 { get; [UsedImplicitly] private set; }
        [field:SerializeField] public ScriptableEvent<F32>                OnAttackStopped                 { get; [UsedImplicitly] private set; }
        
        [field:SerializeField] public ScriptableEvent                     OnAllowNextAttack               { get; [UsedImplicitly] private set; }
    }
}
