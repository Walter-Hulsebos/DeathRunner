using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace Game
{
    public class FeedbackManager : MonoBehaviour
    {
        [SerializeField] MMFeedbacks gotHitFeedbacks;

        [SerializeField] private MMFeedbacks enemyGotHitFeedbacks;

        [SerializeField] private MMFeedbacks enemyDiedFeedbacks;
        // Update is called once per frame
        public void PlayGotHitFeedbacks()
        {
            gotHitFeedbacks.PlayFeedbacks();
        }
        public void EnemyGotHitFeedbacks()
        {
            enemyGotHitFeedbacks.PlayFeedbacks();
        }
        
        
        public void EnemyDiedFeedbacks()
        {
            enemyDiedFeedbacks.PlayFeedbacks();
        }
    }
}
