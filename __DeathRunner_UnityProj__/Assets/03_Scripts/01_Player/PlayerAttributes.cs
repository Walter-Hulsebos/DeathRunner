using System;
using DeathRunner.Attributes;
using UnityEngine;

namespace DeathRunner.Player
{
    [Serializable]
    public sealed class PlayerAttributes
    {
        public Health  health;
        public Stamina stamina;

        public void Init()
        {
            Debug.Log(message: $"Init PlayerAttributes, setting health and stamina to max \n" +
                      $"Health:  {health.Max.Value} \n" +
                      $"Stamina: {stamina.Max.Value}");
            health.Value  = health.Max.Value;
            stamina.Value = stamina.Max.Value;
        }
    }
}
