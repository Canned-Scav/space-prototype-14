using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Content.Shared.Body.Part;
using Content.Shared.ScavPrototype.NewMedical.Targeting;

namespace Content.Shared.ScavPrototype.NewMedical.Woundable.Components;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class WoundableComponent : Component
{
    [DataField, AutoNetworkedField]
    public Dictionary<TargetBodyPart, EntityUid> PartsWoundable = new Dictionary<TargetBodyPart, EntityUid>();
}
