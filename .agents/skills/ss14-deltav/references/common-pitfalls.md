# DeltaV: Common Pitfalls and Anti-Patterns

This document records frequent bugs and edge cases when working with DeltaV systems.

## 1. Oversized Items in Mouth Storage

- **Anti-pattern**: Attempting to put Small, Medium, or Large items into `MouthStorageComponent`.
- **Consequence**: Item placement fails silently or breaks character inventory UI.
- **Rule**: `MouthStorageComponent` is strictly designed for items of size `Tiny` (pills, coins, keys, microchips).

## 2. Carrying State Grid Crossing Desync

- **Anti-pattern**: Forcibly changing the parent transform of a carried mob without going through `CarryingSystem`.
- **Consequence**: The carrier and carried entities become decoupled across grid boundaries, creating invisible ghost entities. `CarryingSystem` subscribes to `EntParentChangedMessage` specifically to handle this.
- **Rule**: Always release or drop carried entities via `CarryingSystem` before triggering teleports, shuttle docking, or grid changes.

## 3. Missing Cartridge Loader Integration

- **Anti-pattern**: Adding `NanoChatCardComponent` to an entity without `CartridgeLoaderComponent`.
- **Consequence**: Chat data is saved, but the user interface cannot be opened from the PDA.
- **Rule**: Ensure both `CartridgeLoaderComponent` (with `NanoChatCartridge` in `installedCartridges`) and `NanoChatCardComponent` are present when configuring PDA communications.

## 4. Resolving Feroxi System from Shared Code

- **Anti-pattern**: Attempting to inject or resolve `FeroxiDehydrateSystem` in shared assemblies (`Content.Shared`).
- **Consequence**: Compilation error or runtime `NullReferenceException` because `FeroxiDehydrateSystem` exists only in `Content.Server._DV.Feroxi`.
- **Rule**: Feroxi hydration logic is server-only. If shared code needs to check hydration state, read `FeroxiDehydrateComponent` data directly (it's a shared component) without calling the server system.

## 5. Ignoring Contest System in Carrying

- **Anti-pattern**: Adding flat carry weight limits instead of using the existing `ContestsSystem` integration.
- **Consequence**: Inconsistent carry mechanics that don't respect entity mass, strength traits, or EinsteinEngines contest modifiers.
- **Rule**: `CarryingSystem` already uses `ContestsSystem` from `_EinsteinEngines` for strength-based carry checks. Extend the contest system's modifiers rather than duplicating weight logic.

## 6. Direct Manipulation of CarryingComponent

- **Anti-pattern**: Adding or removing `CarryingComponent`, `BeingCarriedComponent`, or `CarryingSlowdownComponent` manually via `AddComp`/`RemComp`.
- **Consequence**: Breaks virtual item slot tracking, movement modifier cleanup, and pulling system interaction. The system manages a complex lifecycle involving `VirtualItemDeletedEvent`, pulling release, and buckle checks.
- **Rule**: Always use `CarryingSystem` public methods for pick-up and drop operations. The system handles all component lifecycle internally.

## 7. Cosmic Cult Multi-System Confusion

- **Anti-pattern**: Looking for all Cosmic Cult logic in `SharedCosmicCultSystem` and ignoring `SharedMonumentSystem` and `SharedCosmicGlyphSystem`.
- **Consequence**: Missing monument charging logic (7.4KB in `SharedMonumentSystem`) or glyph mechanics when implementing cult-related interactions.
- **Rule**: The Cosmic Cult spans three shared systems. Check all three before implementing cult-interacting features.
