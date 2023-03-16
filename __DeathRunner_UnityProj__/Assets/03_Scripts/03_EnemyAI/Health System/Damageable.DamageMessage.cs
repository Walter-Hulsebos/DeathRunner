using UnityEngine;

namespace Damageable
{
    public partial class Damageable : MonoBehaviour
    {
        public struct DamageMessage
        {
            public MonoBehaviour damager;
            public int amount;
         //   public Vector3 direction;
         //   public Vector3 damageSource;
         //   public bool throwing;

         //   public bool stopCamera;
        }
    } 
}