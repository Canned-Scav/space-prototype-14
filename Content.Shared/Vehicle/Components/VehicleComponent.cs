// rmc-edit: This file replaces the base SS14 VehicleComponent with the RMC14 extended version.
// The RMC14 version adds Operator tracking, damage transfer, and movement kind.
using Content.Shared.Damage;
using Content.Shared.Whitelist;
using JetBrains.Annotations;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared.Vehicle.Components;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
// rmc-edit: Access includes both the base SS14 VehicleSystem and RMC14's VehicleSystem
[Access(typeof(Content.Shared.Vehicle.VehicleSystem), typeof(Content.Shared._RMC14.Vehicle.VehicleSystem))]
public sealed partial class VehicleComponent : Component
{
    [DataField, AutoNetworkedField]
    public EntityUid? Operator;

    [DataField, AutoNetworkedField]
    public EntityWhitelist? OperatorWhitelist;

    [DataField, AutoNetworkedField]
    public bool TransferDamage = true;

    [DataField, AutoNetworkedField]
    public DamageModifierSet? TransferDamageModifier;

    [DataField, AutoNetworkedField]
    public VehicleMovementKind MovementKind = VehicleMovementKind.Standard;
}

[Serializable, NetSerializable]
public enum VehicleVisuals : byte
{
    HasOperator,
    CanRun
}

[Serializable, NetSerializable]
public enum VehicleMovementKind : byte
{
    Standard,
    Grid
}

[ByRefEvent, UsedImplicitly]
public readonly record struct OnVehicleEnteredEvent(Entity<VehicleComponent> Vehicle, EntityUid Operator);

[ByRefEvent, UsedImplicitly]
public readonly record struct OnVehicleExitedEvent(Entity<VehicleComponent> Vehicle, EntityUid Operator);

[ByRefEvent, UsedImplicitly]
public readonly record struct VehicleOperatorSetEvent(EntityUid? NewOperator, EntityUid? OldOperator);

[ByRefEvent, UsedImplicitly]
public record struct VehicleCanRunEvent(Entity<VehicleComponent> Vehicle, bool CanRun = true);
