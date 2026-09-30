---
name: ss14-goobstation
description: "Goob Station (`Content.Goobstation.*`, `_Goobstation`): upstream fork projects, UIKit rich controls, antagonists (Changeling, Heretic, Blob, Devil, DarkLord, Shadowling, Wraith, Nightmare, Slasher, ChronoLegionnaire, Xenomorph, Hastur), augmentations/autosurgeon, MartialArts, Grab system, Dash, Sandevistan, MantisBlades, Factory, Enchanting, Virology, Supermatter, Vehicles, Xenobiology, and clean integration from `_ScavPrototype`."
---

# Goob Station Architecture and Systems

This skill serves as the working architectural standard for Goob Station (`space-syndicate/Goob-Station`), which is the direct upstream repository of `space-prototype-14`.

## 1. Scope and Boundaries

1. This skill covers systems, components, UI controls, and prototypes living in `Content.Goobstation.*`, `_Goobstation`, and related project assemblies.
2. For medical, surgery, and localized wound damage, refer to `ss14-shitmed`.
3. For language, IPC, flight, and interaction verbs, refer to `ss14-einstein-engines`.
4. For general fork isolation and edit markers, consult `ss14-upstream-maintenance`.

## 2. Resource Reading Order

When working with Goob Station features, read resources in this sequence:
1. `references/components-and-systems.md`: Complete register of Goob Station components, systems, and namespaces.
2. `references/integration-recipes.md`: Practical recipes for subscribing to events, resolving systems, and writing UI from `_ScavPrototype`.
3. `references/common-pitfalls.md`: Frequent bugs, prediction desyncs, and architectural anti-patterns.

## 3. Mental Model and Assembly Structure

Goob Station maintains its own set of C# projects alongside base SS14:

| Project | Execution Target | Responsibilities |
| --- | --- | --- |
| `Content.Goobstation.Shared` | Shared (Server + Client) | Components, network events, game rules, item logic. 177+ directories including all antagonists, combat systems, factories, etc. |
| `Content.Goobstation.Server` | Server | Game rule controllers, server-side entity tracking, admin commands |
| `Content.Goobstation.Client` | Client | Prediction systems, client state, custom UI controllers |
| `Content.Goobstation.UIKit` | Client | Custom UI controls (`IconButton`, `StaticSpriteView`, `ShaderLabel`, `TooltipTextureRect`), custom RichText tags |
| `Content.Goobstation.Common` | Shared | Shared helper structures, data types, traits |
| `Content.Goobstation.Maths` | Shared | Math utilities, trajectory calculations, curve solvers, `FixedPoint` math (used heavily by Shitmed) |

Prototypes and textures are located in:
- `Resources/Prototypes/_Goobstation`: YAML prototypes for entities, abilities, store entries, and game rules.
- `Resources/Textures/_Goobstation`: Sprites, RSIs, UI textures, and icons.

## 4. Scale of `Content.Goobstation.Shared`

The shared assembly is enormous (177+ directories). Major feature groups include:

| Category | Key Directories |
| --- | --- |
| **Antagonists** | `Changeling`, `Heretic`, `Blob`, `Devil`, `DarkLord`, `Shadowling`, `Wraith`, `Nightmare`, `Slasher`, `ChronoLegionnaire`, `Xenomorph`, `Hastur`, `Pirates`, `Traitor`, `Revolutionary` |
| **Combat** | `MartialArts`, `Grab`, `GrabIntent`, `Dash`, `Sandevistan`, `MantisBlades`, `BerserkerImplant`, `Boomerang`, `Bloodtrak`, `Weapons`, `Throwing`, `TableSlam`, `ContractorBaton`, `RecoilAbsorber`, `SmartLinkImplant` |
| **Engineering** | `Supermatter`, `Power`, `Factory`, `Wires`, `Electrocution`, `Atmos` |
| **Science** | `Xenobiology`, `Enchanting`, `Research` |
| **Medical** | `Virology`, `Disease`, `Surgery`, `Medical`, `CheckInfection` |
| **Movement** | `Vehicles`, `Sprinting`, `PhaseShift`, `MomentumSteering`, `Waddle`, `Stealth` |
| **Social** | `Emag`, `Emoting`, `Polls`, `Voice`, `Speech`, `Radio`, `Communications`, `Fax`, `TapeRecorder` |
| **Misc** | `Teleportation`, `Fishing`, `Guardian`, `Mimery`, `Voodoo`, `Religion`, `Exorcism`, `Bingle`, `SlotMachine`, `HoloCigar`, `GPS`, `Loudspeaker`, `CloneProjector` |

## 5. Fundamental Rules for `_ScavPrototype`

1. **Isolation**: Never create new files inside `Content.Goobstation.*` or `_Goobstation`. All custom code belongs in `_ScavPrototype`.
2. **Coupling**: Consume Goob Station systems via dependency injection (`[Dependency] private readonly SharedChangelingSystem _changeling = default!;`) or `_entityManager.System<T>()`.
3. **Prototypes**: When expanding on Goob Station content, inherit from their prototypes using `parent:` in `Resources/Prototypes/_ScavPrototype`.
4. **Minimal Upstream Edits**: If an upstream Goob method must be hooked, insert a 1-2 line event hook wrapped in `// scav-edit start` and `// scav-edit end`.
5. **Shared vs Server**: Always use `Shared*System` variants in shared code. Server-only systems (`ChangelingSystem`, `HereticSystem`, `BlobRuleSystem`) must only be resolved inside `Content.Server/_ScavPrototype`.
6. **FixedPoint Math**: Shitmed wound and trauma values use `Content.Goobstation.Maths.FixedPoint` — not `float` or `int`. Import this namespace when interacting with wound or damage amounts.
