# DeltaV: Common Pitfalls and Anti-Patterns

This document records frequent bugs and edge cases when working with DeltaV systems.

## 1. Oversized Items in Mouth Storage
- **Anti-pattern**: Attempting to put Small, Medium, or Large items into `MouthStorageComponent`.
- **Consequence**: Item placement fails silently or breaks character inventory UI.
- **Rule**: `MouthStorageComponent` is strictly designed for items of size `Tiny` (pills, coins, keys, microchips).

## 2. Carrying State Grid Crossing Desync
- **Anti-pattern**: Forcibly changing the parent transform of a carried mob without calling `CarryingSystem.Drop(...)`.
- **Consequence**: The carrier and carried entities become decoupled across grid boundaries, creating invisible ghost entities.
- **Rule**: Always release or drop carried entities via `CarryingSystem` before triggering teleports or grid changes.

## 3. Missing Cartridge Loader Integration
- **Anti-pattern**: Adding `NanoChatCardComponent` to an entity without `CartridgeLoaderComponent`.
- **Consequence**: Chat data is saved, but the user interface cannot be opened from the PDA.
- **Rule**: Ensure both `CartridgeLoaderComponent` and `NanoChatCardComponent` are present when configuring PDA communications.
