using Content.Client.Gameplay;
using Content.Client.ScavPrototype.NewMedical.UserInterface.PartStatus.Widgets;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.Player;
using Robust.Shared.Utility;
using Robust.Client.Graphics;
using Robust.Shared.Timing;
using Robust.Shared.Timing;
using Content.Client.ScavPrototype.NewMedical.Woundable;
using Content.Shared.ScavPrototype.NewMedical.Woundable.Components;
using Content.Shared.ScavPrototype.NewMedical.Woundable.Events;

namespace Content.Client.ScavPrototype.NewMedical.UserInterface.PartStatus;

public sealed class PartStatusUIController : UIController, IOnStateEntered<GameplayState>, IOnSystemChanged<WoundableSystem>
{
    [Dependency] private readonly IEntityManager _entManager = default!;
    [Dependency] private readonly IEntityNetworkManager _net = default!;
    [Dependency] private readonly IPlayerManager _playerManager = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    private SpriteSystem _spriteSystem = default!;
    private WoundableComponent? _woundableComponent;
    private PartStatusControl? PartStatusControl => UIManager.GetActiveUIWidgetOrNull<PartStatusControl>();

    public void OnSystemLoaded(WoundableSystem system)
    {
        system.PartStatusStartup += AddPartStatusControl;
        system.PartStatusShutdown += RemovePartStatusControl;
        system.PartStatusUpdate += UpdatePartStatusControl;
    }

    public void OnSystemUnloaded(WoundableSystem system)
    {
        system.PartStatusStartup -= AddPartStatusControl;
        system.PartStatusShutdown -= RemovePartStatusControl;
        system.PartStatusUpdate -= UpdatePartStatusControl;
    }

    public void OnStateEntered(GameplayState state)
    {
        WoundableInit();
    }

    public void AddPartStatusControl(WoundableComponent component)
    {
        _woundableComponent = component;

        WoundableInit();
    }

    public void RemovePartStatusControl()
    {
        if (PartStatusControl != null)
            PartStatusControl.SetVisible(false);

        _woundableComponent = null;
    }

    public void UpdatePartStatusControl(WoundablePartChangeEvent args)
    {
        if (PartStatusControl != null && args != null)
            PartStatusControl.SetTexture(args.PartType, args.Integrity);
    }

    public Texture GetTexture(SpriteSpecifier specifier)
    {
        if (_spriteSystem == null)
            _spriteSystem = _entManager.System<SpriteSystem>();

        return _spriteSystem.Frame0(specifier);
    }

    public void WoundableInit()
    {
        if (PartStatusControl != null)
        {
            PartStatusControl.SetVisible(_woundableComponent != null);
            if (_woundableComponent != null)
                foreach (var part in _woundableComponent.PartsWoundable)
                {
                    PartStatusControl.SetTexture(part.Key, 0f);
                }
        }
    }

    /*public void GetPartStatusMessage()
    {
        if (_playerManager.LocalEntity is not { } user
            || _entManager.GetComponent<TargetingComponent>(user) is not { } targetingComponent
            || PartStatusControl == null
            || !_timing.IsFirstTimePredicted)
            return;

        var player = _entManager.GetNetEntity(user);
        _net.SendSystemNetworkMessage(new GetPartStatusEvent(player));
    }*/
}
