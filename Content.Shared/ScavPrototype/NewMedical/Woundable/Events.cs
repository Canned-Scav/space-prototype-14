using Robust.Shared.Serialization;
using Content.Shared.Body.Part;

namespace Content.Shared.ScavPrototype.NewMedical.Woundable.Events;

[Serializable, NetSerializable]
public sealed class WoundablePartChangeEvent : EntityEventArgs
{
    public NetEntity Uid { get; }
    public bool RefreshUi { get; }
    public BodyPartType Type { get; }
    public BodyPartSymmetry Symmetry { get; }
    public float Integrity { get; }
    public WoundablePartChangeEvent(NetEntity uid, BodyPartType type, BodyPartSymmetry symmetry = BodyPartSymmetry.None, float integrity = 0, bool refreshUi = true)
    {
        Uid = uid;
        Type = type;
        Symmetry = symmetry;
        Integrity = integrity;
        RefreshUi = refreshUi;
    }
}
