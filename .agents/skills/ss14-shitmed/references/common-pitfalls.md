# Shitmed: Common Pitfalls and Anti-Patterns

This document records frequent design bugs, balancing errors, and architectural traps when interfacing with Shitmed.

## 1. Omitting Body Part Coverage on Armor

- **Anti-pattern**: Creating a helmet, vest, or greaves without specifying `ProtectedBodyParts`.
- **Consequence**: The armor provides passive damage resistance on paper, but because incoming hits are routed through `WoundSystem` to specific body parts via `SharedTargetingSystem`, attacks to unprotected limbs bypass the armor completely.
- **Rule**: Every piece of combat clothing must explicitly list its protected body parts. A "full armor" set requires 4 clothing items covering all 11 target zones.

## 2. Inappropriate Weapon Damage Scaling

- **Anti-pattern**: Giving a rapid-fire weapon 25-30 damage per bullet without realizing each limb has roughly 40-60 health.
- **Consequence**: Targets are instantly dismembered or decapitated within two bullets.
- **Rule**: Balance weapon damage with individual limb health pools in mind:
  - Rapid-fire weapons: 8-15 damage
  - Standard melee: 15-25 damage
  - Heavy rifles/melee: 25-35 damage
  - Sniper/heavy weapons: 40-55 damage

## 3. Ignoring Tourniquet Necrosis

- **Anti-pattern**: Applying a tourniquet and leaving it on indefinitely.
- **Consequence**: After `necrosisTime` seconds (typically 180s), prolonged blood cutoff causes limb necrosis, forcing medical amputation.
- **Rule**: Tourniquets are emergency stabilization tools; wounds must be clamped or treated surgically, after which the tourniquet is removed.

## 4. Using Float/Int Instead of FixedPoint2

- **Anti-pattern**: Using `float` or `int` for wound severity, pain thresholds, trauma values, or health pool arithmetic.
- **Consequence**: Type mismatch compilation errors. All Shitmed health math uses `FixedPoint2` from `Content.Goobstation.Maths.FixedPoint`.
- **Rule**: Import `Content.Goobstation.Maths.FixedPoint` and use `FixedPoint2.New(value)` for all wound/damage/pain numeric values.

## 5. Healing Damage Without Addressing Wounds

- **Anti-pattern**: Using vanilla `DamageableSystem` to heal an entity without going through `WoundSystem`.
- **Consequence**: The entity's `DamageableComponent` shows healed values, but wound entities on body parts still exist, continuing to cause pain, bleeding, and trauma. The entity may appear healthy but still suffer from ongoing wound effects.
- **Rule**: Healing in Shitmed requires addressing wounds via `WoundSystem` APIs, not just modifying the damage container. Medical items must interact with wound healing ticks.

## 6. Ignoring Pain System When Creating Painkillers

- **Anti-pattern**: Creating a "painkiller" item that simply heals Brute/Burn damage.
- **Consequence**: Pain continues accumulating via `PainSystem` and `NerveComponent` even after damage is healed, because pain is tracked separately from wounds.
- **Rule**: Painkillers must interact with `PainSystem` / `NerveComponent` to reduce pain values. Healing damage alone does not relieve pain.

## 7. Skipping Consciousness System Checks

- **Anti-pattern**: Using vanilla `MobStateSystem.IsAlive()` as the sole check for whether an entity can act.
- **Consequence**: Shitmed entities can be "alive" per mob state but unconscious per `ConsciousnessSystem` due to accumulated pain, blood loss, or organ failure.
- **Rule**: Check both `MobStateSystem` and `ConsciousnessSystem` when determining entity consciousness in systems that interact with Shitmed entities.

## 8. Not Understanding the Surgery Step Pipeline

- **Anti-pattern**: Adding a new surgical procedure by creating a standalone `EntitySystem` with custom do-after logic.
- **Consequence**: Bypasses the existing surgery validation pipeline, tool quality checks (`SurgeryToolConditionsSystem`), and step progression logic in `SharedSurgerySystem.Steps.cs`.
- **Rule**: New surgical procedures should be defined as prototype-driven step chains that plug into `SharedSurgerySystem`, not as independent systems.

## 9. Partial Class File Discovery

- **Anti-pattern**: Modifying `SharedSurgerySystem.cs` without checking `.Steps.cs` (45KB) and `.Start.cs`.
- **Consequence**: Missing 68% of the surgery logic. `ConsciousnessSystem` similarly has 4 partial files (`.cs`, `.Helpers.cs` at 19KB, `.Process.cs`, `.Networking.cs`).
- **Rule**: Before modifying any Shitmed system, list all files in its directory. Key multi-partial systems:
  - `SharedSurgerySystem`: 3 files, 67KB+ total
  - `ConsciousnessSystem`: 4 files, 31KB+ total
  - `WoundSystem`: Large single file but references many partial subsystems
  - `TraumaSystem`: Multi-partial via `InitProcess`, `InitBones`, `InitOrgans`
