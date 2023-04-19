using System;
using System.Threading;

using UnityEngine;
using static Unity.Mathematics.math;

using Cysharp.Threading.Tasks;
using GenericScriptableArchitecture;
using HFSM;
using JetBrains.Annotations;

using Sirenix.OdinInspector;
using F32  = System.Single;
using Bool = System.Boolean;

namespace DeathRunner.PlayerState
{
    public class PlayerStateLeaf_PrimaryAttack : StateLeaf
    {
        #region Variables

        private readonly PrimaryAttackSettings _settings;
        private readonly PlayerReferences      _references;

        private CancellationTokenSource _cancellationTokenSource = new();
        private CancellationToken       _cancellationToken;
        
        private readonly F32                     _secondsToAllowNextAttack;
        
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
            
            _secondsToAllowNextAttack = clamp(_settings.AttackAnimation.length - _settings.SecondsFromEndToAllowNextAttack, 0.0001f, _settings.AttackAnimation.length);
        }
        
        #endregion
        
        protected override void OnEnter()
        {
            base.OnEnter();
            Debug.Log("PrimaryAttack.Enter");
            
            RefreshCancellationToken();

            if (_settings.OnAttackStarted != null)
            {
                _settings.OnAttackStarted.Invoke(_settings.AttackAnimation);
            }
            
            IsAttacking = true;
            
            EnableCanGoIntoNextAttackAfterTime().Forget();
            StopAttackAfterFinishTime().Forget();
        }

        protected override void OnExit()
        {
            base.OnExit();

            _cancellationTokenSource.Cancel();

            if (IsAttacking)
            {
                IsAttacking = false;
                
                Debug.Log("PrimaryAttack.OnExit: Attack was still going on");
            }
            
            if (_settings.OnAttackStopped != null)
            {
                _settings.OnAttackStopped.Invoke();
            }
            
            Debug.Log("PrimaryAttack.Exit");
        }
        
        private async UniTask EnableCanGoIntoNextAttackAfterTime()
        {
            CanGoIntoNextAttack = false;
            
            Debug.Log("Waiting for " + _secondsToAllowNextAttack + " seconds to allow next attack, time: " + Time.time);
            
            await UniTask.Delay(TimeSpan.FromSeconds(_secondsToAllowNextAttack), ignoreTimeScale: true, cancellationToken: _cancellationToken);

            Debug.Log("Can go into next attack now, time: " + Time.time);
            
            CanGoIntoNextAttack = true;
        }

        private async UniTask StopAttackAfterFinishTime()
        {
            IsAttacking = true;
            
            Debug.Log("Waiting for " + _settings.AttackAnimation.length + " seconds to finish attack, time: " + Time.time);
            
            await UniTask.Delay(TimeSpan.FromSeconds(_settings.AttackAnimation.length), ignoreTimeScale: true, cancellationToken: _cancellationToken);
            
            Debug.Log("Attack finished, time: " + Time.time);
            
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
        [field:SerializeField] public AnimationClip                  AttackAnimation                 { get; [UsedImplicitly] private set; }
        [field:SerializeField] public Constant<F32>                  SecondsFromEndToAllowNextAttack { get; [UsedImplicitly] private set; }
        
        [field:SerializeField] public ScriptableEvent<AnimationClip> OnAttackStarted                 { get; [UsedImplicitly] private set; }
        [field:SerializeField] public ScriptableEvent                OnAttackStopped                 { get; [UsedImplicitly] private set; }
    }
}
