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

        public void Init(GameObject owner)
        {
            Debug.Log(message: $"<b><color=red>Before</color></b> Init PlayerAttributes, setting health and stamina to max \n" +
                               $"Health Max:  {health.Max.Value} \n" +
                               $"Stamina Max: {stamina.Max.Value} \n" +
                               $"Health:  {health.Value} \n" +
                               $"Stamina: {stamina.Value}", context: owner);
            
            //Debug.Log(message: "----------------------------------------------------------", context: owner);
            
            health.Init(owner);
            Debug.Log(message: "----------------------------------------------------------", context: owner);
            stamina.Init(owner);
            
            //Debug.Log(message: "----------------------------------------------------------", context: owner);
            
            Debug.Log(message: $"<b><color=lime>After</color></b> Init PlayerAttributes: \n" +
                               $"Health:  {health.Value} \n" +
                               $"Stamina: {stamina.Value}", context: owner);
        }
    }
}