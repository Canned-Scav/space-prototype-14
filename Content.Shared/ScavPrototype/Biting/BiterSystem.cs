using Content.Shared.Actions;
using Content.Shared.DoAfter;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Damage;
using Content.Shared.Damage.Components;
using Content.Shared.Damage.Systems;
using Content.Shared.Body.Components;
using Content.Shared.Body.Systems;
using Content.Shared.Popups;
using Content.Shared.Whitelist;
using Content.Shared.IdentityManagement;
using Content.Shared.Chemistry.Components;
using Content.Shared.CombatMode;
using Content.Shared.Nutrition.EntitySystems;
using Content.Shared.Nutrition.Components;
using Content.Shared.Random.Helpers;
using Content.Shared.Gibbing;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;

namespace Content.Shared.ScavPrototype.Biting;

public sealed class BiterSystem : EntitySystem
{
    [Dependency] private readonly SharedActionsSystem _actionsSystem = default!;
    [Dependency] protected readonly DamageableSystem _damageable = default!;
    [Dependency] private readonly SharedBloodstreamSystem _bloodstreamSystem = default!;
    [Dependency] private readonly SharedPopupSystem _popupSystem = default!;
    [Dependency] private readonly SharedDoAfterSystem _doAfterSystem = default!;
    [Dependency] private readonly SharedCombatModeSystem _combatModeSystem = default!;
    [Dependency] private readonly HungerSystem _hungerSystem = default!;
    [Dependency] private readonly MobStateSystem _mobStateSystem = default!;
    [Dependency] private readonly IGameTiming _gameTiming = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly GibbingSystem _gib = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<BiterComponent, MapInitEvent>(OnInit);
        SubscribeLocalEvent<BiterComponent, ComponentShutdown>(OnShutdown);
        SubscribeLocalEvent<BiterComponent, BiteActionEvent>(OnBiteAction);
        SubscribeLocalEvent<BiterComponent, BiteDoAfterEvent>(OnDoAfter);
    }

    private void OnInit(Entity<BiterComponent> ent, ref MapInitEvent args)
    {
        _actionsSystem.AddAction(ent.Owner, ref ent.Comp.BiteActionEntity, ent.Comp.BiteAction);
    }

    private void OnShutdown(Entity<BiterComponent> ent, ref ComponentShutdown args)
    {
        _actionsSystem.RemoveAction(ent.Owner, ent.Comp.BiteActionEntity);
    }

    private void OnBiteAction(Entity<BiterComponent> ent, ref BiteActionEvent args)
    {
        if (args.Handled || !TryComp<MobStateComponent>(args.Target, out var mobStateComp)) {
            _popupSystem.PopupClient(Loc.GetString("bite-action-popup-message-fail"), ent.Owner, ent.Owner);
            return;
        }

        args.Handled = true;

        var type = BiteType.Normal;

        if (_mobStateSystem.IsAlive(args.Target, mobStateComp))
        {
            if (TryComp<CombatModeComponent>(ent.Owner, out var combatModeComp) && _combatModeSystem.IsInCombatMode(ent.Owner))
            {
                type = BiteType.Strong;
                _popupSystem.PopupClient(Loc.GetString("strong-bite-action-popup-message-succes", ("target", Identity.Entity(args.Target, EntityManager))), ent.Owner, ent.Owner);
                _popupSystem.PopupClient(Loc.GetString("strong-bite-action-popup-message-succes-other", ("user", Identity.Entity(ent.Owner, EntityManager))), args.Target, args.Target, PopupType.MediumCaution);
            }
            else
            {
                _popupSystem.PopupClient(Loc.GetString("bite-action-popup-message-succes", ("target", Identity.Entity(args.Target, EntityManager))), ent.Owner, ent.Owner);
                _popupSystem.PopupClient(Loc.GetString("bite-action-popup-message-succes-other", ("user", Identity.Entity(ent.Owner, EntityManager))), args.Target, args.Target);
            }
        }
        else
        {
            type = BiteType.Dead;
            _popupSystem.PopupPredicted(Loc.GetString("dead-bite-action-popup-message-succes", ("user", Identity.Entity(ent.Owner, EntityManager)), ("target", Identity.Entity(args.Target, EntityManager))), ent.Owner, args.Target, PopupType.MediumCaution);
        }

        _doAfterSystem.TryStartDoAfter(new DoAfterArgs(EntityManager, ent.Owner, ent.Comp.BiteTypes[type].BiteTime, new BiteDoAfterEvent(type), ent.Owner, target: args.Target, used: ent.Owner)
        {
            BreakOnMove = true,
        });
    }

    private void OnDoAfter(Entity<BiterComponent> ent, ref BiteDoAfterEvent args)
    {
        if (args.Handled || args.Cancelled || args.Target == null)
            return;

        var target = args.Target.Value;

        if (!TryComp<BloodstreamComponent>(target, out var streamComp) || !TryComp<HungerComponent>(ent.Owner, out var hungerComp))
            return;

        args.Handled = true;

        switch (args.Type)
        {
            case BiteType.Normal:
                _popupSystem.PopupPredicted(Loc.GetString("bite-complete-popup-message", ("user", Identity.Entity(ent.Owner, EntityManager)), ("target", Identity.Entity(target, EntityManager))), ent.Owner, target);
                break;

            case BiteType.Strong:
                _popupSystem.PopupPredicted(Loc.GetString("strong-bite-complete-popup-message", ("user", Identity.Entity(ent.Owner, EntityManager)), ("target", Identity.Entity(target, EntityManager))), ent.Owner, target, PopupType.MediumCaution);
                break;

            case BiteType.Dead:
                _popupSystem.PopupPredicted(Loc.GetString("dead-bite-complete-popup-message", ("user", Identity.Entity(ent.Owner, EntityManager)), ("target", Identity.Entity(target, EntityManager))), ent.Owner, target, PopupType.MediumCaution);
                RemoveOneButcherable(target);
                break;
        }

        var biteEntry = ent.Comp.BiteTypes[args.Type];

        _damageable.TryChangeDamage(target, biteEntry.Damage, origin: ent.Owner);

        var (bloodReagent, _) = streamComp.BloodReferenceSolution.Contents[0];
        var bloodInjection = new Solution(bloodReagent.Prototype, biteEntry.TransferAmount);

        _bloodstreamSystem.TryModifyBloodLevel(target, -biteEntry.TransferAmount);
        _bloodstreamSystem.TryAddToBloodstream(ent.Owner, bloodInjection);

        _hungerSystem.ModifyHunger(ent.Owner, biteEntry.HungerAmount);

        _audio.PlayPredicted(biteEntry.BiteSound, ent.Owner, ent.Owner);
    }

    private void RemoveOneButcherable(EntityUid target)
    {
        if (!TryComp<ButcherableComponent>(target, out var butcherable))
            return;

        var seed = SharedRandomExtensions.HashCodeCombine((int)_gameTiming.CurTick.Value, GetNetEntity(target).Id);
        var rand = new System.Random(seed);

        var index = rand.Next(butcherable.SpawnedEntities.Count);
        var entry = butcherable.SpawnedEntities[index];

        // Decrease the amount since we spawned an entity from that entry.
        entry.Amount--;

        // Remove the entry if its new amount is zero, or update it.
        if (entry.Amount <= 0)
            butcherable.SpawnedEntities.RemoveAt(index);
        else
            butcherable.SpawnedEntities[index] = entry;

        Dirty(target, butcherable);

        if (butcherable.SpawnedEntities.Count == 0)
            _gib.Gib(target, true);
    }

}


public sealed partial class BiteActionEvent : EntityTargetActionEvent;

[Serializable, NetSerializable]
public sealed partial class BiteDoAfterEvent : DoAfterEvent
{
    [DataField]
    public BiteType Type;

    private BiteDoAfterEvent()
    {
    }

    public BiteDoAfterEvent(BiteType type)
    {
        Type = type;
    }

    public override DoAfterEvent Clone() => this;
}
