// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.Alert;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
using Content.Shared.Nutrition.Components;
using Content.Shared.Rejuvenate;
using Content.Shared.StatusIcon;
using JetBrains.Annotations;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using System.Diagnostics.CodeAnalysis;

namespace Content.Shared.Nutrition.EntitySystems;

[UsedImplicitly]
public sealed partial class ThirstSystem : EntitySystem // Goob
{
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly IPrototypeManager _prototype = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly AlertsSystem _alerts = default!;
    [Dependency] private readonly MovementSpeedModifierSystem _movement = default!;
    [Dependency] private readonly SharedJetpackSystem _jetpack = default!;

    private static readonly ProtoId<SatiationIconPrototype> ThirstIconOverhydratedId = "ThirstIconOverhydrated";
    private static readonly ProtoId<SatiationIconPrototype> ThirstIconThirstyId = "ThirstIconThirsty";
    private static readonly ProtoId<SatiationIconPrototype> ThirstIconParchedId = "ThirstIconParched";

    private const float MaxThreshold = 150.0f;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ThirstComponent, RefreshMovementSpeedModifiersEvent>(OnRefreshMovespeed);
        SubscribeLocalEvent<ThirstComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<ThirstComponent, RejuvenateEvent>(OnRejuvenate);
        SubscribeLocalEvent<ThirstComponent, ComponentShutdown>(OnShutdown); // Goob
    }

    private void OnMapInit(EntityUid uid, ThirstComponent component, MapInitEvent args)
    {
        // Do not change behavior unless starting value is explicitly defined
        if (component.CurrentThirst < 0)
        {
            component.CurrentThirst = _random.Next(
                (int) component.ThirstThresholds[ThirstThreshold.Thirsty] + 10,
                (int) component.ThirstThresholds[ThirstThreshold.Okay] - 1);

            DirtyField(uid, component, nameof(ThirstComponent.CurrentThirst));
        }
        component.NextUpdateTime = _timing.CurTime + component.NextUpdateTime;
        component.CurrentThirstThreshold = GetThirstThreshold(component, component.CurrentThirst);
        component.LastThirstThreshold = ThirstThreshold.Okay; // TODO: Potentially change this -> Used Okay because no effects.
        // TODO: Check all thresholds make sense and throw if they don't.
        UpdateEffects(uid, component);

        DirtyFields(uid, component, null, nameof(ThirstComponent.NextUpdateTime), nameof(ThirstComponent.CurrentThirstThreshold), nameof(ThirstComponent.LastThirstThreshold));

        TryComp(uid, out MovementSpeedModifierComponent? moveMod);
            _movement.RefreshMovementSpeedModifiers(uid, moveMod);
    }

    private void OnRefreshMovespeed(EntityUid uid, ThirstComponent component, RefreshMovementSpeedModifiersEvent args)
    {
        // TODO: This should really be taken care of somewhere else
        if (_jetpack.IsUserFlying(uid) || component.LifeStage > ComponentLifeStage.Running) // Goob
            return;

        if (component.CurrentThirstThreshold >= ThirstThreshold.OverHydrated)
            args.ModifySpeed(component.ParchedSlowdownModifier, component.ParchedSlowdownModifier);
        else if (component.CurrentThirstThreshold <= ThirstThreshold.Parched)
            args.ModifySpeed(component.ParchedSlowdownModifier, component.ParchedSlowdownModifier);
        else if (component.CurrentThirstThreshold <= ThirstThreshold.Dead)
            args.ModifySpeed(component.DeadSlowdownModifier, component.DeadSlowdownModifier);
    }

    private void OnRejuvenate(EntityUid uid, ThirstComponent component, RejuvenateEvent args)
    {
        SetThirst(uid, component, component.ThirstThresholds[ThirstThreshold.Okay]);
    }

    private ThirstThreshold GetThirstThreshold(ThirstComponent component, float amount)
    {
        ThirstThreshold result = ThirstThreshold.Dead;
        var value = component.ThirstThresholds[ThirstThreshold.OverHydrated];
        foreach (var threshold in component.ThirstThresholds)
        {
            if (threshold.Value <= value && threshold.Value >= amount)
            {
                result = threshold.Key;
                value = threshold.Value;
            }
        }

        return result;
    }

    public void ModifyThirst(EntityUid uid, ThirstComponent component, float amount)
    {
        SetThirst(uid, component, component.CurrentThirst + amount);
    }

    public void SetThirst(EntityUid uid, ThirstComponent component, float amount)
    {
        component.CurrentThirst = Math.Clamp(amount,
            component.ThirstThresholds[ThirstThreshold.Dead]-MaxThreshold,
            component.ThirstThresholds[ThirstThreshold.OverHydrated]
        );

        DirtyField(uid, component, nameof(ThirstComponent.CurrentThirst));
    }

    private bool IsMovementThreshold(ThirstThreshold threshold)
    {
        switch (threshold)
        {
            case ThirstThreshold.Dead:
            case ThirstThreshold.Parched:
                return true;
            case ThirstThreshold.Thirsty:
            case ThirstThreshold.Okay:
            case ThirstThreshold.OverHydrated:
                return false;
            default:
                throw new ArgumentOutOfRangeException(nameof(threshold), threshold, null);
        }
    }

    public bool TryGetStatusIconPrototype(ThirstComponent component, [NotNullWhen(true)] out SatiationIconPrototype? prototype)
    {
        switch (component.CurrentThirstThreshold)
        {
            case ThirstThreshold.OverHydrated:
                _prototype.Resolve(ThirstIconOverhydratedId, out prototype);
                break;

            case ThirstThreshold.Thirsty:
                _prototype.Resolve(ThirstIconThirstyId, out prototype);
                break;

            case ThirstThreshold.Parched:
                _prototype.Resolve(ThirstIconParchedId, out prototype);
                break;

            default:
                prototype = null;
                break;
        }

        return prototype != null;
    }

    private void UpdateEffects(EntityUid uid, ThirstComponent component)
    {
        if (component.LastThirstThreshold != component.CurrentThirstThreshold &&
                TryComp(uid, out MovementSpeedModifierComponent? movementSlowdownComponent))
        {
            _movement.RefreshMovementSpeedModifiers(uid, movementSlowdownComponent);
        }

        // Update UI
        if (ThirstComponent.ThirstThresholdAlertTypes.TryGetValue(component.CurrentThirstThreshold, out var alertId))
        {
            _alerts.ShowAlert(uid, alertId);
        }
        else
        {
            _alerts.ClearAlertCategory(uid, component.ThirstyCategory);
        }


        if (component.ThirstThresholdDecayModifiers.TryGetValue(component.CurrentThirstThreshold, out var modifier))
        {
            component.ActualDecayRate = component.BaseDecayRate * modifier;
            DirtyField(uid, component, nameof(ThirstComponent.ActualDecayRate));
        }

        component.LastThirstThreshold = component.CurrentThirstThreshold;
        DirtyField(uid, component, nameof(ThirstComponent.LastThirstThreshold));
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<ThirstComponent>();
        while (query.MoveNext(out var uid, out var thirst))
        {
            if (_timing.CurTime < thirst.NextUpdateTime)
                continue;

            thirst.NextUpdateTime += thirst.UpdateRate;

            ModifyThirst(uid, thirst, -thirst.ActualDecayRate);
            var calculatedThirstThreshold = GetThirstThreshold(thirst, thirst.CurrentThirst);

            if (calculatedThirstThreshold == thirst.CurrentThirstThreshold)
                continue;

            thirst.CurrentThirstThreshold = calculatedThirstThreshold;
            UpdateEffects(uid, thirst);
        }
    }
}