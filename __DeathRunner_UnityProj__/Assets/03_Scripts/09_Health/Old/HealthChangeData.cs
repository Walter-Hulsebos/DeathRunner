using System.Collections.Generic;
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
    public struct HealthChangeData : IEqualityComparer<HealthChangeData>
    {
        public F32  delta;
        public F32  secondsLeft;
        public Bool isSingleFrame;
        public readonly U16      targetHealthIndex;
        public readonly UnitType affectedUnitTypes;

        /// <param name="delta">
        /// The health change per second.
        /// `+` heals,
        /// `-` damages,
        /// Per-second values, unless <see cref="isSingleFrame"/> is true, then it's an instant (single-frame) change.
        /// </param>
        /// <param name="durationInSeconds">The amount in seconds this health change should be active. </param>
        /// <param name="targetHealthIndex">The target health index in the <see cref="HealthManager"/>'s health pool.</param>
        /// <param name="affectedUnitTypes">The types of units that are affected by this health change, by default it's ALL. </param>
        ///// <param name="affectsHealth">used to determine whether or not this health change should be performed.</param>
        public HealthChangeData(F32 delta, U16 targetHealthIndex, F32 durationInSeconds, UnitType affectedUnitTypes = UnitType.All)
        {
            this.delta             = delta;
            this.secondsLeft       = durationInSeconds;
            this.isSingleFrame     = (durationInSeconds == 0);
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
            this.isSingleFrame     = true;
            this.targetHealthIndex = targetHealthIndex;
            this.affectedUnitTypes = affectedUnitTypes;
        }
        
        public Bool IsFinished => isSingleFrame ? (secondsLeft < 0) : (secondsLeft <= 0);
        public Bool Equals(HealthChangeData x, HealthChangeData y)
        {
            return x.delta.Equals(y.delta) && x.secondsLeft.Equals(y.secondsLeft) && x.isSingleFrame == y.isSingleFrame && x.targetHealthIndex == y.targetHealthIndex && x.affectedUnitTypes == y.affectedUnitTypes;
        }

        public I32 GetHashCode(HealthChangeData obj)
        {
            unchecked
            {
                I32 __hashCode = obj.delta.GetHashCode();
                __hashCode = (__hashCode * 397) ^ obj.secondsLeft.GetHashCode();
                __hashCode = (__hashCode * 397) ^ obj.isSingleFrame.GetHashCode();
                __hashCode = (__hashCode * 397) ^ obj.targetHealthIndex.GetHashCode();
                __hashCode = (__hashCode * 397) ^ (I32)obj.affectedUnitTypes;
                return __hashCode;
            }
        }
    }
}