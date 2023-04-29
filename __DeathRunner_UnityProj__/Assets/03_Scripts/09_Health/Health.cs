using System;
using System.Collections;
using System.Collections.Generic;
using GenericScriptableArchitecture;
using JetBrains.Annotations;
using UnityEngine;

using U16  = System.UInt16; //max 65,535
using Bool = System.Boolean;

namespace DeathRunner.Health
{
    [Serializable]
    public class Health : IDamageable
    {
        public Constant<U16> MaxHealth     { get; [UsedImplicitly] private set; }
        
        private U16 _currentHealthBackingField;
        public U16 CurrentHealth 
        {
            get => _currentHealthBackingField;
            private set
            {
                _currentHealthBackingField = value;
                
                if (_currentHealthBackingField > MaxHealth.Value)
                {
                    _currentHealthBackingField = MaxHealth.Value;
                }
                
                if (_currentHealthBackingField <= 0)
                {
                    _currentHealthBackingField = 0;
                }
            }
        }

        public ScriptableEvent<U16, U16> OnHealthChanged;
        public ScriptableEvent OnHealthIncreased;
        public ScriptableEvent OnHealthDecreased;
        public ScriptableEvent OnHealthDepleted;
        
        public Bool IsDead => CurrentHealth == 0;


        #region Constructors

        public Health(Constant<U16> maxHealth)
        {
            MaxHealth     = maxHealth;
            CurrentHealth = maxHealth.Value;
        }
        
        public Health(U16 initialHealth)
        {
            CurrentHealth = initialHealth;
        }

        

        #endregion

        public void Damage(U16 damage)
        {
            CurrentHealth -= damage;
        }
        
        public void Heal(U16 heal)
        {
            CurrentHealth += heal;
        }
        
        
        
        
    }
}
