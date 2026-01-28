using Content.Shared.Administration.Logs;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Item.ItemToggle;
using Content.Shared.Maps;
using Content.Shared.Popups;
using Content.Shared.Tools.Components;
using JetBrains.Annotations;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
//Space Prototype changes
using System.Linq;
using Content.Shared.Damage.Components;
using Content.Shared.Damage.Systems;
using Content.Shared.PowerCell;
using Content.Shared.Power.Components;

namespace Content.Shared.Tools.Systems;

public abstract partial class SharedToolSystem : EntitySystem
{
    [Dependency] private   readonly IGameTiming _timing = default!;
    [Dependency] private   readonly IMapManager _mapManager = default!;
    [Dependency] private   readonly IPrototypeManager _protoMan = default!;
    [Dependency] protected readonly ISharedAdminLogManager AdminLogger = default!;
    [Dependency] private   readonly ITileDefinitionManager _tileDefManager = default!;
    [Dependency] private   readonly SharedAudioSystem _audioSystem = default!;
    [Dependency] private   readonly SharedDoAfterSystem _doAfterSystem = default!;
    [Dependency] protected readonly SharedInteractionSystem InteractionSystem = default!;
    [Dependency] protected readonly ItemToggleSystem ItemToggle = default!;
    [Dependency] private   readonly SharedMapSystem _maps = default!;
    [Dependency] private   readonly SharedPopupSystem _popup = default!;
    [Dependency] protected readonly SharedSolutionContainerSystem SolutionContainerSystem = default!;
    [Dependency] private   readonly SharedTransformSystem _transformSystem = default!;
    [Dependency] private   readonly TileSystem _tiles = default!;
    [Dependency] private   readonly TurfSystem _turfs = default!;

    //Space Prototype changes start
    [Dependency] private readonly DamageableSystem _damageableSystem = default!;
    [Dependency] private readonly PowerCellSystem _powerCell = default!;
    //Space Prototype changes end

    public const string CutQuality = "Cutting";
    public const string PulseQuality = "Pulsing";

    public override void Initialize()
    {
        InitializeMultipleTool();
        InitializeTile();
        InitializeWelder();
        SubscribeLocalEvent<ToolComponent, ToolDoAfterEvent>(OnDoAfter);
        SubscribeLocalEvent<ToolComponent, ExaminedEvent>(OnExamine);
        //Space Prototype changes start
        SubscribeLocalEvent<ToolComponent, ComponentInit>(OnInit);
    }

    private void OnInit(Entity<ToolComponent> entity, ref ComponentInit args)
    {
        entity.Comp.Qualities = entity.Comp.QualitiesLevels
            .ToDictionary(
                pair => pair.Key.Id,
                pair => pair.Value
            );
    }
    //Space Prototype changes end

    private void OnDoAfter(EntityUid uid, ToolComponent tool, ToolDoAfterEvent args)
    {
        if (!args.Cancelled)
        {
            PlayToolSound(uid, tool, args.User);
            //Space Prototype changes start
            if (tool.EnergyTool)
                _powerCell.TryUseCharge(uid, tool.ChargeUse);
            else if (TryComp<DamageableComponent>(uid, out var damageable) && tool.DamagePerUse != null)
                _damageableSystem.ChangeDamage((uid, damageable), tool.DamagePerUse, false, false);
            //Space Prototype changes end
        }

        var ev = args.WrappedEvent;
        ev.DoAfter = args.DoAfter;

        if (args.OriginalTarget != null)
            RaiseLocalEvent(GetEntity(args.OriginalTarget.Value), (object) ev);
        else
            RaiseLocalEvent((object) ev);
    }

    private void OnExamine(Entity<ToolComponent> ent, ref ExaminedEvent args)
    {
        // If the tool has no qualities, exit early
        if (ent.Comp.Qualities.Count == 0)
            return;

        var message = new FormattedMessage();

        // Create a dict to store tool quality names
        //Space Prototype changes start
        var toolQualities = new Dictionary<string, float>();

        // Loop through tool qualities and add localized names to the list
        foreach (var toolQuality in ent.Comp.Qualities)
        {
            if (_protoMan.TryIndex<ToolQualityPrototype>(toolQuality.Key, out var protoToolQuality))
            {
                toolQualities.Add(Loc.GetString(protoToolQuality.Name), toolQuality.Value);
            }
        }

        // Combine the qualities into a single string and localize the final message
        var qualitiesString = string.Join(", ", toolQualities.Select(kvp => $"{kvp.Key} {kvp.Value}"));

        // Add the localized message to the FormattedMessage object
        message.AddMarkupPermissive(Loc.GetString("tool-component-qualities", ("qualities", qualitiesString)));
        args.PushMessage(message);

        if (!TryComp<DamageableComponent>(ent.Owner, out var damageable))
            return;

        ToolDamageExamine(ent.Owner, damageable, ref args);
        //Space Prototype changes end
    }

    public void PlayToolSound(EntityUid uid, ToolComponent tool, EntityUid? user)
    {
        if (tool.UseSound == null)
            return;

        _audioSystem.PlayPredicted(tool.UseSound, uid, user);
    }

    /// <summary>
    ///     Attempts to use a tool on some entity, which will start a DoAfter. Returns true if an interaction occurred.
    ///     Note that this does not mean the interaction was successful, you need to listen for the DoAfter event.
    /// </summary>
    /// <param name="tool">The tool to use</param>
    /// <param name="user">The entity using the tool</param>
    /// <param name="target">The entity that the tool is being used on. This is also the entity that will receive the
    /// event. If null, the event will be broadcast</param>
    /// <param name="doAfterDelay">The base tool use delay (seconds). This will be modified by the tool's quality</param>
    /// <param name="toolQualitiesNeeded">The qualities needed for this tool to work.</param>
    /// <param name="doAfterEv">The event that will be raised when the tool has finished (including cancellation). Event
    /// will be directed at the tool target.</param>
    /// <param name="fuel">Amount of fuel that should be taken from the tool.</param>
    /// <param name="toolComponent">The tool component.</param>
    /// <returns>Returns true if any interaction takes place.</returns>
    public bool UseTool(
        EntityUid tool,
        EntityUid user,
        EntityUid? target,
        float doAfterDelay,
        [ForbidLiteral] Dictionary<string, float> qualitiesNeeded,
        DoAfterEvent doAfterEv,
        float fuel = 0,
        ToolComponent? toolComponent = null)
    {
        return UseTool(tool,
            user,
            target,
            TimeSpan.FromSeconds(doAfterDelay),
            qualitiesNeeded,
            doAfterEv,
            out _,
            fuel,
            toolComponent);
    }

    /// <summary>
    ///     Attempts to use a tool on some entity, which will start a DoAfter. Returns true if an interaction occurred.
    ///     Note that this does not mean the interaction was successful, you need to listen for the DoAfter event.
    /// </summary>
    /// <param name="tool">The tool to use</param>
    /// <param name="user">The entity using the tool</param>
    /// <param name="target">The entity that the tool is being used on. This is also the entity that will receive the
    /// event. If null, the event will be broadcast</param>
    /// <param name="delay">The base tool use delay. This will be modified by the tool's quality</param>
    /// <param name="toolQualitiesNeeded">The qualities needed for this tool to work.</param>
    /// <param name="doAfterEv">The event that will be raised when the tool has finished (including cancellation). Event
    /// will be directed at the tool target.</param>
    /// <param name="id">The id of the DoAfter that was created. This may be null even if the function returns true in
    /// the event that this tool-use cancelled an existing DoAfter</param>
    /// <param name="fuel">Amount of fuel that should be taken from the tool.</param>
    /// <param name="toolComponent">The tool component.</param>
    /// <returns>Returns true if any interaction takes place.</returns>
    public bool UseTool(
        EntityUid tool,
        EntityUid user,
        EntityUid? target,
        TimeSpan delay,
        [ForbidLiteral] Dictionary<string, float> qualitiesNeeded,
        DoAfterEvent doAfterEv,
        out DoAfterId? id,
        float fuel = 0,
        ToolComponent? toolComponent = null)
    {
        id = null;
        if (!Resolve(tool, ref toolComponent, false))
            return false;

        //Space Prototype changes start

        if (!CanStartToolUse(tool, user, target, fuel, qualitiesNeeded, toolComponent))
            return false;
        //Space Prototype changes end

        var toolEvent = new ToolDoAfterEvent(fuel, doAfterEv, GetNetEntity(target));
        var doAfterArgs = new DoAfterArgs(EntityManager, user, delay / toolComponent.SpeedModifier, toolEvent, tool, target: target, used: tool)
        {
            BreakOnDamage = true,
            BreakOnMove = true,
            BreakOnWeightlessMove = false,
            NeedHand = tool != user,
            AttemptFrequency = fuel > 0 ? AttemptFrequency.EveryTick : AttemptFrequency.Never
        };

        _doAfterSystem.TryStartDoAfter(doAfterArgs, out id);
        return true;
    }

    /// <summary>
    ///     Attempts to use a tool on some entity, which will start a DoAfter. Returns true if an interaction occurred.
    ///     Note that this does not mean the interaction was successful, you need to listen for the DoAfter event.
    /// </summary>
    /// <param name="tool">The tool to use</param>
    /// <param name="user">The entity using the tool</param>
    /// <param name="target">The entity that the tool is being used on. This is also the entity that will receive the
    /// event. If null, the event will be broadcast</param>
    /// <param name="doAfterDelay">The base tool use delay (seconds). This will be modified by the tool's quality</param>
    /// <param name="toolQualityNeeded">The quality needed for this tool to work.</param>
    /// <param name="doAfterEv">The event that will be raised when the tool has finished (including cancellation). Event
    /// will be directed at the tool target.</param>
    /// <param name="fuel">Amount of fuel that should be taken from the tool.</param>
    /// <param name="toolComponent">The tool component.</param>
    /// <returns>Returns true if any interaction takes place.</returns>
    public bool UseTool(
        EntityUid tool,
        EntityUid user,
        EntityUid? target,
        float doAfterDelay,
        [ForbidLiteral] string toolQualityNeeded,
        DoAfterEvent doAfterEv,
        float fuel = 0,
        ToolComponent? toolComponent = null,
        float qualitiyLevelNeed = 1f)
    {
        return UseTool(tool,
            user,
            target,
            TimeSpan.FromSeconds(doAfterDelay),
            new Dictionary<string, float> { { toolQualityNeeded, qualitiyLevelNeed } },
            doAfterEv,
            out _,
            fuel,
            toolComponent);
    }

    /// <summary>
    ///     Whether a tool entity has the specified quality or not.
    /// </summary>
    public bool HasQuality(EntityUid uid, [ForbidLiteral] string quality, ToolComponent? tool = null)
    {
        return Resolve(uid, ref tool, false) && tool.Qualities.ContainsKey(quality);
    }

    /// <summary>
    ///     Whether a tool entity has all specified qualities or not.
    /// </summary>
    /*[PublicAPI]
    public bool HasAllQualities(EntityUid uid, [ForbidLiteral] IEnumerable<string> qualities, ToolComponent? tool = null)
    {
        return Resolve(uid, ref tool, false) && tool.Qualities.ContainsAll(qualities);
    }*/

    //Space Prototype changes start
    public bool HasMinQualityLevel(EntityUid uid, [ForbidLiteral] string quality, float qualityLevel, ToolComponent? tool = null)
    {
        return Resolve(uid, ref tool, false) && tool.Qualities.ContainsKey(quality) && tool.Qualities[quality] >= qualityLevel;
    }

    public Dictionary<string, float> DefaultQualitiesLevels(PrototypeFlags<ToolQualityPrototype> qualities)
    {
        var _qualities = new Dictionary<string, float>();

        foreach (var quality in qualities)
        {
            _qualities.Add(quality, 1f);
        }

        return _qualities;
    }

    public bool HasAnyQuality(Dictionary<string, float> qualitiesInitial, PrototypeFlags<ToolQualityPrototype> qualitiesToCheck)
    {
        foreach (var quality in qualitiesToCheck)
        {
            if (qualitiesInitial.ContainsKey(quality))
                return true;
        }
        return false;
    }

    public virtual void ToolDamageExamine(EntityUid uid, DamageableComponent damageable, ref ExaminedEvent args)
    {
        //На серверной части
    }
    //Space Prototype changes end

    private bool CanStartToolUse(EntityUid tool, EntityUid user, EntityUid? target, float fuel, [ForbidLiteral] Dictionary<string, float> qualitiesNeeded, ToolComponent? toolComponent = null)
    {
        if (!Resolve(tool, ref toolComponent))
            return false;

        //Space Prototype changes start
        foreach (var quality in qualitiesNeeded)
        {
            if(!toolComponent.Qualities.ContainsKey(quality.Key) || toolComponent.Qualities[quality.Key] < quality.Value)
                return false;
        }

        if (toolComponent.EnergyTool && _powerCell.HasCharge(tool, toolComponent.ChargeUse, user: user))
            return false;
        //Space Prototype changes end

        // check if the user allows using the tool
        var ev = new ToolUserAttemptUseEvent(target);
        RaiseLocalEvent(user, ref ev);
        if (ev.Cancelled)
            return false;

        // check if the tool allows being used
        var beforeAttempt = new ToolUseAttemptEvent(user, fuel);
        RaiseLocalEvent(tool, beforeAttempt);
        if (beforeAttempt.Cancelled)
            return false;

        // check if the target allows using the tool
        if (target != null && target != tool)
        {
            RaiseLocalEvent(target.Value, beforeAttempt);
        }

        return !beforeAttempt.Cancelled;
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        UpdateWelders();
    }

    #region DoAfterEvents

    [Serializable, NetSerializable]
    protected sealed partial class ToolDoAfterEvent : DoAfterEvent
    {
        [DataField]
        public float Fuel;

        /// <summary>
        ///     Entity that the wrapped do after event will get directed at. If null, event will be broadcast.
        /// </summary>
        [DataField("target")]
        public NetEntity? OriginalTarget;

        [DataField("wrappedEvent")]
        public DoAfterEvent WrappedEvent = default!;

        private ToolDoAfterEvent()
        {
        }

        public ToolDoAfterEvent(float fuel, DoAfterEvent wrappedEvent, NetEntity? originalTarget)
        {
            DebugTools.Assert(wrappedEvent.GetType().HasCustomAttribute<NetSerializableAttribute>(), "Tool event is not serializable");

            Fuel = fuel;
            WrappedEvent = wrappedEvent;
            OriginalTarget = originalTarget;
        }

        public override DoAfterEvent Clone()
        {
            var evClone = WrappedEvent.Clone();

            // Most DoAfter events are immutable
            if (evClone == WrappedEvent)
                return this;

            return new ToolDoAfterEvent(Fuel, evClone, OriginalTarget);
        }

        public override bool IsDuplicate(DoAfterEvent other)
        {
            return other is ToolDoAfterEvent toolDoAfter && WrappedEvent.IsDuplicate(toolDoAfter.WrappedEvent);
        }
    }

    [Serializable, NetSerializable]
    protected sealed partial class LatticeCuttingCompleteEvent : DoAfterEvent
    {
        [DataField(required:true)]
        public NetCoordinates Coordinates;

        private LatticeCuttingCompleteEvent()
        {
        }

        public LatticeCuttingCompleteEvent(NetCoordinates coordinates)
        {
            Coordinates = coordinates;
        }

        public override DoAfterEvent Clone() => this;
    }
}

[Serializable, NetSerializable]
public sealed partial class CableCuttingFinishedEvent : SimpleDoAfterEvent;

#endregion
