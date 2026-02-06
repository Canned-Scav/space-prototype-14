using Robust.Shared.Serialization;
using Content.Shared.ScavPrototype.NewMedical.Targeting;

namespace Content.Shared.ScavPrototype.NewMedical.Woundable.Events;

[Serializable, NetSerializable]
public sealed class WoundablePartChangeEvent : EntityEventArgs
{
    public NetEntity Uid { get; }
    public bool RefreshUi { get; }
    public TargetBodyPart PartType { get; }
    public float Integrity { get; }
    public WoundablePartChangeEvent(NetEntity uid, TargetBodyPart type, float integrity = 0, bool refreshUi = true)
    {
        Uid = uid;
        PartType = type;
        Integrity = integrity;
        RefreshUi = refreshUi;
    }
}
