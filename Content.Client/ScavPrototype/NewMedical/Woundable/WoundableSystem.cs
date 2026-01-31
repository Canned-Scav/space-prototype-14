using Content.Shared.ScavPrototype.NewMedical.Woundable.Systems;
using Content.Shared.ScavPrototype.NewMedical.Woundable.Components;
using Content.Shared.ScavPrototype.NewMedical.Woundable.Events;
using Content.Shared.Input;
using Content.Shared.Body.Components;
using Content.Shared.Body.Systems;
using Robust.Client.Player;
using Robust.Shared.Player;

namespace Content.Client.ScavPrototype.NewMedical.Woundable;

public sealed class WoundableSystem : SharedWoundableSystem
{
    [Dependency] private readonly IPlayerManager _playerManager = default!;
    [Dependency] private readonly SharedBodySystem _body = default!;

    public event Action<WoundableComponent>? PartStatusStartup;
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
        PartStatusStartup?.Invoke(component);
    }

    private void HandlePlayerDetached(EntityUid uid, WoundableComponent component, LocalPlayerDetachedEvent args)
    {
        PartStatusShutdown?.Invoke();
    }

    private void OnPartStatusStartup(EntityUid uid, WoundableComponent component, ComponentStartup args)
    {
        if (_playerManager.LocalEntity != uid)
            return;

        PartStatusStartup?.Invoke(component);
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
}
