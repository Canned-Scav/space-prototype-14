// rmc-edit: Ported subset of CMKeyFunctions from RMC14 needed for vehicle system.
// Only CMUniqueAction is used by VehicleOverchargeSystem.
using Robust.Shared.Input;

namespace Content.Shared._RMC14.Input;

[KeyFunctions]
public sealed class CMKeyFunctions
{
    /// <summary>
    /// Used by vehicles for overcharge action binding.
    /// </summary>
    public static readonly BoundKeyFunction CMUniqueAction = "CMUniqueAction";
}
