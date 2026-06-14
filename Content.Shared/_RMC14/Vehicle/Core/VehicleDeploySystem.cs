using System;
using System.Collections.Generic;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Buckle.Components;
using Content.Shared.CombatMode;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Chat;
using Content.Shared.Interaction;
using Content.Shared.Mobs.Components;
using Content.Shared.Popups;
using Content.Shared.Vehicle.Components;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Weapons.Ranged.Systems;
// rmc-edit: removed using Content.Shared._RMC14.Chat and Content.Shared._RMC14.Sentry
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Timing;
using Robust.Shared.Containers;
using Content.Shared.Mobs;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Shared._RMC14.Vehicle;

public sealed class VehicleDeploySystem : EntitySystem
{
    private static readonly EntProtoId HardpointTypeCannon = "HardpointTypeCannon";

    private readonly List<VehicleMountedSlot> _mountedSlotsBuffer = new();

    [Dependency] private readonly SharedActionsSystem _actions = default!;
    [Dependency] private readonly SharedCombatModeSystem _combatMode = default!;
    [Dependency] private readonly SharedGunSystem _guns = default!;
    [Dependency] private readonly SharedInteractionSystem _interaction = default!;
    [Dependency] private readonly MetaDataSystem _meta = default!;
    [Dependency] private readonly INetManager _net = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    // rmc-edit: removed _rmcChat and _targeting (SentryTargetingSystem)
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly VehicleTopologySystem _topology = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;
    [Dependency] private readonly VehicleTurretSystem _turret = default!;
    [Dependency] private readonly VehicleSystem _vehicle = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<StrapComponent, StrappedEvent>(OnDriverStrapped);
        SubscribeLocalEvent<StrapComponent, UnstrappedEvent>(OnDriverUnstrapped);
        SubscribeLocalEvent<VehicleDeployActionComponent, VehicleDeployActionEvent>(OnDeployAction);
        SubscribeLocalEvent<VehicleDeployActionComponent, ComponentShutdown>(OnDeployActionShutdown);
        SubscribeLocalEvent<VehicleDeployableComponent, VehicleCanRunEvent>(OnVehicleCanRun);
        SubscribeLocalEvent<HardpointSlotsChangedEvent>(OnHardpointSlotsChanged);
        SubscribeLocalEvent<HardpointItemComponent, AttemptShootEvent>(OnDeployableAttemptShoot);
    }

    private void OnDriverStrapped(Entity<StrapComponent> ent, ref StrappedEvent args)
    {
        if (_net.IsClient)
            return;

        if (!HasComp<VehicleDriverSeatComponent>(ent.Owner))
            return;

        if (!_vehicle.TryGetVehicleFromInterior(ent.Owner, out var vehicle) || vehicle == null)
            return;

        if (!TryComp(vehicle.Value, out VehicleDeployableComponent? deployable))
            return;

        EnableDeployAction(args.Buckle.Owner, vehicle.Value, deployable);
    }

    private void OnDriverUnstrapped(Entity<StrapComponent> ent, ref UnstrappedEvent args)
    {
        if (_net.IsClient)
            return;

        if (!HasComp<VehicleDriverSeatComponent>(ent.Owner))
            return;

        if (!_vehicle.TryGetVehicleFromInterior(ent.Owner, out var vehicle) || vehicle == null)
            return;

        DisableDeployAction(args.Buckle.Owner, vehicle.Value);
    }

    private void EnableDeployAction(EntityUid user, EntityUid vehicle, VehicleDeployableComponent deployable)
    {
        var actionComp = EnsureComp<VehicleDeployActionComponent>(user);
        actionComp.Vehicle = vehicle;

        if (actionComp.Action == null)
            actionComp.Action = _actions.AddAction(user, actionComp.ActionId);

        UpdateDeployActionState(user, actionComp, deployable);
        Dirty(user, actionComp);
    }

    private void DisableDeployAction(EntityUid user, EntityUid vehicle)
    {
        if (!TryComp(user, out VehicleDeployActionComponent? actionComp))
            return;

        if (actionComp.Vehicle != vehicle)
            return;

        if (actionComp.Action is { } action)
        {
            RemoveAndDeleteDeployAction(user, action);
            actionComp.Action = null;
        }

        RemCompDeferred<VehicleDeployActionComponent>(user);
    }

    private void OnDeployActionShutdown(Entity<VehicleDeployActionComponent> ent, ref ComponentShutdown args)
    {
        if (ent.Comp.Action is { } action)
            RemoveAndDeleteDeployAction(ent.Owner, action);
    }

    private void RemoveAndDeleteDeployAction(EntityUid user, EntityUid action)
    {
        if (TerminatingOrDeleted(action))
            return;

        _actions.RemoveAction(user, action);

        if (_net.IsClient)
            return;

        if (Exists(action))
            QueueDel(action);
    }

    private void OnDeployAction(Entity<VehicleDeployActionComponent> ent, ref VehicleDeployActionEvent args)
    {
        if (_net.IsClient)
            return;

        if (args.Handled || args.Performer != ent.Owner)
            return;

        args.Handled = true;

        if (ent.Comp.Vehicle is not { } vehicle)
            return;

        if (!TryComp(vehicle, out VehicleDeployableComponent? deployable))
            return;

        if (!TryComp(vehicle, out VehicleComponent? vehicleComp) ||
            vehicleComp.Operator != ent.Owner)
        {
            _popup.PopupClient(Loc.GetString("rmc-vehicle-deploy-not-driver"), ent.Owner, ent.Owner, PopupType.SmallCaution);
            return;
        }

        if (deployable.Deploying)
            return;

        var deployingTo = !deployable.Deployed;
        if (deployingTo && !TryGetVehicleTurret(vehicle, out _))
        {
            _popup.PopupClient(Loc.GetString("rmc-vehicle-deploy-requires-turret"), ent.Owner, ent.Owner, PopupType.SmallCaution);
            return;
        }

        deployable.Deploying = true;
        deployable.DeployingTo = deployingTo;
        deployable.Deployer = ent.Owner;
        var delay = deployingTo ? deployable.DeployTime : deployable.UndeployTime;
        deployable.DeployEndTime = _timing.CurTime + delay;
        deployable.AutoTarget = null;
        deployable.NextAutoTargetTime = TimeSpan.Zero;
        deployable.AutoSpinInitialized = false;
        Dirty(vehicle, deployable);

        if (!deployingTo && TryGetVehicleTurret(vehicle, out var turretUid))
        {
            var vehicleRot = _transform.GetWorldRotation(vehicle);
            _turret.TrySetTargetRotationWorld(turretUid, vehicleRot);
        }

        UpdateDeployActionState(ent.Owner, ent.Comp, deployable);

        var actionEntity = ent.Comp.Action;
        if (actionEntity != null)
            _actions.SetCooldown(actionEntity.Value, delay);

        var popupKey = deployingTo ? "rmc-vehicle-deploy-start" : "rmc-vehicle-undeploy-start";
        var startMsg = Loc.GetString(popupKey);
        _popup.PopupClient(startMsg, ent.Owner, ent.Owner, PopupType.Small);
        SendDeployChat(ent.Owner, vehicle, startMsg);
    }

    private void OnVehicleCanRun(Entity<VehicleDeployableComponent> ent, ref VehicleCanRunEvent args)
    {
        if (!args.CanRun)
            return;

        if (ent.Comp.Deploying || ent.Comp.Deployed)
            args.CanRun = false;
    }

    private void UpdateDriverActionState(EntityUid vehicle, VehicleDeployableComponent deployable)
    {
        if (!TryComp(vehicle, out VehicleComponent? vehicleComp) || vehicleComp.Operator == null)
            return;

        var driver = vehicleComp.Operator.Value;
        if (!TryComp(driver, out VehicleDeployActionComponent? actionComp))
            return;

        if (actionComp.Vehicle != vehicle)
            return;

        UpdateDeployActionState(driver, actionComp, deployable);
        Dirty(driver, actionComp);
    }

    private void UpdateDeployActionState(EntityUid user, VehicleDeployActionComponent actionComp, VehicleDeployableComponent deployable)
    {
        if (actionComp.Action == null)
            return;

        var canDeploy = true;
        EntityUid? turretUid = null;
        if (actionComp.Vehicle is { } vehicle)
        {
            var hasTurret = TryGetVehicleTurret(vehicle, out var foundTurret);
            canDeploy = deployable.Deployed || hasTurret;
            turretUid = hasTurret ? foundTurret : null;

            if (actionComp.Action is { } actionEntity && TryComp(actionEntity, out ActionComponent? actionComponent))
                _actions.SetEntityIcon((actionEntity, actionComponent), turretUid ?? vehicle);
        }

        var actionEntityUid = actionComp.Action.Value;
        _actions.SetToggled(actionEntityUid, deployable.Deployed || deployable.Deploying);
        _actions.SetEnabled(actionEntityUid, !deployable.Deploying && canDeploy);

        UpdateDeployActionText(actionEntityUid, deployable);
    }

    private void UpdateDeployActionText(EntityUid action, VehicleDeployableComponent deployable)
    {
        string nameKey;
        string descKey;

        if (deployable.Deploying)
        {
            if (deployable.DeployingTo)
            {
                nameKey = "rmc-vehicle-deploy-action-name-deploying";
                descKey = "rmc-vehicle-deploy-action-desc-deploying";
            }
            else
            {
                nameKey = "rmc-vehicle-deploy-action-name-undeploying";
                descKey = "rmc-vehicle-deploy-action-desc-undeploying";
            }
        }
        else if (deployable.Deployed)
        {
            nameKey = "rmc-vehicle-deploy-action-name-undeploy";
            descKey = "rmc-vehicle-deploy-action-desc-undeploy";
        }
        else
        {
            nameKey = "rmc-vehicle-deploy-action-name-deploy";
            descKey = "rmc-vehicle-deploy-action-desc-deploy";
        }

        _meta.SetEntityName(action, Loc.GetString(nameKey));
        _meta.SetEntityDescription(action, Loc.GetString(descKey));
    }

    private void ClearDriverDeployCooldown(EntityUid vehicle)
    {
        if (!TryComp(vehicle, out VehicleComponent? vehicleComp) || vehicleComp.Operator == null)
            return;

        var driver = vehicleComp.Operator.Value;
        if (!TryComp(driver, out VehicleDeployActionComponent? actionComp) || actionComp.Action == null)
            return;

        _actions.ClearCooldown(actionComp.Action.Value);
    }

    private void OnHardpointSlotsChanged(HardpointSlotsChangedEvent args)
    {
        if (_net.IsClient)
            return;

        if (!TryComp(args.Vehicle, out VehicleDeployableComponent? deployable))
            return;

        UpdateDriverActionState(args.Vehicle, deployable);
    }

    private void OnDeployableAttemptShoot(Entity<HardpointItemComponent> ent, ref AttemptShootEvent args)
    {
        if (_net.IsClient && !_timing.IsFirstTimePredicted)
            return;

        if (args.Cancelled)
            return;

        if (ent.Comp.HardpointType != HardpointTypeCannon)
            return;

        if (!TryGetVehicleFromContained(ent.Owner, out var vehicle))
        {
            args.Cancelled = true;
            args.ResetCooldown = true;
            return;
        }

        if (!TryComp(vehicle, out VehicleDeployableComponent? deployable))
            return;

        if (!TryComp(vehicle, out VehicleDeployGatedHardpointsComponent? gated) ||
            !IsBlockedHardpoint(gated, ent.Comp.HardpointType))
        {
            return;
        }

        if (!deployable.Deployed)
        {
            args.Cancelled = true;
            args.ResetCooldown = true;
        }
    }

    private static bool IsBlockedHardpoint(VehicleDeployGatedHardpointsComponent gated, EntProtoId hardpointType)
    {
        if (hardpointType == default)
            return false;

        foreach (var blocked in gated.BlockedHardpoints)
        {
            if (blocked == hardpointType)
                return true;
        }

        return false;
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        if (_net.IsClient)
            return;

        var query = EntityQueryEnumerator<VehicleDeployableComponent, TransformComponent>();
        while (query.MoveNext(out var vehicle, out var deployable, out _))
        {
            var now = _timing.CurTime;
            if (deployable.Deploying)
            {
                if (now >= deployable.DeployEndTime)
                {
                    var finishedDeploy = deployable.DeployingTo;
                    var deployer = deployable.Deployer;

                    deployable.Deploying = false;
                    deployable.DeployingTo = false;
                    deployable.DeployEndTime = TimeSpan.Zero;
                    deployable.Deployed = finishedDeploy;

                    if (!deployable.Deployed)
                    {
                        deployable.Deployer = null;
                        deployable.TargetingDeployer = null;
                        deployable.AutoTarget = null;
                        deployable.AutoSpinInitialized = false;
                    }

                    Dirty(vehicle, deployable);
                    UpdateDriverActionState(vehicle, deployable);
                    ClearDriverDeployCooldown(vehicle);

                    var popupKey = finishedDeploy ? "rmc-vehicle-deploy-finish" : "rmc-vehicle-undeploy-finish";
                    if (deployer != null)
                    {
                        var finishMsg = Loc.GetString(popupKey);
                        _popup.PopupClient(finishMsg, deployer.Value, deployer.Value, PopupType.Small);
                        SendDeployChat(deployer.Value, vehicle, finishMsg);
                    }
                    else
                        _popup.PopupEntity(Loc.GetString(popupKey), vehicle, PopupType.Small);
                }

                continue;
            }

            if (!deployable.Deployed || !deployable.AutoTurretEnabled)
            {
                deployable.AutoSpinInitialized = false;
                continue;
            }

            // rmc-edit: removed auto-targeting logic (requires SentryTargetingSystem / IFF)
            // Auto-spin still works without auto-targeting
            if (!TryFindAutoGun(vehicle, out var gunUid, out _))
                continue;

            if (deployable.AutoSpinSpeed > 0f)
            {
                if (!deployable.AutoSpinInitialized)
                {
                    deployable.AutoSpinWorldRotation = GetTurretWorldRotation(gunUid, vehicle);
                    deployable.AutoSpinInitialized = true;
                }

                var delta = Angle.FromDegrees(deployable.AutoSpinSpeed * frameTime);
                deployable.AutoSpinWorldRotation = (deployable.AutoSpinWorldRotation + delta).Reduced();
                _turret.TrySetTargetRotationWorld(gunUid, deployable.AutoSpinWorldRotation);
            }
        }
    }

    private bool TryFindAutoGun(EntityUid vehicle, out EntityUid gunUid, out GunComponent gunComp)
    {
        gunUid = default;
        gunComp = default!;

        EntityUid? fallbackGun = null;
        GunComponent? fallbackComp = null;

        _topology.GetMountedSlots(vehicle, _mountedSlotsBuffer);
        foreach (var mountedSlot in _mountedSlotsBuffer)
        {
            if (mountedSlot.Item is not { } installed)
                continue;

            if (TryGetGunCandidate(installed, out var directGun, out var directComp))
            {
                if (HasAmmo(directGun))
                {
                    gunUid = directGun;
                    gunComp = directComp;
                    return true;
                }

                fallbackGun ??= directGun;
                fallbackComp ??= directComp;
            }
        }

        if (fallbackGun != null && fallbackComp != null)
        {
            gunUid = fallbackGun.Value;
            gunComp = fallbackComp;
            return true;
        }

        return false;
    }

    private bool TryGetGunCandidate(EntityUid uid, out EntityUid gunUid, out GunComponent gunComp)
    {
        gunUid = uid;
        gunComp = default!;

        if (!TryComp(uid, out GunComponent? gun) || !HasComp<VehicleTurretComponent>(uid))
            return false;

        gunComp = gun;
        return true;
    }

    private bool HasAmmo(EntityUid gunUid)
    {
        if (!HasComp<GunComponent>(gunUid))
            return false;

        var ammoEv = new GetAmmoCountEvent();
        RaiseLocalEvent(gunUid, ref ammoEv);
        return ammoEv.Capacity <= 0 || ammoEv.Count > 0;
    }

    // rmc-edit: removed FindAutoTarget and IsValidAutoTarget - required SentryTargetingSystem (CM-specific)

    private Angle GetTurretWorldRotation(EntityUid turretUid, EntityUid vehicle)
    {
        if (!TryComp(turretUid, out VehicleTurretComponent? turret))
            return _transform.GetWorldRotation(vehicle);

        var vehicleRot = _transform.GetWorldRotation(vehicle);
        return (turret.WorldRotation + vehicleRot).Reduced();
    }

    private void SendDeployChat(EntityUid deployer, EntityUid vehicle, string message)
    {
        // rmc-edit: replaced SharedCMChatSystem with standard popup (CM chat relay not available)
        _popup.PopupEntity(message, vehicle, PopupType.Small);
    }

}
