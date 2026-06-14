using Robust.Shared.GameStates;

namespace Content.Shared.Vehicle.Components;

/// <summary>
/// Tracking component for handling the operator of a given <see cref="VehicleComponent"/>
/// </summary>
// rmc-edit: Ported from RMC14.
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
// rmc-edit: Access includes both SS14 and RMC14 VehicleSystem
[Access(typeof(Content.Shared.Vehicle.VehicleSystem), typeof(Content.Shared._RMC14.Vehicle.VehicleSystem))]
public sealed partial class VehicleOperatorComponent : Component
{
    /// <summary>
    /// The vehicle we are currently operating.
    /// </summary>
    [DataField, AutoNetworkedField]
    public EntityUid? Vehicle;
}
