# DeltaV: Components and Systems Register

Complete catalog of DeltaV systems, components, and game rules.

## 1. Species Mechanics

### 1.1 Harpy
- **Namespace**: `Content.Shared._DV.Harpy`, `Content.Server._DV.Harpy`
- **Systems**:
  - `HarpySingerSystem`: Manages vocal performance, song pitch modification, and acoustic abilities.
  - `HarpyVisualsSystem`: Client-side rendering of avian plumage, crests, and feather states.
- **Components**:
  - `HarpySingerComponent`: Attached to Harpy mobs to enable singing mechanics.
  - `SyrinxVoiceMaskComponent`: Voice modification device emulating harpy vocal cords.

### 1.2 Feroxi
- **Namespace**: `Content.Shared._DV.Feroxi`, `Content.Server._DV.Feroxi`
- **Systems**:
  - `FeroxiDehydrateSystem`: Monitors fluid levels, applying thirst debuffs and gradual damage when dehydrated.
- **Components**:
  - `FeroxiDehydrateComponent`: Stores current hydration points and consumption thresholds.

## 2. NanoChat and PDA Cartridges

- **Namespace**: `Content.Shared._DV.NanoChat`, `Content.Server._DV.CartridgeLoader.Cartridges`
- **Systems**:
  - `NanoChatSystem`: Manages peer-to-peer and channel text messaging across crew PDAs.
  - `NanoChatCartridgeSystem`: Coordinates the CartridgeLoader UI interface for NanoChat.
- **Components**:
  - `NanoChatCardComponent`: Stored in PDA/ID items to preserve chat history and contact list.
  - `NanoChatCartridgeComponent`: Marker on the cartridge item.

## 3. Physical Interactions and Movement

- **Namespace**: `Content.Shared._DV.Carrying`, `Content.Shared._DV.Storage.Components`
- **Systems**:
  - `CarryingSystem`: Handles carrying players on shoulders, movement penalties, and dropping carried entities.
  - `CrawlUnderObjectsSystem`: Allows low-profile crawling under tables and conveyors.
- **Components**:
  - `MouthStorageComponent`: Concealed storage pocket inside the mouth for small contraband.

## 4. Cosmic Cult Antagonist

- **Namespace**: `Content.Shared._DV.CosmicCult`, `Content.Server._DV.CosmicCult`
- **Systems**:
  - `CosmicCultSystem` / `CosmicCultRuleSystem`: Game rule lifecycle and victory tracking.
  - `MonumentSystem`: Charging and defense of cosmic monuments.
  - `SharedCosmicGlyphSystem`: Astral projection, conversion runes, and reality warping.

## 5. Equipment and Vendors

- **Namespace**: `Content.Shared._DV.Salvage`, `Content.Shared._DV.VendingMachines`, `Content.Shared._DV.Holosign`
- **Systems**:
  - `MiningPointsSystem` / `ShopVendorSystem`: Point redemption for salvage tools and rewards.
  - `ChargeHolosignSystem`: Deployable energy barrier projection.
  - `PlayerAccuracyModifierSystem`: Aiming time and stance accuracy modifiers on firearms.
