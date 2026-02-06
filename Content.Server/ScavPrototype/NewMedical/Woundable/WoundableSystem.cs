using Content.Shared.ScavPrototype.NewMedical.Woundable.Components;
using Content.Shared.ScavPrototype.NewMedical.Woundable.Events;
using Content.Shared.ScavPrototype.NewMedical.Woundable.Systems;
using Content.Shared.ScavPrototype.NewMedical.Targeting;
using Content.Shared.Body.Part;

namespace Content.Server.ScavPrototype.NewMedical.Woundable;
public sealed class WoundableSystem : SharedWoundableSystem
{
    public override void UpdateIntegrity(EntityUid uid, TargetBodyPart bodyPart, float integrityChanged)
    {
        base.UpdateIntegrity(uid, bodyPart, integrityChanged);

        RaiseNetworkEvent(new WoundablePartChangeEvent(GetNetEntity(uid), bodyPart, integrityChanged), uid);
    }
}
