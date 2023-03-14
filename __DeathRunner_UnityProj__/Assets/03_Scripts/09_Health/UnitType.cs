using System;

namespace DeathRunner.Health
{
    [Flags]
    public enum UnitType
    {
        None   = 0,
        Small  = 1,
        Medium = 2 | Small,
        Large  = 4 | Medium | Small,
        Boss   = 8 | Large | Medium | Small,
        Player = 16,
        All    = (Small | Medium | Large | Player),
    }
}