// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 DrSmugleaf <drsmugleaf@gmail.com>
// SPDX-FileCopyrightText: 2023 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 LordEclipse <106132477+LordEclipse@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 PrPleGoo <PrPleGoo@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Slava0135 <40753025+Slava0135@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Slava0135 <super.novalskiy_0135@inbox.ru>
// SPDX-FileCopyrightText: 2023 Visne <39844191+Visne@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Aidenkrz <aiden@djkraz.com>
// SPDX-FileCopyrightText: 2024 ElectroJr <leonsfriedrich@gmail.com>
// SPDX-FileCopyrightText: 2024 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Pieter-Jan Briers <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2024 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.Alert;
using Content.Shared.Nutrition.EntitySystems;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Generic;

namespace Content.Shared.Nutrition.Components;

[RegisterComponent, NetworkedComponent, Access(typeof(ThirstSystem))]
[AutoGenerateComponentState(fieldDeltas: true), AutoGenerateComponentPause]
public sealed partial class ThirstComponent : Component
{
    // Base stuff
    [ViewVariables(VVAccess.ReadWrite)]
    [DataField("baseDecayRate")]
    [AutoNetworkedField]
    public float BaseDecayRate = 0.1f;

    [ViewVariables(VVAccess.ReadWrite)]
    [AutoNetworkedField]
    public float ActualDecayRate;

    [DataField, AutoNetworkedField, ViewVariables(VVAccess.ReadWrite)]
    public ThirstThreshold CurrentThirstThreshold;

    [DataField, AutoNetworkedField, ViewVariables(VVAccess.ReadWrite)]
    public ThirstThreshold LastThirstThreshold;

    [ViewVariables(VVAccess.ReadWrite)]
    [DataField("startingThirst")]
    [AutoNetworkedField]
    public float CurrentThirst = -1f;

    /// <summary>
    /// The time when the hunger will update next.
    /// </summary>
    [DataField("nextUpdateTime", customTypeSerializer: typeof(TimeOffsetSerializer)), ViewVariables(VVAccess.ReadWrite)]
    [AutoNetworkedField]
    [AutoPausedField]
    public TimeSpan NextUpdateTime;

    /// <summary>
    /// The time between each update.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    [DataField, AutoNetworkedField]
    public TimeSpan UpdateRate = TimeSpan.FromSeconds(1);

    [DataField("thresholds", customTypeSerializer: typeof(DictionarySerializer<ThirstThreshold, float>))]
    [AutoNetworkedField]
    public Dictionary<ThirstThreshold, float> ThirstThresholds = new()
    {
        {ThirstThreshold.OverHydrated, 850.0f},
        {ThirstThreshold.HigHydrated, 700.0f},
        {ThirstThreshold.Okay, 550.0f},
        {ThirstThreshold.Thirsty, 300.0f},
        {ThirstThreshold.Parched, 150.0f},
        {ThirstThreshold.Dead, 0.0f},
    };

    [DataField("thirstThresholdDecayModifiers", customTypeSerializer: typeof(DictionarySerializer<ThirstThreshold, float>))]
    [AutoNetworkedField]
    public Dictionary<ThirstThreshold, float> ThirstThresholdDecayModifiers = new()
    {
        { ThirstThreshold.OverHydrated, 1.4f },
        { ThirstThreshold.HigHydrated, 1.2f },
        { ThirstThreshold.Okay, 1f },
        { ThirstThreshold.Thirsty, 0.8f },
        { ThirstThreshold.Parched, 0.6f },
        { ThirstThreshold.Dead, 0.4f }
    };


    [DataField("parchedSlowdownModifier"), ViewVariables(VVAccess.ReadWrite)]
    [AutoNetworkedField]
    public float ParchedSlowdownModifier = 0.85f;

    [DataField("deadSlowdownModifier"), ViewVariables(VVAccess.ReadWrite)]
    [AutoNetworkedField]
    public float DeadSlowdownModifier = 0.65f;


    [DataField]
    public ProtoId<AlertCategoryPrototype> ThirstyCategory = "Thirst";

    public static readonly Dictionary<ThirstThreshold, ProtoId<AlertPrototype>> ThirstThresholdAlertTypes = new()
    {
        {ThirstThreshold.OverHydrated, "OverHydrated"},
        {ThirstThreshold.HigHydrated, "HigHydrated"},
        {ThirstThreshold.Thirsty, "Thirsty"},
        {ThirstThreshold.Parched, "Parched"},
        {ThirstThreshold.Dead, "ThirstDead"},
    };
}

[Serializable, NetSerializable]
public enum ThirstThreshold : byte
{
    // Hydrohomies
    Dead = 0,
    Parched = 1 << 0,
    Thirsty = 1 << 1,
    Okay = 1 << 2,
    HigHydrated = 1 << 3,
    OverHydrated = 1 << 4
}
