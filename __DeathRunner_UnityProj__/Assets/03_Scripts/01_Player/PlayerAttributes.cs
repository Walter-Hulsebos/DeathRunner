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
            health.Init();
            stamina.Init();
            
            Debug.Log(message: $"Init PlayerAttributes, setting health and stamina to max \n" +
                               $"Health Max:  {health.Max.Value} \n" +
                               $"Stamina Max: {stamina.Max.Value} \n" +
                               $"Health:  {health.Value} \n" +
                               $"Stamina: {stamina.Value}");
        }
    }
}