using System;
using System.Numerics;
using Content.Shared.Movement.Components;
using Content.Shared.Vehicle.Components;
// rmc-edit: removed using Content.Shared._RMC14.Xenonids
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Content.Shared.Movement.Systems;

namespace Content.Shared.Vehicle;

public sealed partial class GridVehicleMoverSystem : EntitySystem
{
    private Vector2i GetInputDirection(InputMoverComponent input)
    {
        var buttons = input.HeldMoveButtons;
        var dir = Vector2i.Zero;

        if ((buttons & MoveButtons.Up) != 0) dir += new Vector2i(0, 1);
        if ((buttons & MoveButtons.Down) != 0) dir += new Vector2i(0, -1);
        if ((buttons & MoveButtons.Right) != 0) dir += new Vector2i(1, 0);
        if ((buttons & MoveButtons.Left) != 0) dir += new Vector2i(-1, 0);

        if (dir == Vector2i.Zero)
            return dir;

        if (dir.X != 0 && dir.Y != 0)
        {
            if (Math.Abs(dir.X) >= Math.Abs(dir.Y))
                dir = new Vector2i(Math.Sign(dir.X), 0);
            else
                dir = new Vector2i(0, Math.Sign(dir.Y));
        }

        return dir;
    }

    private Vector2i GetMoverInput(EntityUid uid, GridVehicleMoverComponent mover, VehicleComponent vehicle, out bool pushing)
    {
        pushing = false;
        if (vehicle.Operator is { } op && TryComp<InputMoverComponent>(op, out var inputComp))
        {
            // rmc-edit: removed _activeXenoPushers.Remove
            return GetInputDirection(inputComp);
        }

        if (vehicle.Operator != null)
        {
            // rmc-edit: removed _activeXenoPushers.Remove
            return Vector2i.Zero;
        }

        // rmc-edit: xeno push system not ported - vehicles can only be driven by operators
        if (mover.IsPushMove &&
            mover.PushDirection != Vector2i.Zero &&
            mover.CurrentSpeed > MinVehicleSpeed)
        {
            pushing = true;
            return Vector2i.Zero;
        }

        return Vector2i.Zero;
    }

    // rmc-edit: removed TryGetActivePusher - required XenoComponent check
    // rmc-edit: removed GetPushDirection - only used by xeno push
    // rmc-edit: removed CanXenoPushVehicle - required RMCSizeStunSystem

    private bool CanPushNow(GridVehicleMoverComponent mover)
    {
        if (mover.PushCooldown <= 0f)
            return true;

        return _timing.CurTime >= mover.NextPushTime;
    }
}
