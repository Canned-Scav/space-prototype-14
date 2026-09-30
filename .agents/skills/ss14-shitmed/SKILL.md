---
name: ss14-shitmed
description: "Shitmed (`_Shitmed`): WoundSystem (localized damage routing), TraumaSystem (bone/organ/nerve/vein/dismemberment/braindeath), PainSystem (NerveComponent, screaming, pain shock), ConsciousnessSystem (replaces vanilla consciousness), SharedBloodstreamSystem override, SharedSurgerySystem (steps/conditions/effects/tools), Autodoc, PartStatus, BodyScanner, organ simulation (Heart, Eyes, Brain), tourniquets, FumbleOnDamage, GhettoSurgery, DelayedDeath, cybernetics, and Abductor antag."
---

# Shitmed Architecture: Surgery, Body and Damage

This skill covers the Shitmed medical and anatomical system located in `_Shitmed` across `Content.Shared`, `Content.Server`, `Content.Client`, and `Resources/Prototypes`.

In Goob Station, Shitmed completely replaces vanilla SS14 monolithic entity health with realistic localized limb damage, wound tracking, trauma simulation, pain mechanics, consciousness modeling, bleeding, and surgical procedures.

## 1. Scope and Boundaries

1. This skill covers:
   - **WoundSystem**: The central damage routing system (431+ lines). Receives damage, creates wound entities on body parts, triggers trauma, handles wound healing ticks, and manages wound appearance.
   - **TraumaSystem**: Manages six trauma types — `BoneDamage`, `OrganDamage`, `NerveDamage`, `Dismemberment`, `VeinsDamage`, `Braindeath` — with threshold-based activation, stun/knockdown effects, and movement impairment.
   - **PainSystem**: Tracks pain via `NerveComponent` on body parts. Causes jitter, screaming, stun, knockdown, and consciousness loss based on cumulative pain levels. Networked with `ComponentGetState`/`ComponentHandleState`.
   - **ConsciousnessSystem**: Replaces much of vanilla `MobStateSystem` for determining whether entities are conscious. Multi-partial (`.cs`, `.Helpers.cs`, `.Process.cs`, `.Networking.cs`).
   - **SharedBloodstreamSystem**: Major override (14KB) of vanilla bloodstream behavior for wound-based bleeding.
   - **SharedSurgerySystem**: Orchestrates surgical procedures via multi-partial class (`.cs`, `.Start.cs`, `.Steps.cs` at 45KB). Steps, conditions, effects, tools, and do-after events.
   - **Targeting system**: `SharedTargetingSystem` with `TargetingComponent` for body zone selection (Head, Torso, Groin, LeftArm, RightArm, LeftHand, RightHand, LeftLeg, RightLeg, LeftFoot, RightFoot).
   - **AutodocSystem**: Automated surgery pod (`SharedAutodocSystem` abstract, plus `HandsFillSystem`).
   - **Body parts and organs**: `BodyPartEffectSystem`, `OrganEffectSystem`, server-side `HeartSystem`, `EyesSystem`, `DebrainedSystem`, `GenerateChildPartSystem`.
   - **Tourniquets**: `TourniquetSystem` (server) for arterial bleed control with necrosis timer.
   - **Misc systems**: `FumbleOnDamageSystem` (drop weapons on injury), `SharedItemSwitchSystem`, `SharedOnHitSystem`, `SharedRestrictSystem`, `SurgeryToolConditionsSystem`, `SurgeryToolExamineSystem`, `PartStatusSystem` (server), `GhettoSurgerySystem`, `DelayedDeathSystem`, `BodyScannerSystem`, `TraumaBloodlossSystem`, `TraumaCauseSystem`, `ScrambleDnaEffectSystem`.
   - **Antags**: `SharedAbductorSystem` (Abductor antagonist).
2. For base chemistry and reagent metabolism, consult `Content.Shared.Chemistry`.
3. For base SS14 damage containers, consult `ss14-ecs-prototypes`.
4. Values use `FixedPoint` from `Content.Goobstation.Maths.FixedPoint`, not `float`.

## 2. Resource Reading Order

Read resources in this sequence:
1. `references/components-and-systems.md`: Register of all Shitmed systems, wound/trauma/pain mechanics, surgical pipeline, and cybernetics.
2. `references/integration-recipes.md`: Recipes for authoring armor coverage, weapons with trauma chances, and tourniquets.
3. `references/common-pitfalls.md`: Critical design traps when balancing combat, healing, or pain against Shitmed.

## 3. Mental Model and Directory Organization

| Side | Path | Key Contents |
| --- | --- | --- |
| Shared | `Content.Shared/_Shitmed` | **Surgery** (SharedSurgerySystem multi-partial, Steps, Conditions, Effects, Tools, Wounds/WoundSystem, Traumas/TraumaSystem, Pain/PainSystem, Consciousness/ConsciousnessSystem, SharedBloodstreamSystem), **Targeting** (SharedTargetingSystem), **Autodoc** (SharedAutodocSystem, HandsFillSystem), **Body** (Components, Events, Organ, Part, Systems), **BodyEffects** (BodyPartEffectSystem, OrganEffectSystem), **OnHit** (SharedOnHitSystem), **ItemSwitch**, **Switchable**, **Restrict**, **Weapons** (FumbleOnDamageSystem), **Damage**, **Antags** (SharedAbductorSystem), **Cybernetics**, **Abilities**, **Analyzer**, **CCVar**, **DoAfter**, **EntityEffects**, **Humanoid**, **Medical**, **PartStatus**, **Roles**, **Spawners**, **StatusEffects**, **Tourniquet** |
| Server | `Content.Server/_Shitmed` | **PartStatus** (PartStatusSystem), **Medical** (TourniquetSystem, GhettoSurgerySystem, TraumaBloodlossSystem, TraumaCauseSystem, BraindeathSystem), **Body** (DebrainedSystem, HeartSystem, EyesSystem, StatusEffectOrganSystem, GenerateChildPartSystem, RandomStatusActivationSystem), **Analyzer** (BodyScannerSystem), **Targeting**, **DelayedDeath**, **Destructible**, **OnHit**, **ItemSwitch**, **StatusEffects** (ScrambleDnaEffectSystem, ExpelGasSystem, ScrambleLocationEffectSystem, SpawnEntityEffectSystem, ActivateArtifactEffectSystem), **Objectives**, **GameTicking**, **Autodoc**, **Antags**, **Cybernetics** |
| Client | `Content.Client/_Shitmed` | Targeting HUD UI, surgery radial menu, body part damage indicators, wound visualization |
| Prototypes | `Resources/Prototypes/_Shitmed` | Surgical operations, tools, cybernetic implants, body parts, wound types, trauma types, pain thresholds |

## 4. Key Architectural Notes

### 4.1 Core Data Flow

```
Damage Event → WoundSystem (creates/updates Wound on body part)
                 → TraumaSystem (bone/organ/nerve/vein damage if threshold exceeded)
                 → PainSystem (pain accumulation on NerveComponent)
                 → ConsciousnessSystem (consciousness check, possible unconsciousness)
                 → SharedBloodstreamSystem (arterial bleed if VeinsDamage)
```

### 4.2 TraumaSystem Trauma Types

These are predefined `ProtoId<TraumaTypePrototype>` constants in `TraumaSystem`:

| Constant | Effect |
| --- | --- |
| `BoneDamage` | Fractures, movement speed penalty, pain spikes |
| `OrganDamage` | Internal organ degradation, failure states |
| `NerveDamage` | Nerve impairment, reduced manipulation, fumbling |
| `Dismemberment` | Limb separation, entity drop, critical blood loss |
| `VeinsDamage` | Arterial bleeding, rapid blood loss requiring tourniquet |
| `Braindeath` | Irreversible brain death |

### 4.3 PainSystem and NerveComponent

`PainSystem` is networked — `NerveComponent` uses `ComponentGetState`/`ComponentHandleState` for client prediction. Pain accumulates per body part and triggers:
- Jitter at low thresholds
- Screaming at medium thresholds (configurable via CVar, ~20% chance per tick)
- Stun/knockdown at high thresholds
- Consciousness loss via `ConsciousnessSystem` at extreme levels

### 4.4 Surgery Pipeline

`SharedSurgerySystem` is a 67KB+ multi-partial system:
- `.cs` (21KB): Core logic, component management, tool handling
- `.Steps.cs` (45KB): Individual surgical step resolution, validation, completion
- `.Start.cs` (2.4KB): Surgery initiation and target validation

Each surgery is a chain of typed steps (Incision → Retraction → Clamping → procedure-specific → Cauterization/Suturing) validated by `SurgeryToolConditionsSystem`.

### 4.5 FixedPoint Math

All wound severity, pain thresholds, trauma values, and health pool numbers use `FixedPoint2` from `Content.Goobstation.Maths.FixedPoint`, not raw `float` or `int`. Always use the correct type when doing arithmetic on health values.

## 5. Fundamental Rules for `_ScavPrototype`

1. **Armor Coverage**: Armor in `_ScavPrototype` must explicitly define which Shitmed body parts it shields via `ProtectedBodyParts`. A vest protecting only `Torso` leaves `Groin` and `Arms` vulnerable.
2. **Weapons**: Calibrate damage values against individual limb health pools, not total entity HP. Rapid-fire weapons: 8-15 damage; heavy rifles: 25-35; sniper/heavy: 40-55. Use `FixedPoint2` for all values.
3. **Trauma Interaction**: Blunt weapons should interact with `BoneDamage` trauma; piercing with `VeinsDamage`; slash with both. Consider adding `SharedOnHitSystem` hooks for trauma triggers.
4. **Medical Gear**: Healing items and field dressings must address localized wounds and pain, not generic HP restoration. Use `WoundSystem` APIs.
5. **Isolation**: Never edit `_Shitmed` files directly. Keep custom medical items and armor in `_ScavPrototype`.
6. **Pain Balance**: Items that suppress pain (painkillers, nerve blocks) should interact with `PainSystem` / `NerveComponent`, not simply heal damage.
