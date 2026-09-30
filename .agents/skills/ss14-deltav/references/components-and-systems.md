# DeltaV: Components and Systems Register

Complete catalog of DeltaV systems, components, and game rules.

## 1. Carrying System

### 1.1 Core System
- **Namespace**: `Content.Shared._DV.Carrying`
- **System**: `CarryingSystem` (sealed, 421 lines) — Full carrying lifecycle: verb registration, do-after pickup, virtual item slot management, movement penalties, grid change handling, throw integration, and pseudo-item interaction.
- **Dependencies**: `ContestsSystem` from `_EinsteinEngines` (strength-based carry checks), `SharedPseudoItemSystem`, `SharedVirtualItemSystem`, `PullingSystem`, `StandingStateSystem`.

### 1.2 Components (all in `Content.Shared._DV.Carrying`)
| Component | Role |
| --- | --- |
| `CarriableComponent` | Marks entity as eligible to be picked up. Has `AlternativeVerb` for carry action. |
| `CarryingComponent` | Attached to the **carrier** (the entity doing the carrying). |
| `BeingCarriedComponent` | Attached to the **carried** entity. |
| `CarryingSlowdownComponent` | Defines movement speed penalties while carrying. |
| `CantCarryOthersComponent` | Marker preventing entity from carrying anyone. |

### 1.3 Events
- `CarryDoAfterEvent`: Do-after completion event for picking up a carried entity.

### 1.4 Sub-System
- `CarryingSlowdownSystem` (sealed, 985 bytes): Applies/removes movement slowdown modifiers when carrying state changes.

### 1.5 Key Implementation Details
- Carry verb registered as `AlternativeVerb` on `CarriableComponent`.
- Uses `VirtualItemDeletedEvent` for drop detection.
- Auto-drops on `EntParentChangedMessage` (grid crossing).
- Integrates with `SharedPseudoItemSystem` for storage slot insertion of carried entities.
- Uses `ContestsSystem` for mass-based strength comparisons.

## 2. Species Mechanics

### 2.1 Harpy
- **Shared Namespace**: `Content.Shared._DV.Harpy`
- **Server Namespace**: `Content.Server._DV.Harpy`
- **Systems**:
  - `HarpySingerSystem`: Manages vocal performance, song pitch modification, and acoustic abilities.
  - `HarpyVisualsSystem` (client): Client-side rendering of avian plumage, crests, and feather states.
- **Components**:
  - `HarpySingerComponent`: Attached to Harpy mobs to enable singing mechanics.
  - `SyrinxVoiceMaskComponent`: Voice modification device emulating harpy vocal cords.

### 2.2 Feroxi
- **Server-Only Namespace**: `Content.Server._DV.Feroxi`
- **System**: `FeroxiDehydrateSystem` — Monitors fluid levels, applying thirst debuffs and gradual damage when dehydrated. **Server-only**, do not resolve from shared code.
- **Component**: `FeroxiDehydrateComponent` — Stores current hydration points and consumption thresholds.

## 3. NanoChat and PDA Cartridges

- **Shared Namespace**: `Content.Shared._DV.NanoChat`, `Content.Shared._DV.CartridgeLoader`
- **Server Namespace**: `Content.Server._DV.NanoChat`, `Content.Server._DV.CartridgeLoader.Cartridges`
- **Systems**:
  - `NanoChatSystem`: Manages peer-to-peer and channel text messaging across crew PDAs.
  - `NanoChatCartridgeSystem`: Coordinates the CartridgeLoader UI interface for NanoChat.
- **Components**:
  - `NanoChatCardComponent`: Stored in PDA/ID items to preserve chat history and contact list.
  - `NanoChatCartridgeComponent`: Marker on the cartridge item.

## 4. Movement and Abilities

- **Namespace**: `Content.Shared._DV.Abilities`
- **Systems**:
  - `SharedCrawlUnderObjectsSystem` (shared): Allows low-profile crawling under tables and conveyors.
  - `ItemCougherSystem` (shared, 3.8KB): Forces entities to cough up stored items.
- **Components**:
  - `CrawlUnderObjectsComponent` (1.2KB): Configures crawl speed, collision changes, and which objects can be crawled under.
  - `ItemCougherComponent` (1.4KB): Defines cough-up triggers, intervals, and item ejection behavior.
  - `CoughingUpItemComponent` (732 bytes): Attached to items being coughed up.
  - `RummagerComponent`: Mark for entities that rummage through containers.
  - `AlwaysTriggerMousetrapComponent`: Forces mousetrap activation.
  - `UltraVisionComponent`: Enhanced vision capability.

## 5. Mouth Storage (Contraband)

- **Namespace**: `Content.Shared._DV.Storage.Components`
- **Component**: `MouthStorageComponent` — Concealed storage pocket inside the mouth for `Tiny`-sized contraband items only. Used in conjunction with `ItemCougherComponent` for forced ejection.
- **Server**: `Content.Server._DV.Storage` — Server-side mouth storage processing.

## 6. Cosmic Cult Antagonist

- **Shared Namespace**: `Content.Shared._DV.CosmicCult`
- **Server Namespace**: `Content.Server._DV.CosmicCult`
- **Shared Systems**:
  - `SharedCosmicCultSystem` (5.7KB): Core cult mechanics, conversion, and ability management.
  - `SharedMonumentSystem` (7.4KB): Monument charging, defense, and BUI coordination.
  - `SharedCosmicGlyphSystem` (1.2KB): Astral projection, conversion runes, and reality warping.
- **Components Directory**: `Content.Shared._DV.CosmicCult.Components` — All cult member and structure components.
- **Prototypes Directory**: `Content.Shared._DV.CosmicCult.Prototypes` — Cult ability and item prototypes.
- **UI**: `MonumentUI.cs` (1.3KB) — Monument BUI state and messages.
- **Events**: `CosmicCult.Events.cs`, `CosmicCult.Actions.cs`, `CosmicCult.DoAfter.cs`, `CosmicCultAssociateRuleEvent.cs`, `CleanseOnDoAfterEvent.cs`.
- **Misc**: `CosmicCultExamineComponent`, `MonumentOnDespawnComponent`.

## 7. Equipment and Vendors

- **Namespace**: `Content.Shared._DV.Salvage`, `Content.Shared._DV.VendingMachines`, `Content.Shared._DV.Holosign`
- **Systems**:
  - `MiningPointsSystem` / `ShopVendorSystem`: Point redemption for salvage tools and rewards.
  - `ChargeHolosignSystem`: Deployable energy barrier projection.
- **Namespace**: `Content.Shared._DV.Weapons`
  - `PlayerAccuracyModifierSystem`: Aiming time and stance accuracy modifiers on firearms.

## 8. Other DeltaV Subsystems

| Directory | Content |
| --- | --- |
| `Content.Shared._DV.Polymorph` | Polymorph transformation system |
| `Content.Shared._DV.Prying` | Door prying mechanics |
| `Content.Shared._DV.IdentityManagement` | Identity/name management |
| `Content.Shared._DV.Construction` | DeltaV construction recipes |
| `Content.Shared._DV.Weather` | Weather scheduling and effects |
| `Content.Shared._DV.Silicons` | DeltaV silicon extensions |
| `Content.Shared._DV.StepTrigger` | Step trigger extensions |
| `Content.Shared._DV.Whitelist` | Whitelist extensions |
| `Content.Shared._DV.Lathe` | Lathe recipe extensions |
| `Content.Shared._DV.EntityEffects` | DeltaV entity effects |
| `Content.Shared._DV.Roles` | DeltaV role definitions |
| `Content.Shared._DV.CCVars` | DeltaV configuration variables |
| `Content.Shared._DV.Actions` | DeltaV action definitions |
| `Content.Server._DV.Shuttles` | DeltaV shuttle systems |
| `Content.Server._DV.Paper` | Paper/document processing |
| `Content.Server._DV.Implants` | Implant installation |
| `Content.Server._DV.Objectives` | DeltaV objectives |
| `Content.Server._DV.Cargo` | Cargo system extensions |
| `Content.Server._DV.StationEvents` | DeltaV station events |
| `Content.Server._DV.VoiceMask` | Voice masking server logic |
