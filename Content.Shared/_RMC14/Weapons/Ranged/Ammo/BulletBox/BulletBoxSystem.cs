using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Weapons.Ranged.Ammo.BulletBox;

// rmc-edit: Ported from RMC14. BulletBoxComponent tracks ammo in a crate used for reloading vehicle guns.
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
[Access(typeof(BulletBoxSystem))]
public sealed partial class BulletBoxComponent : Component
{
    [DataField, AutoNetworkedField]
    public int Amount = 600;

    [DataField, AutoNetworkedField]
    public int Max = 600;

    [DataField(required: true), AutoNetworkedField]
    public EntProtoId BulletType;

    [DataField, AutoNetworkedField]
    public string? UsedIn;

    [DataField, AutoNetworkedField]
    public TimeSpan Delay = TimeSpan.FromSeconds(1.5);
}

// rmc-edit: Marks ammo providers (magazines/clips) that can be refilled by a BulletBox.
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
[Access(typeof(BulletBoxSystem))]
public sealed partial class RefillableByBulletBoxComponent : Component
{
    [DataField(required: true), AutoNetworkedField]
    public EntProtoId? BulletType;
}

[Serializable, NetSerializable]
public sealed partial class BulletBoxTransferDoAfterEvent : SimpleDoAfterEvent
{
    public readonly bool ToBox;

    public BulletBoxTransferDoAfterEvent(bool toBox)
    {
        ToBox = toBox;
    }
}

[Serializable, NetSerializable]
public enum BulletBoxLayers
{
    Fill,
}

[Serializable, NetSerializable]
public enum BulletBoxVisuals
{
    Empty = 0,
    Low,
    Medium,
    High,
    Full,
}

// rmc-edit: Ported from RMC14. Handles interactions between BulletBox crates and ammo providers.
public sealed class BulletBoxSystem : EntitySystem
{
    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;
    [Dependency] private readonly SharedDoAfterSystem _doAfter = default!;
    [Dependency] private readonly SharedGunSystem _gun = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly SharedHandsSystem _hands = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<BulletBoxComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<BulletBoxComponent, ExaminedEvent>(OnExamined);
        SubscribeLocalEvent<BulletBoxComponent, InteractUsingEvent>(OnInteractUsing);
        SubscribeLocalEvent<BulletBoxComponent, BulletBoxTransferDoAfterEvent>(OnTransferDoAfter);
        SubscribeLocalEvent<BulletBoxComponent, GetVerbsEvent<AlternativeVerb>>(OnGetAlternativeVerbs);
    }

    private void OnMapInit(Entity<BulletBoxComponent> ent, ref MapInitEvent args)
    {
        UpdateAppearance(ent);
    }

    private void OnExamined(Entity<BulletBoxComponent> ent, ref ExaminedEvent args)
    {
        using (args.PushGroup(nameof(BulletBoxComponent)))
        {
            args.PushText(Loc.GetString("rmc-bullet-box-amount", ("amount", ent.Comp.Amount)));

            if (!string.IsNullOrWhiteSpace(ent.Comp.UsedIn))
                args.PushText(Loc.GetString("rmc-bullet-box-used-in", ("vehicle", ent.Comp.UsedIn)));
        }
    }

    private void OnGetAlternativeVerbs(Entity<BulletBoxComponent> ent, ref GetVerbsEvent<AlternativeVerb> args)
    {
        var user = args.User;
        if (!_hands.TryGetActiveItem(user, out var usedId))
            return;

        var used = new Entity<RefillableByBulletBoxComponent?, BallisticAmmoProviderComponent?>(usedId.Value, null, null);
        if (!Resolve(used, ref used.Comp1, ref used.Comp2, false))
            return;

        args.Verbs.Add(new AlternativeVerb
        {
            Act = () =>
            {
                if (!CanTransferPopup(ent, user, ref used, true))
                    return;

                var ev = new BulletBoxTransferDoAfterEvent(true);
                var doAfter = new DoAfterArgs(EntityManager, user, ent.Comp.Delay, ev, ent, ent, usedId)
                {
                    BreakOnMove = true,
                    BreakOnDropItem = true,
                    NeedHand = true,
                };
                _doAfter.TryStartDoAfter(doAfter);
            },
            Text = Loc.GetString("rmc-bullet-box-transferto"),
            Impact = LogImpact.Low,
        });
    }

    private void OnInteractUsing(Entity<BulletBoxComponent> ent, ref InteractUsingEvent args)
    {
        var used = new Entity<RefillableByBulletBoxComponent?, BallisticAmmoProviderComponent?>(args.Used, null, null);
        if (!Resolve(used, ref used.Comp1, ref used.Comp2, false))
            return;

        args.Handled = true;
        var user = args.User;

        if (!CanTransferPopup(ent, user, ref used, false))
            return;

        var ev = new BulletBoxTransferDoAfterEvent(false);
        var doAfter = new DoAfterArgs(EntityManager, user, ent.Comp.Delay, ev, ent, ent, args.Used)
        {
            BreakOnMove = true,
            BreakOnDropItem = true,
            NeedHand = true,
        };
        _doAfter.TryStartDoAfter(doAfter);
    }

    private void OnTransferDoAfter(Entity<BulletBoxComponent> ent, ref BulletBoxTransferDoAfterEvent args)
    {
        if (args.Cancelled || args.Handled || args.Used is not { } usedId)
            return;

        args.Handled = true;

        var user = args.User;
        var used = new Entity<RefillableByBulletBoxComponent?, BallisticAmmoProviderComponent?>(usedId, null, null);
        var transferToBox = args.ToBox;

        if (!CanTransferPopup(ent, user, ref used, transferToBox) || used.Comp2 == null)
            return;

        int transfer;
        if (!transferToBox)
        {
            transfer = used.Comp2.Capacity - used.Comp2.Count;
            if (transfer <= 0)
                return;

            transfer = Math.Min(transfer, ent.Comp.Amount);
            _gun.SetBallisticUnspawned((used, used.Comp2), used.Comp2.UnspawnedCount + transfer);
            ent.Comp.Amount -= transfer;
        }
        else
        {
            transfer = ent.Comp.Max - ent.Comp.Amount;
            if (transfer <= 0)
                return;

            transfer = Math.Min(transfer, used.Comp2.Count);
            _gun.SetBallisticUnspawned((used, used.Comp2), used.Comp2.UnspawnedCount - transfer);
            ent.Comp.Amount += transfer;
        }

        _popup.PopupClient(Loc.GetString("rmc-bullet-box-transfer-done", ("amount", transfer), ("used", ent)), ent, user);
        Dirty(ent);
        UpdateAppearance(ent);
    }

    private bool CanTransferPopup(
        Entity<BulletBoxComponent> box,
        EntityUid user,
        ref Entity<RefillableByBulletBoxComponent?, BallisticAmmoProviderComponent?> used,
        bool transferToBox)
    {
        if (!Resolve(used, ref used.Comp1, ref used.Comp2, false))
            return false;

        string? popup = null;

        if (box.Comp.BulletType != used.Comp1!.BulletType)
            popup = Loc.GetString("rmc-bullet-box-wrong-rounds");

        if (!transferToBox)
        {
            if (used.Comp2!.Count >= used.Comp2.Capacity)
                popup = Loc.GetString("rmc-bullet-box-mag-full");
            if (box.Comp.Amount <= 0)
                popup = Loc.GetString("rmc-bullet-box-box-empty");
        }
        else
        {
            if (used.Comp2!.Count <= 0)
                popup = Loc.GetString("rmc-bullet-box-mag-empty");
            if (box.Comp.Amount >= box.Comp.Max)
                popup = Loc.GetString("rmc-bullet-box-box-full");
        }

        if (popup is not null)
        {
            _popup.PopupClient(popup, box, user);
            return false;
        }

        return true;
    }

    private void UpdateAppearance(Entity<BulletBoxComponent> ent)
    {
        var visual = ((double) ent.Comp.Amount / ent.Comp.Max) switch
        {
            >= 1   => BulletBoxVisuals.Full,
            >= 0.66 => BulletBoxVisuals.High,
            >= 0.33 => BulletBoxVisuals.Medium,
            > 0    => BulletBoxVisuals.Low,
            _      => BulletBoxVisuals.Empty,
        };

        _appearance.SetData(ent, BulletBoxLayers.Fill, visual);
    }

    /// <summary>Consume <paramref name="amount"/> rounds from the box. Returns false if not enough.</summary>
    public bool TryConsume(Entity<BulletBoxComponent> ent, int amount)
    {
        if (amount <= 0 || ent.Comp.Amount < amount)
            return false;

        ent.Comp.Amount -= amount;
        Dirty(ent);
        UpdateAppearance(ent);
        return true;
    }

    /// <summary>Add <paramref name="amount"/> rounds to the box. Returns false if it would overflow.</summary>
    public bool TryAdd(Entity<BulletBoxComponent> ent, int amount)
    {
        if (amount <= 0 || ent.Comp.Amount + amount > ent.Comp.Max)
            return false;

        ent.Comp.Amount += amount;
        Dirty(ent);
        UpdateAppearance(ent);
        return true;
    }

    /// <summary>Set the box to exactly <paramref name="amount"/> rounds.</summary>
    public bool TrySetAmount(Entity<BulletBoxComponent> ent, int amount)
    {
        if (amount < 0 || amount > ent.Comp.Max)
            return false;

        ent.Comp.Amount = amount;
        Dirty(ent);
        UpdateAppearance(ent);
        return true;
    }
}
