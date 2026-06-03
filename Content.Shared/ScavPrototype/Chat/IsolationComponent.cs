using Robust.Shared.GameStates;

namespace Content.Shared.ScavPrototype.Chat;

[RegisterComponent, NetworkedComponent]
[Access(typeof(IsolationSystem))]
public sealed partial class IsolationComponent : Component
{
    [DataField]
    public float Isolation = 1f;
}
