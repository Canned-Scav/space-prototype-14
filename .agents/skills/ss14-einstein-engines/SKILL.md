---
name: ss14-einstein-engines
description: "EinsteinEngines (`_EinsteinEngines`): Language subsystem and translators, Silicon/IPC battery/charging/death, Interaction Verbs (YAML-defined with 11 action types and complex requirements), Flight, Contests/Height adjustment, ScentTracker forensics, SelfExtinguisher, Psionics/Telepathy, RestrictedMelee, TelescopicBaton, RomanNaming, and BatteryDrinker power."
---

# EinsteinEngines Architecture and Systems

This skill covers the systems and components inherited from EinsteinEngines, located in `_EinsteinEngines` folders across `Content.Shared`, `Content.Server`, `Content.Client`, and `Resources/Prototypes`.

## 1. Scope and Boundaries

1. This skill covers:
   - **Language subsystem**: Speech obfuscation, translation devices and implants, chat commands (`/say :<lang>`, `/listlanguages`, `/selectlanguage`).
   - **Silicon and IPC**: Battery drain (`SharedBatteryDrinkerSystem`), charge monitoring (`SharedSiliconChargeSystem`), death on power loss (`SharedSiliconDeathSystem`), reboot button (`SharedDeadStartupButtonSystem`), blind healing (`SharedBlindHealingSystem`).
   - **Interaction Verbs**: YAML-defined contextual verb actions with 11 built-in action types and complex requirements without C# boilerplate.
   - **Flight**: Activation, gravity interactions, fixture mask toggling, and client elevation visuals.
   - **Contests and Height**: Standardized entity-vs-entity contest checks and character height slider adjustments.
   - **Forensics**: Scent tracking via `SharedScentTrackerSystem`.
   - **Self-Extinguisher**: `SharedSelfExtinguisherSystem` for automatic fire suppression.
   - **Psionics**: `TelepathyComponent` for psionic communication.
   - **Combat**: `RestrictedMeleeSystem` (weapon usage restrictions), `TelescopicBaton` namespace.
   - **Misc**: `RomanNamingSystem`, `RevolutionaryConverterSystem`, `BloodstreamAffectedByMassComponent` / `BloodstreamAdjustSystem`.
2. For medical systems and cybernetics surgery, consult `ss14-shitmed`.
3. For base SS14 ECS patterns, consult `ss14-ecs-components` and `ss14-ecs-systems`.

## 2. Resource Reading Order

Read resources in this sequence:
1. `references/components-and-systems.md`: Complete register of components, systems, prototypes, and chat commands.
2. `references/integration-recipes.md`: Practical recipes for custom languages, IPC mobs, and YAML verbs.
3. `references/common-pitfalls.md`: Frequent bugs, language desyncs, and battery drain issues.

## 3. Mental Model and Directory Organization

EinsteinEngines features live in:

| Side | Path | Key Contents |
| --- | --- | --- |
| Shared | `Content.Shared/_EinsteinEngines` | Language (Systems, Components), Silicon (Charge, Death, DeadStartupButton, BlindHealing), InteractionVerbs (Actions, Requirements, Events, Prototypes), Flight, Contests, HeightAdjust, Power/BatteryDrinker, Forensics/ScentTracker, SelfExtinguisher, Psionics, Items/RestrictedMelee, Humanoid/RomanNaming, Revolutionary, TelescopicBaton, Medical, CCVar |
| Server | `Content.Server/_EinsteinEngines` | Server systems, chat commands, and event handlers |
| Client | `Content.Client/_EinsteinEngines` | Client systems, visualizers, and XAML UI menus |
| Prototypes | `Resources/Prototypes/_EinsteinEngines` | YAML definitions for languages, interaction verbs, traits, and items |

## 4. Key Architectural Notes

### 4.1 Interaction Verbs Has 11 Action Types

The `Content.Shared._EinsteinEngines.InteractionVerbs.Actions` namespace provides these built-in action types for YAML-composed verbs:

| Action | Purpose |
| --- | --- |
| `ChatMessageAction` | Sends custom chat messages or popups |
| `ChangeStandingStateAction` | Knocks down or helps up the target |
| `ModifyHealthAction` | Applies healing or damage |
| `ModifyStatusEffectAction` | Applies or removes stun, sleep, or jitter |
| `ToggleSleepingAction` | Puts target to sleep or wakes them |
| `RaiseEventAction` | Raises a custom entity event for further processing |
| `JitterAction` | Applies jitter effect on the target |
| `ComplexAction` | Composes multiple sub-actions into a sequence |
| `ConditionalAction` | Executes different actions based on runtime conditions |
| `NoOpAction` | Placeholder action (useful in conditionals) |
| `OnUserAction` | Executes an action on the user performing the verb, not the target |

Requirements live in `Content.Shared._EinsteinEngines.InteractionVerbs.Requirements`:
- `AssortedRequirements.cs`: Distance checks, tool quality checks, consciousness checks, hand availability checks.
- `ComplexRequirement.cs`: Logical combination of requirements (AND/OR).
- `UtilityRequirements.cs`: Miscellaneous utility checks.

### 4.2 Silicon Systems Are Abstract-Based

All core silicon systems use the abstract class pattern:
- `SharedSiliconChargeSystem` (sealed, shared) — charge tick logic
- `SharedSiliconDeathSystem` (abstract, shared → concrete on server)
- `SharedDeadStartupButtonSystem` (abstract partial, shared → concrete on server)
- `SharedBlindHealingSystem` (abstract partial, shared → concrete on server)
- `SharedBatteryDrinkerSystem` (abstract, shared → concrete on server)

### 4.3 Language System Architecture

The language system has two parallel component paths:
- `LanguageSpeakerComponent` (shared): Tracks known languages and currently selected spoken language for the UI.
- `LanguageKnowledgeComponent` (server): Server-side authoritative state for which languages the entity speaks and understands.

Both must be present on sentient speaking mobs. The `SharedLanguageSystem` is abstract and extended on server by `LanguageSystem` which handles actual chat obfuscation and translation.

## 5. Fundamental Rules for `_ScavPrototype`

1. **Isolation**: Never create new files inside `_EinsteinEngines`. Place all custom features in `_ScavPrototype`.
2. **Languages**: To add a custom language, define a `LanguagePrototype` in `Resources/Prototypes/_ScavPrototype` and attach both `LanguageSpeakerComponent` and `LanguageKnowledgeComponent` to the mob.
3. **Synthetic Entities**: When making droid, drone, or cyborg entities, inherit from EinsteinEngines silicon components and always include `BatterySlotRequiresLockComponent`.
4. **Verbs**: Prefer YAML `InteractionVerbPrototype` over custom C# verbs. Check all 11 action types (`ComplexAction`, `ConditionalAction`, `OnUserAction`, etc.) before deciding that C# is necessary.
5. **Contests**: Use `ContestsSystem` for any entity-vs-entity strength/mass comparison rather than implementing ad-hoc checks.
