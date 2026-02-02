using Content.Shared.ScavPrototype.NewMedical.Woundable.Components;
using Content.Shared.ScavPrototype.NewMedical.Woundable.Events;
using Content.Shared.ScavPrototype.NewMedical.Woundable.Systems;
using Content.Shared.Body.Part;

namespace Content.Server.ScavPrototype.NewMedical.Woundable;
public sealed class WoundableSystem : SharedWoundableSystem
{
    public override void UpdateIntegrity(EntityUid uid, BodyPartComponent bodyPart, float integrityChanged)
    {
        base.UpdateIntegrity(uid, bodyPart, integrityChanged);

        RaiseNetworkEvent(new WoundablePartChangeEvent(GetNetEntity(uid), bodyPart.PartType, bodyPart.Symmetry, integrityChanged), uid);
    }
}
