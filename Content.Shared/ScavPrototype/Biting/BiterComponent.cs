using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Prototypes;
using Content.Shared.Damage;
using Robust.Shared.Audio;

namespace Content.Shared.ScavPrototype.Biting;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
[Access(typeof(BiterSystem))]
public sealed partial class BiterComponent : Component
{
    [DataDefinition]
    public sealed partial class BiteEntry
    {
        [DataField(required: true)]
        public float BiteTime;

        [DataField]
        public float TransferAmount;

        [DataField]
        public float HungerAmount;

        [DataField]
        public DamageSpecifier Damage = default!;

        [DataField]
        public SoundSpecifier? BiteSound;
    }

    [DataField]
    public EntProtoId BiteAction = "DuneBiteAction";

    [ViewVariables, AutoNetworkedField]
    public EntityUid? BiteActionEntity;

    [DataField(required: true)]
    public Dictionary<BiteType, BiteEntry> BiteTypes = new();
}

[Serializable, NetSerializable]
public enum BiteType : byte
{
    Normal = 0,
    Strong = 1 << 0,
    Dead = 1 << 1,
}
