// rmc-edit: Ported from RMC14.
using Content.Shared.Whitelist;
using Robust.Shared.GameStates;

namespace Content.Shared.Vehicle.Components;

/// <summary>
/// A vehicle that can only operate when a specific key matching a whitelist is inserted.
/// </summary>
[RegisterComponent, NetworkedComponent]
[Access(typeof(VehicleSystem))]
public sealed partial class GenericKeyedVehicleComponent : Component
{
    /// <summary>
    /// The ID corresponding to the container where the "key" must be inserted.
    /// </summary>
    [DataField(required: true)]
    public string ContainerId = string.Empty;

    /// <summary>
    /// A whitelist determining what qualifies as a valid key for this vehicle.
    /// </summary>
    [DataField(required: true)]
    public EntityWhitelist KeyWhitelist = new();

    /// <summary>
    /// If true, prevents keys which do not pass the <see cref="KeyWhitelist"/> from being inserted.
    /// </summary>
    [DataField]
    public bool PreventInvalidInsertion = true;
}
