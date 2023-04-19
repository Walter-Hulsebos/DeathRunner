using System.Collections;
using UnityEngine;

using GenericScriptableArchitecture;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

using F32 = System.Single;

namespace DeathRunner.Animations
{
    public sealed class PrimaryAttackAnimHandler : AnimHandler
    {
        #region Variables
        
        //#if ODIN_INSPECTOR
        //[BoxGroup(group: "AnimationClips", showLabel: false)]
        //#endif
        //[SerializeField] private AnimationClip attackClip;
        
        #if ODIN_INSPECTOR
        [FoldoutGroup("Events")]
        #endif
        [SerializeField] private ScriptableEvent<AnimationClip> onAttackStarted;
        
        // #if ODIN_INSPECTOR
        // [FoldoutGroup("Events")]
        // #endif
        // [SerializeField] private ScriptableEvent onAttackStopped;


        #endregion
        
        #region Methods
        
        private void OnEnable()
        {
            onAttackStarted += OnAttackStartedHandler;
            //onAttackStopped += OnAttackStoppedHandler;
        }
        private void OnDisable()
        {
            onAttackStarted -= OnAttackStartedHandler;
            //onAttackStopped -= OnAttackStoppedHandler;
        }
        
        private void OnAttackStartedHandler(AnimationClip attackAnimation)
        {
            Animancer.Play(attackAnimation);
            
            //StartCoroutine(routine: CheckIfAttackIsFinished(attackAnimation));
            //StartCoroutine(routine: CheckIfCanGoIntoNextAttack(attackData.secondsToAllowNextAttack));
        }
        
        // private void OnAttackStoppedHandler()
        // {
        //     Animancer.Stop()
        // }
        //
        

        // private IEnumerator CheckIfAttackIsFinished(AnimationClip attackClip)
        // {
        //     // Check every frame if the attack animation is still playing.
        //     while (Animancer.IsPlaying(attackClip))
        //     //while (Animancer.IsPlayingClip(attackClip))
        //     {
        //         yield return null;
        //     }
        //
        //     //If the attack animation is not playing anymore, invoke the event.
        //     if (onAttackStopped != null)
        //     {
        //         onAttackStopped.Invoke();   
        //     }
        // }
        
        // private F32 _timeSpentPlayingAttackAnimation = 0;
        // private IEnumerator CheckIfCanGoIntoNextAttack(F32 secondsToAllowNextAttack)
        // {
        //     _timeSpentPlayingAttackAnimation = 0;
        //    
        //     while (_timeSpentPlayingAttackAnimation < secondsToAllowNextAttack)
        //     {
        //         _timeSpentPlayingAttackAnimation += Time.deltaTime;
        //         yield return null;
        //     }
        //     
        //     if (onCanGoIntoNextAttack != null)
        //     {
        //         onCanGoIntoNextAttack.Invoke();
        //     }
        // }

        #endregion
    }
}