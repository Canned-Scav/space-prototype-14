using Robust.Shared.GameStates;
using Content.Shared.Trigger.Components.Triggers;

namespace Content.Shared.ScavPrototype.Trigger;

/// <summary>
/// Triggers when the lock toggle.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class TriggerOnLockComponent : BaseTriggerOnXComponent;
