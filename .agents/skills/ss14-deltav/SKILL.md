---
name: ss14-deltav
description: "DeltaV (`_DV`): Carrying system with slowdown and contests, Harpy singing/syrinx voice, Feroxi hydration, NanoChat PDA messaging, CrawlUnderObjects, MouthStorage/ItemCougher, Cosmic Cult antagonist, mining vendors, holosigns, weather scheduling, silicons, and accuracy modifiers."
---

# DeltaV Architecture and Features

This skill covers the DeltaV layer located in `_DV` folders across `Content.Shared`, `Content.Server`, `Content.Client`, and `Resources/Prototypes`.

## 1. Scope and Boundaries

1. This skill covers:
   - **Carrying system**: `CarryingSystem` with `CarriableComponent`, `CarryingComponent`, `BeingCarriedComponent`, `CantCarryOthersComponent`, `CarryingSlowdownComponent` and `CarryingSlowdownSystem`, contest-based strength checks via `_EinsteinEngines.Contests.ContestsSystem`.
   - **Species mechanics**: Harpy (vocal syrinx, singing via `HarpySingerSystem`, voice masking via `SyrinxVoiceMaskComponent`) and Feroxi (hydration monitoring via `FeroxiDehydrateSystem`).
   - **NanoChat and PDA cartridges**: Peer-to-peer messaging (`NanoChatSystem`, `NanoChatCartridgeSystem`, `NanoChatCardComponent`).
   - **Movement abilities**: `CrawlUnderObjectsComponent` + `SharedCrawlUnderObjectsSystem` for crawling under tables and conveyors, `ItemCougherSystem` / `ItemCougherComponent` for coughing up stored items.
   - **Mouth storage**: `MouthStorageComponent` for tiny contraband concealment (DeltaV-specific `Content.Shared._DV.Storage.Components`), plus `CoughingUpItemComponent` for forced item ejection.
   - **Cosmic Cult antagonist**: `SharedCosmicCultSystem`, `SharedMonumentSystem`, `SharedCosmicGlyphSystem`, monument BUI, glyph prototypes, associate rule events.
   - **Equipment and vendors**: `MiningPointsSystem` / `ShopVendorSystem`, `ChargeHolosignSystem`, `PlayerAccuracyModifierSystem`.
   - **Other DeltaV subsystems**: Polymorph, Prying, IdentityManagement, Construction, Weather scheduling, Silicons, StepTrigger, Whitelist, Shuttles, Paper, Implants, Objectives, Cargo, StationEvents, VoiceMask, EntityEffects, CCVars.
2. For medical systems and cybernetics, consult `ss14-shitmed`.
3. For language and IPC mechanics, consult `ss14-einstein-engines`.
4. For base inventory and hands systems, consult `Content.Shared.Hands`.

## 2. Resource Reading Order

Read resources in this sequence:
1. `references/components-and-systems.md`: Full catalog of DeltaV components, systems, and mechanics.
2. `references/integration-recipes.md`: Practical recipes for PDA messaging, carrying interactions, and contraband storage.
3. `references/common-pitfalls.md`: Common bugs, carrying desyncs, and cartridge setup errors.

## 3. Mental Model and Directory Organization

| Side | Path | Key Contents |
| --- | --- | --- |
| Shared | `Content.Shared/_DV` | Carrying (full system + 6 components), NanoChat, Harpy, CosmicCult, Holosign, CrawlUnderObjects/ItemCougher, Salvage, Weapons, Weather, Abilities, Roles, CCVars, VendingMachines, Polymorph, Prying, IdentityManagement, Silicons, StepTrigger, Whitelist |
| Server | `Content.Server/_DV` | Feroxi dehydration, CosmicCult game rule, NanoChat server, CartridgeLoader, Harpy server, Silicons, Weather, Shuttles, Weapons, Cargo, Implants, Objectives, StationEvents, Paper, VendingMachines, VoiceMask |
| Client | `Content.Client/_DV` | Client UI overlays, Harpy visual systems, cartridge interfaces, cosmic cult UI |
| Prototypes | `Resources/Prototypes/_DV` | Prototypes for species, PDA programs, mining vendors, cosmic cult items, weather configs |

## 4. Key Architectural Notes

### 4.1 Carrying System Uses Contests

`CarryingSystem` depends on `ContestsSystem` from `_EinsteinEngines` for strength-based carry attempts. The carry verb is an `AlternativeVerb` on `CarriableComponent`. Dropping uses `VirtualItemDeletedEvent`. Grid/parent changes auto-drop via `EntParentChangedMessage`. The system also integrates with `SharedPseudoItemSystem` for pseudo-item storage interaction.

### 4.2 Feroxi Is Server-Only

`FeroxiDehydrateSystem` and `FeroxiDehydrateComponent` live only in `Content.Server._DV.Feroxi`, not in shared code. Do not attempt to resolve this system from shared assemblies.

### 4.3 Cosmic Cult Is a Multi-System Feature

The Cosmic Cult spans three major shared systems (`SharedCosmicCultSystem`, `SharedMonumentSystem`, `SharedCosmicGlyphSystem`) with extensive component hierarchies, a BUI for monuments (`MonumentUI`), prototype definitions in `Content.Shared._DV.CosmicCult.Prototypes`, and server-side game rule processing in `Content.Server._DV.CosmicCult`.

## 5. Fundamental Rules for `_ScavPrototype`

1. **Isolation**: Never add files to `_DV`. Keep all custom content in `_ScavPrototype`.
2. **Mob Rescue**: When designing scavenging rescue or traversal gear, leverage `CarryingSystem` and its slowdown components rather than writing redundant dragging code. Use the `ContestsSystem` for strength checks.
3. **Contraband**: Use `MouthStorageComponent` (from `Content.Shared._DV.Storage.Components`) for small concealable salvage items. Size must be `Tiny`.
4. **Crawling**: For entities that should crawl under obstacles, attach `CrawlUnderObjectsComponent` instead of implementing custom collision logic.
