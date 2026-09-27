# EinsteinEngines: Common Pitfalls and Anti-Patterns

This document records frequent bugs and misconfigurations when using EinsteinEngines subsystems.

## 1. Missing `LanguageSpeakerComponent`
- **Anti-pattern**: Adding `LanguageKnowledgeComponent` on a mob prototype but forgetting `LanguageSpeakerComponent`.
- **Consequence**: The entity's knowledge is tracked on the server, but the shared and client systems cannot determine the active language or show the language menu UI.
- **Rule**: Always include both `LanguageSpeakerComponent` and `LanguageKnowledgeComponent` on sentient speaking mobs.

## 2. Omitting Battery Slot Lock on Synthetic Chassis
- **Anti-pattern**: Defining an IPC humanoid entity without `BatterySlotRequiresLockComponent`.
- **Consequence**: Any adjacent player can freely yank the power cell out of the IPC mid-combat with zero resistance or delay, immediately disabling them.
- **Rule**: Always add `BatterySlotRequiresLockComponent` to playable or combat silicon chassis.

## 3. Creating C# Verbs for Simple Actions Covered by Interaction Verbs
- **Anti-pattern**: Writing a custom `EntitySystem` and verb subscriptions just to show a popup, play a sound, or modify a status effect on click.
- **Consequence**: Unnecessary C# boilerplate and maintenance burden.
- **Rule**: Check `InteractionVerbPrototype` first; if existing actions (`ChatMessageAction`, `ModifyHealthAction`, etc.) can achieve the behavior, use a YAML interaction verb.

## 4. Broken Obfuscation Replacement Lists
- **Anti-pattern**: Leaving `replacement.syllables` empty in a new `LanguagePrototype`.
- **Consequence**: Chat messages spoken in this language cause null reference exceptions or appear blank to non-speakers.
- **Rule**: Always provide at least 4-8 distinct syllables in the `replacement.syllables` YAML node.
