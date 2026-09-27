---
name: ss14-einstein-engines
description: Architecture and systems of EinsteinEngines (`_EinsteinEngines`): Language subsystem and translators, Silicon/IPC mechanics and battery charging, Interaction Verbs, Flight, and Contests/Height adjustment.
---

# EinsteinEngines Architecture and Systems

This skill covers the systems and components inherited from EinsteinEngines, located in `_EinsteinEngines` folders across `Content.Shared`, `Content.Server`, `Content.Client`, and `Resources/Prototypes`.

## 1. Scope and Boundaries

1. This skill covers:
   - Language subsystem, speech obfuscation, translation devices, and chat commands.
   - Silicon and IPC (Integrated Positronic Chassis) mechanics, battery drinking, charging, and reboot buttons.
   - Interaction Verbs system: YAML-defined contextual verb actions without C# boilerplate.
   - Flight and height adjustment systems.
2. For medical systems and cybernetics surgery, consult `ss14-shitmed`.
3. For base SS14 ECS patterns, consult `ss14-ecs-components` and `ss14-ecs-systems`.

## 2. Resource Reading Order

Read resources in this sequence:
1. `references/components-and-systems.md`: Complete register of components, systems, prototypes, and chat commands.
2. `references/integration-recipes.md`: Practical recipes for custom languages, IPC mobs, and YAML verbs.
3. `references/common-pitfalls.md`: Frequent bugs, language desyncs, and battery drain issues.

## 3. Mental Model and Directory Organization

EinsteinEngines features live in:
- `Content.Shared/_EinsteinEngines`: Shared components, events, prototypes, and systems.
- `Content.Server/_EinsteinEngines`: Server systems, chat commands, and event handlers.
- `Content.Client/_EinsteinEngines`: Client systems, visualizers, and XAML UI menus.
- `Resources/Prototypes/_EinsteinEngines`: YAML definitions for languages, interaction verbs, traits, and items.

## 4. Fundamental Rules for `_ScavPrototype`

1. **Isolation**: Never create new files inside `_EinsteinEngines`. Place all custom features in `_ScavPrototype`.
2. **Languages**: To add a custom language, define a `LanguagePrototype` in `Resources/Prototypes/_ScavPrototype` and attach `LanguageSpeakerComponent`.
3. **Synthetic Entities**: When making droid, drone, or cyborg entities, inherit from EinsteinEngines silicon components.
4. **Verbs**: Prefer YAML `InteractionVerbPrototype` over custom C# verbs whenever the desired logic can be composed from existing actions.
