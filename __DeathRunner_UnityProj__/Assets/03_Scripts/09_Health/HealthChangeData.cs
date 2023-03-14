using I32 = System.Int32;
using U16 = System.UInt16;
using F32 = System.Single;

using Bool = System.Boolean;

namespace DeathRunner.Health
{
    /// <summary>
    /// Health change data. Data-oriented design approach to health changes.
    /// Can be used to damage or heal health over time or instantly.
    /// </summary>
    public struct HealthChangeData
    {
        public F32  delta;
        public F32  secondsLeft;
        public Bool isInstant;
        public readonly U16      targetHealthIndex;
        public readonly UnitType affectedUnitTypes;

        /// <param name="delta">
        /// The health change per second.
        /// `+` heals,
        /// `-` damages,
        /// Per-second values, unless <see cref="isInstant"/> is true, then it's an instant (single-frame) change.
        /// </param>
        /// <param name="durationInSeconds">The amount in seconds this health change should be active. </param>
        /// <param name="targetHealthIndex">The target health index in the <see cref="HealthManager"/>'s health pool.</param>
        /// <param name="affectedUnitTypes">The types of units that are affected by this health change, by default it's ALL. </param>
        ///// <param name="affectsHealth">used to determine whether or not this health change should be performed.</param>
        public HealthChangeData(F32 delta, U16 targetHealthIndex, F32 durationInSeconds, UnitType affectedUnitTypes = UnitType.All)
        {
            this.delta             = delta;
            this.secondsLeft       = durationInSeconds;
            this.isInstant         = (durationInSeconds == 0); //NOTE: [Walter] Safety precaution, if the duration is 0 in the constructor, it's an instant change.
            this.targetHealthIndex = targetHealthIndex;
            this.affectedUnitTypes = affectedUnitTypes;
        }
        
        /// <param name="delta">
        /// The health change in a single frame.
        /// `+` heals,
        /// `-` damages,
        /// </param>
        /// <param name="targetHealthIndex">The target health index in the <see cref="HealthManager"/>'s health pool.</param>
        /// <param name="affectedUnitTypes">The types of units that are affected by this health change, by default it's ALL. </param>
        ///// <param name="affectsHealth">used to determine whether or not this health change should be performed.</param>
        public HealthChangeData(F32 delta, U16 targetHealthIndex, UnitType affectedUnitTypes = UnitType.All)
        {
            this.delta             = delta;
            this.secondsLeft       = 0;
            this.isInstant         = true;
            this.targetHealthIndex = targetHealthIndex;
            this.affectedUnitTypes = affectedUnitTypes;
        }
    }
}