---
name: ss14-corvax
description: Architecture and ecosystem of Corvax, CorvaxGoob and CorvaxNext: station Announcer, Criminal Records, footprints, Russian localization/grammar helpers, remote silicon control, and Corvax interfaces.
---

# Corvax Architecture and Ecosystem

This skill covers the Corvax layer in the repository, consisting of `_CorvaxGoob`, `_Corvax`, `_CorvaxNext`, and the root `Corvax/` interface libraries.

## 1. Scope and Boundaries

1. This skill covers:
   - `_CorvaxGoob`: Announcer audio announcements, Criminal Records, floor footprints, item offering, Russian grammar integration, and station utilities.
   - `_Corvax` & `Corvax/`: Shared interfaces for sponsors, Discord authentication, queue management, and the GuideGenerator wiki exporter.
   - `_CorvaxNext`: AI remote device and borg control, remote view BUI.
2. For base SS14 networking and events, consult `ss14-netcode` and `ss14-events`.
3. For localization string standards, consult `ss14-localization-strings`.

## 2. Resource Reading Order

Read resources in this sequence:
1. `references/components-and-systems.md`: Full catalog of Corvax systems, components, and interfaces.
2. `references/integration-recipes.md`: Practical recipes for custom announcements, criminal records, and item offering.
3. `references/common-pitfalls.md`: Frequent pitfalls, grammatical formatting bugs, and interface decoupling rules.

## 3. Mental Model and Directory Organization

| Layer | Path | Core Role |
| --- | --- | --- |
| `_CorvaxGoob` | `Content.*/_CorvaxGoob` | The largest module: Announcer, Criminal Records, Footprints, Russian grammar, utilities |
| `_Corvax` | `Content.*/_Corvax` | Guide generator (wiki tool), sprite export services, wiki CVars |
| `Corvax/` | Root directory | Interface assemblies (`Content.Corvax.Interfaces.*`) for sponsors, Discord auth, join queue |
| `_CorvaxNext` | `Content.*/_CorvaxNext` | AI remote control of slaved borgs and remote devices |

## 4. Fundamental Rules for `_ScavPrototype`

1. **Isolation**: Never place new files in `_CorvaxGoob`, `_Corvax`, `_CorvaxNext`, or `Corvax/`. Place all custom content in `_ScavPrototype`.
2. **Localization & Grammar**: When authoring Russian `.ftl` strings for scavenger roles or items, utilize Corvax grammatical gender and case markers where appropriate.
3. **Audio Alerts**: Use `AnnouncerSystem` for station-wide or faction-wide raid sirens and broadcasts.
