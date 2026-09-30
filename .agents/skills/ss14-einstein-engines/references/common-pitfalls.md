# EinsteinEngines: Common Pitfalls and Anti-Patterns

This document records frequent bugs and misconfigurations when using EinsteinEngines subsystems.

## 1. Missing `LanguageSpeakerComponent`

- **Anti-pattern**: Adding `LanguageKnowledgeComponent` on a mob prototype but forgetting `LanguageSpeakerComponent`.
- **Consequence**: The entity's knowledge is tracked on the server, but the shared and client systems cannot determine the active language or show the language menu UI.
- **Rule**: Always include both `LanguageSpeakerComponent` and `LanguageKnowledgeComponent` on sentient speaking mobs. `LanguageSpeaker` is the shared/client component; `LanguageKnowledge` is the server-side authoritative state.

## 2. Omitting Battery Slot Lock on Synthetic Chassis

- **Anti-pattern**: Defining an IPC humanoid entity without `BatterySlotRequiresLockComponent`.
- **Consequence**: Any adjacent player can freely yank the power cell out of the IPC mid-combat with zero resistance or delay, immediately disabling them.
- **Rule**: Always add `BatterySlotRequiresLockComponent` to playable or combat silicon chassis.

## 3. Creating C# Verbs for Simple Actions Covered by Interaction Verbs

- **Anti-pattern**: Writing a custom `EntitySystem` and verb subscriptions just to show a popup, play a sound, or modify a status effect on click.
- **Consequence**: Unnecessary C# boilerplate and maintenance burden.
- **Rule**: Check all 11 `InteractionVerbPrototype` action types first. Beyond the basic ones (`ChatMessageAction`, `ModifyHealthAction`), also check:
  - `ComplexAction` for composing multiple actions
  - `ConditionalAction` for branching logic
  - `OnUserAction` for affecting the performer instead of the target
  - `JitterAction` for jitter effects
  - `NoOpAction` as a placeholder in conditionals

## 4. Broken Obfuscation Replacement Lists

- **Anti-pattern**: Leaving `replacement.syllables` empty in a new `LanguagePrototype`.
- **Consequence**: Chat messages spoken in this language cause null reference exceptions or appear blank to non-speakers.
- **Rule**: Always provide at least 4-8 distinct syllables in the `replacement.syllables` YAML node.

## 5. Resolving Abstract Silicon Systems as Concrete

- **Anti-pattern**: Trying to `[Dependency]` inject `SharedSiliconDeathSystem` or `SharedBatteryDrinkerSystem` directly in shared code.
- **Consequence**: These are abstract classes and cannot be resolved directly. `SharedSiliconChargeSystem` is the only sealed shared silicon system.
- **Rule**: In shared code, either:
  1. Use `SharedSiliconChargeSystem` (sealed, resolvable in shared).
  2. Read component data directly (`SiliconComponent`, `BatteryDrinkerComponent`) without calling system methods.
  3. For server-only behavior, resolve the concrete server system in `Content.Server/_ScavPrototype`.

## 6. Ignoring ContestsSystem for Strength Checks

- **Anti-pattern**: Implementing ad-hoc mass/strength comparisons with custom formulas.
- **Consequence**: Inconsistent behavior with DeltaV's `CarryingSystem` and other systems that already use `ContestsSystem`.
- **Rule**: Use `ContestsSystem` from `Content.Shared._EinsteinEngines.Contests` for any entity-vs-entity mass, strength, or grappling comparisons.

## 7. Missing Verb Requirements

- **Anti-pattern**: Creating `InteractionVerbPrototype` without distance or hand requirements.
- **Consequence**: Players can activate verbs from across the map or without free hands, breaking immersion and balance.
- **Rule**: Always include at least `DistanceRequirement` and, for physical actions, `HandsRequirement`. Check `AssortedRequirements.cs` for available requirement types.

## 8. Confusing `InteractionVerbsComponent` and `OwnInteractionVerbsComponent`

- **Anti-pattern**: Adding `InteractionVerbsComponent` to an entity that should define its own verbs.
- **Consequence**: `InteractionVerbsComponent` means the entity is a **target** for verbs from other entities. `OwnInteractionVerbsComponent` means the entity **owns** verbs to perform on others.
- **Rule**: Use `InteractionVerbsComponent` on entities that receive verbs (patients, objects). Use `OwnInteractionVerbsComponent` on entities that perform verbs (tools, medics).
