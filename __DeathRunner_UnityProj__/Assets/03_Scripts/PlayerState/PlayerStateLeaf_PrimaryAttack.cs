using System;
using System.Collections;
using System.Threading;

using UnityEngine;
using static Unity.Mathematics.math;

using Cysharp.Threading.Tasks;
using GenericScriptableArchitecture;
using HFSM;
using JetBrains.Annotations;

using DeathRunner.Shared;
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

        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private readonly CancellationToken       _cancellationToken;
        
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
            
            _cancellationToken = _cancellationTokenSource.Token;
            
            _secondsToAllowNextAttack = clamp(_settings.AttackAnimation.length - _settings.SecondsFromEndToAllowNextAttack, 0.0001f, _settings.AttackAnimation.length);

            // if (_settings.OnAttackStopped != null)
            // {
            //     _settings.OnAttackStopped.AddListener(OnAttackStoppedHandler);
            // }

            //_waitForSecondsToAllowNextAttack = new WaitForSeconds(_settings.AttackAnimation.length - _settings.SecondsFromEndToAllowNextAttack);
            //_attackData = new AttackData(attackAnimation: _settings.AttackAnimation,secondsToAllowNextAttack: _settings.AttackAnimation.length - _settings.SecondsFromEndToAllowNextAttack);
        }
        
        #endregion
        
        protected override void OnEnter()
        {
            base.OnEnter();
            
            Debug.Log("PrimaryAttack.Enter");

            if (_settings.OnAttackStarted != null)
            {
                _settings.OnAttackStarted.Invoke(_settings.AttackAnimation);
            }
            
            IsAttacking = true;
            
            EnableCanGoIntoNextAttackAfterTime().Forget();
            StopAttackAfterFinishTime().Forget();
        }

        // private F32 _timeSpentInAttack = 0;
        // private IEnumerator CheckIfCanGoIntoNextAttack(F32 secondsToAllowNextAttack)
        // {
        //     _timeSpentInAttack = 0;
        //    
        //     while (_timeSpentInAttack < secondsToAllowNextAttack)
        //     {
        //         _timeSpentInAttack += Time.deltaTime;
        //         yield return null;
        //     }
        //     
        //     CanGoIntoNextAttack = true;
        // }
        
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


            //CanGoIntoNextAttack = true;
            
            Debug.Log("PrimaryAttack.Exit");
        }
        
        private void OnAttackStoppedHandler()
        {
            Debug.Log("PrimaryAttack.OnAttackStoppedHandler");
            
            IsAttacking = false;
        }
        
        private async UniTask EnableCanGoIntoNextAttackAfterTime()
        {
            CanGoIntoNextAttack = false;
            
            //Debug.Log("Can't go into next attack yet " + Time.time);
            await UniTask.Delay(TimeSpan.FromSeconds(_secondsToAllowNextAttack), ignoreTimeScale: true, cancellationToken: _cancellationToken);
            //Debug.Log("Can go into next attack now " + Time.time);
            
            Debug.Log("Can go into next attack now " + Time.time);
            
            CanGoIntoNextAttack = true;
        }

        private async UniTask StopAttackAfterFinishTime()
        {
            IsAttacking = true;
            
            await UniTask.Delay(TimeSpan.FromSeconds(_settings.AttackAnimation.length), ignoreTimeScale: true, cancellationToken: _cancellationToken);
            
            Debug.Log("Attack finished " + Time.time);
            
            IsAttacking = false;
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
