using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Utility;
//Space Prototype changes
using Robust.Shared.Prototypes;

namespace Content.Shared.Tools.Components;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState(true)]
public sealed partial class MultipleToolComponent : Component
{
    [DataDefinition]
    public sealed partial class ToolEntry
    {
        //Space Prototype changes start
        [ViewVariables]
        public Dictionary<string, float> Behavior = new();

        [DataField("behavior", required: true)]
        public Dictionary<ProtoId<ToolQualityPrototype>, float> BehaviorLevels = new();

        //Space Prototype end

        [DataField]
        public SoundSpecifier? UseSound;

        [DataField]
        public SoundSpecifier? ChangeSound;

        [DataField]
        public SpriteSpecifier? Sprite;
    }

    [DataField(required: true)]
    public ToolEntry[] Entries { get; private set; } = Array.Empty<ToolEntry>();

    [ViewVariables]
    [AutoNetworkedField]
    public uint CurrentEntry = 0;

    [ViewVariables]
    public string CurrentQualityName = string.Empty;

    [ViewVariables(VVAccess.ReadWrite)]
    public bool UiUpdateNeeded;

    [DataField]
    public bool StatusShowBehavior = true;
}
