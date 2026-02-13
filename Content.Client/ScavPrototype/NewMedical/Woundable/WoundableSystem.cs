using Content.Shared.ScavPrototype.NewMedical.Woundable.Systems;
using Content.Shared.ScavPrototype.NewMedical.Woundable.Components;
using Content.Shared.ScavPrototype.NewMedical.Woundable.Events;
using Content.Shared.Input;
using Content.Shared.Body.Components;
using Content.Shared.Body.Systems;
using Content.Shared.ScavPrototype.NewMedical.Targeting;
using Robust.Client.Player;
using Robust.Shared.Player;
using Robust.Shared.Log;
using Robust.Shared.GameStates;
using Robust.Shared.GameObjects;
using Robust.Shared.Timing;
using System.Reflection.Metadata;

namespace Content.Client.ScavPrototype.NewMedical.Woundable;

public sealed class WoundableSystem : SharedWoundableSystem
{
    [Dependency] private readonly IPlayerManager _playerManager = default!;
    [Dependency] private readonly SharedBodySystem _body = default!;

    public event Action<WoundableComponent, List<TargetBodyPart>>? PartStatusStartup;
    public event Action<WoundablePartChangeEvent>? PartStatusUpdate;
    public event Action? PartStatusShutdown;
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<WoundableComponent, LocalPlayerAttachedEvent>(HandlePlayerAttached);
        SubscribeLocalEvent<WoundableComponent, LocalPlayerDetachedEvent>(HandlePlayerDetached);
        SubscribeLocalEvent<WoundableComponent, ComponentStartup>(OnPartStatusStartup);
        SubscribeLocalEvent<WoundableComponent, ComponentShutdown>(OnPartStatusShutdown);
        SubscribeNetworkEvent<WoundablePartChangeEvent>(OnWoundableIntegrityChange);
    }

    private void HandlePlayerAttached(EntityUid uid, WoundableComponent component, LocalPlayerAttachedEvent args)
    {
        Timer.Spawn(200, () =>
        {
            PartStatusStartup?.Invoke(component, InitParts(uid));
        });
    }

    private void HandlePlayerDetached(EntityUid uid, WoundableComponent component, LocalPlayerDetachedEvent args)
    {
        PartStatusShutdown?.Invoke();
    }

    private void OnPartStatusStartup(EntityUid uid, WoundableComponent component, ComponentStartup args)
    {
        if (_playerManager.LocalEntity != uid)
            return;

        PartStatusStartup?.Invoke(component, InitParts(uid));
    }

    private void OnPartStatusShutdown(EntityUid uid, WoundableComponent component, ComponentShutdown args)
    {
        if (_playerManager.LocalEntity != uid)
            return;

        PartStatusShutdown?.Invoke();
    }

    private void OnWoundableIntegrityChange(WoundablePartChangeEvent args)
    {
        if (!TryGetEntity(args.Uid, out var uid)
            || !_playerManager.LocalEntity.Equals(uid)
            || !args.RefreshUi)
            return;


        PartStatusUpdate?.Invoke(args);
    }

    private List<TargetBodyPart> InitParts(Entity<BodyComponent?> ent)
    {
        if (!Resolve(ent, ref ent.Comp, false) || ent.Comp.RootContainer == null)
            return new List<TargetBodyPart>();

        var parts = new List<TargetBodyPart>();

        foreach (var (partUid, partComp) in _body.GetBodyChildren(ent.Owner, ent.Comp))
        {
            var targetPart = _body.GetTargetBodyPart(partComp.PartType, partComp.Symmetry);

            parts.Add(targetPart);
        }

        return parts;
    }
}
