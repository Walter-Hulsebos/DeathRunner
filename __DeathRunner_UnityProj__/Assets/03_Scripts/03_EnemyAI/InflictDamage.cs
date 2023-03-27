using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO remove damageable namescape make it Game instead
namespace Damageable
{
    public class InflictDamage : MonoBehaviour
    {
        // Start is called before the first frame update
        [SerializeField]  private string targetTag;

        [SerializeField] private int damageInflicted;
        // Update is called once per frame
        private void OnTriggerEnter(Collider other)
        {
            //Todo make it usable for other things maybe
            if (other.CompareTag(targetTag))
            {
                Damageable.DamageMessage data;
                data.amount = damageInflicted;
                data.damager = this;
                other.GetComponent<Damageable>().ApplyDamage(data);             
            }
        }
    }
}
