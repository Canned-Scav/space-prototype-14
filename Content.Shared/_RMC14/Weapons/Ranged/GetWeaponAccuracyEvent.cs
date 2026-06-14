// rmc-edit: Ported from RMC14. Event for calculating weapon accuracy modifiers.
using Content.Goobstation.Maths.FixedPoint;

namespace Content.Shared._RMC14.Weapons.Ranged;

[ByRefEvent]
public record struct GetWeaponAccuracyEvent(
    FixedPoint2 AccuracyMultiplier,
    float Range
);
