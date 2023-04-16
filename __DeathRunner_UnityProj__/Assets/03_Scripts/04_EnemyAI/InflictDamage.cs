using UnityEngine;

//TODO remove damageable namescape make it Game instead
namespace DeathRunner.EnemyAI
{
    public class InflictDamage : MonoBehaviour
    {
        // Start is called before the first frame update
        [SerializeField] private string targetTag;

        [SerializeField] private int damageInflicted = 1;
        
        // Update is called once per frame
        private void OnTriggerEnter(Collider other)
        {
            //Todo make it usable for other things maybe
            if (!other.CompareTag(targetTag)) return;
            
            Damageable.Damageable.DamageMessage data;
            data.amount = damageInflicted;
            data.damager = this;

            if (other.TryGetComponent(out Damageable.Damageable damageable))
            {
                damageable.ApplyDamage(data);
            }
        }
    }
}