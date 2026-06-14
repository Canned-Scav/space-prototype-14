using Robust.Shared.GameStates;

namespace Content.Shared.Vehicle.Components;

[RegisterComponent, NetworkedComponent]
[Access(typeof(Content.Shared._RMC14.Vehicle.VehicleSystem))]
public sealed partial class GridVehicleOperatorComponent : Component
{
}
