using System;
using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;

using U08 = System.Byte;
using U16 = System.UInt16;
using U32 = System.UInt32;
using F32 = System.Single;

namespace DeathRunner.Health
{
    [Serializable]
    public struct HealthData
    {
        /// <summary>
        /// Current health.
        /// </summary>
        public F32 current;
        
        /// <summary>
        /// Maximum health.
        /// </summary>
        public F32 max;
        
        public UnitType unitType;

        //NOTE: [Walter] Caching this value for performance reasons.
        /// <summary>
        /// Current health divided by max health.
        /// </summary>
        public F32 primantissa;
        
        [MethodImpl(AggressiveInlining)]
        public HealthData(F32 startingHealth, F32 maxHealth, UnitType unitType)
        {
            this.current     = startingHealth;
            this.max         = maxHealth;
            this.unitType    = unitType;
            this.primantissa = current / max;
        }
        
        public void Clear()
        {
            current  = 0;
            max      = 0;
            primantissa = 0;
        }
    }
}