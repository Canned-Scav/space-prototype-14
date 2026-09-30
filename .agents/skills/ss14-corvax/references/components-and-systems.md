# Corvax: Components and Systems Register

Complete catalog of systems, components, and interfaces across Corvax modules.

## 1. CorvaxGoob Subsystems (`_CorvaxGoob`)

### 1.1 Calendar-Based Announcer Selection
- **Namespace**: `Content.Server._CorvaxGoob.Announcer`
- **System**: `AnnouncerSystem` — **NOT a broadcast system**. Selects an `AnnouncerPrototype` for the current day/round using a deterministic calendar algorithm with per-announcer seeds.
- **Prototype**: `AnnouncerPrototype` — Defines voice lines, alert sounds, min/max days per month, chance probability.
- **Public API**:
  - `ForceSetAnnouncer(AnnouncerPrototype announcer, int rounds = 1)`: Override announcer for N rounds.
  - `GetAnnouncerToday() → AnnouncerPrototype?`: Get the current day's announcer.
  - `TryGetAnnouncerToday(out AnnouncerPrototype?) → bool`: Safe accessor.
- **Internal**: Subscribes to `GameRunLevelChangedEvent` to recalculate each round. Uses `CCCVars.CalendarAnnouncerEnabled` CVar.

### 1.2 TTS (Text-to-Speech) Voice Synthesis
- **Server Namespace**: `Content.Server._CorvaxGoob.TTS`
- **System**: `TTSSystem` (sealed partial, multi-file):
  - `TTSSystem.cs`: Core TTS processing and speech synthesis.
  - `TTSSystem.Announcements.cs`: TTS for station announcements.
  - `TTSSystem.RateLimit.cs`: Per-player TTS rate limiting.
  - `TTSSystem.Sanitize.cs`: Input sanitization and profanity filtering.
  - `TTSSystem.SSML.cs`: SSML (Speech Synthesis Markup Language) generation.
  - `VoiceMaskSystem.TTS.cs`: TTS integration with voice mask devices.
- **Shared Components**: `TTSComponent`, `TTSVoicePrototype`, `PlayTTSEvent`, `RequestPreviewTTSEvent`, `TTSAnnounceEvent`, `TransformSpeakerVoiceEvent`.

### 1.3 Criminal Records
- **Server Namespace**: `Content.Server._CorvaxGoob.CriminalRecords`
- **System**: `CriminalRecordExamineSystem` — Adds criminal record information to entity examine text. Server-only.
- **Note**: The core `CriminalRecordsSystem` and `CriminalRecordsConsoleSystem` live in vanilla `Content.Server.CriminalRecords`, not in CorvaxGoob. CorvaxGoob only extends the examine behavior.

### 1.4 Corvax Footprints
- **Server Namespace**: `Content.Server._CorvaxGoob.CorvaxFootPrint`
- **Systems**:
  - `CorvaxFootprintSystem`: Spawns decal tracks on floor tiles when entities step into blood, water, or chemical puddles.
  - `CleanableDecalTrackerSystem`: Tracks and manages cleanup of footprint decals.
- **Shared Namespace**: `Content.Shared._CorvaxGoob.CorvaxFootPrint`
- **Components**: `CorvaxFootPrintComponent` — Configures footprint appearance, stride distance, and fading rates.

### 1.5 Skills System
- **Shared Namespace**: `Content.Shared._CorvaxGoob.Skills`
- **System**: `SharedSkillsSystem` (abstract) → `SkillsSystem` (server, sealed partial).
- **Data**: `Skills.cs` — Skills enum/data definitions.

### 1.6 Nuclear Reactor / Fission Generator Complex
- **Shared Namespace**: `Content.Shared._CorvaxGoob.Power.Generation.FissionGenerator`
- **Server Namespace**: `Content.Server._CorvaxGoob.Power.Generation.FissionGenerator`
- **Shared Systems** (abstract):
  - `SharedNuclearReactorSystem`
  - `SharedTurbineSystem`
  - `SharedReactorPartSystem`
- **Server Systems**:
  - `NuclearReactorSystem` (extends SharedNuclearReactorSystem, sealed partial, multi-file including `.cs` and `NuclearReactorSystemStation.cs`)
  - `TurbineSystem` (extends SharedTurbineSystem)
  - `ReactorPartSystem` (extends SharedReactorPartSystem, multi-file including `ReactorPartSystem.Item.cs`)
  - `NuclearCentrifugeSystem`: Uranium enrichment processing.
  - `NuclearReactorMonitorSystem`: Reactor monitoring console BUI.
  - `GasTurbineMonitorSystem`: Gas turbine monitoring console BUI.

### 1.7 Item Offering
- **Shared Namespace**: `Content.Shared._CorvaxGoob.OfferItem`
- **System**: `SharedOfferItemSystem` (abstract partial, multi-file: `.cs`, `.Interactions.cs`, `.Verbs.cs`) → `OfferItemSystem` (server).
- Enables player-to-player item offering popups without dropping items on the ground.

### 1.8 Station Utilities
- `BluespaceHarvesterSystem` (server): Bluespace energy extractor machine with risk/reward generation. Also `BluespaceHarvesterBundleSystem`, `BluespaceHarvesterRiftSystem`.
- `QuantumTelepadSystem` (server, sealed partial): Paired quantum teleportation pads.
- `MedipenRefillerSystem` / `MedipenSystem` (server): Medical device refilling and medipen logic.
- `SprayableWallSystem` (shared, sealed): Sprayable barricade walls.
- `SharedChameleonStampSystem` (shared, abstract) → `ChameleonStampSystem` (server): Chameleon stamp disguise.
- `BookOfGreentextSystem` (shared) / `CurseOfBookOfGreentextSystem` (server): Book of Greentext mechanics.
- `ExecutionChairSystem` (server, sealed partial): Electric execution chair.
- `SecApartmentSystem` (server, sealed partial): Security apartment assignment.
- `StationGoalPaperSystem` (server, sealed): Station goal paper generation.
- `GhostBarSystem` (server): Ghost bar lounge area management.
- `DiceOfFateSystem` (server, sealed): Random fate dice rolls with consequences.
- `DocumentPrinterSystem` (server): Document printing machine.
- `PhotoSystem` (shared abstract → server sealed partial): In-game photography.
- `SharedPhotoSystem` (shared): Photo data and rendering.
- `StaminaDamageModifierOnCollideSystem` (server): Collision stamina damage.
- `AnimationPlayerSystem` (server): Animation playback.
- `AppearanceConverterSystem` (server): Appearance data transformation.
- `PlantAnalyzerSystem` (server, extends `AbstractAnalyzerSystem<PlantAnalyzerComponent, PlantAnalyzerDoAfterEvent>`): Plant analysis tool.
- `SharedGrapplingGunHunterSystem` (shared, abstract) → `GrapplingGunHunterSystem` (server): Grappling hook gun.
- `ShuttleDroneLinkSystem` (server): Shuttle drone remote control linking.
- `HailerDeathSoundSystem` (server): Hailer clothing death sound.
- `TargetEventsSystem` / `ExecuteTargetEventsOnTriggerSystem` (server): Target-based event execution.
- `MrpJobSystem` (shared) / `MrpJobServerSystem` (server): MRP (Medium Role Play) job management.
- `SharedTogglePowerSystem` (shared, abstract) → `TogglePowerSystem` (server): Generic power toggling.
- `SharedVendingMachineSystem` (shared, abstract partial) → `VendingMachineSystem` (server, sealed partial): Enhanced vending machines.
- `SharedNuclearReactorSystem` / `TurbineSystem` (see §1.6).
- `MaterialSystem` (shared, sealed): Material composition tracking.

### 1.9 VendingMachines (CorvaxGoob Overrides)
- **Shared**: `Content.Shared._CorvaxGoob.VendingMachines.SharedVendingMachineSystem` (abstract partial).
- **Server**: `Content.Server._CorvaxGoob.VendingMachines.VendingMachineSystem` (sealed partial).

### 1.10 CCVars
- **Shared**: `Content.Shared._CorvaxGoob.CCCVars` — CorvaxGoob-specific configuration variables including `CalendarAnnouncerEnabled`.

## 2. Corvax Core Interfaces (`Corvax/`)

- `ISharedSponsorsManager`: Queries player tier, sponsor markings, ghost trails, and loadout unlocks.
- `IServerJoinQueueManager`: Controls server connection queueing when population is near cap.
- `IServerDiscordAuthManager`: Validates player Discord linking and verification.
- `GuideGenerator`: Automated wiki documentation and prototype JSON generator.

## 3. CorvaxNext: Silicon Remote Control (`_CorvaxNext`)

- **Shared Namespace**: `Content.Shared._CorvaxNext`
- **Directories**: `Alert`, `Silicons`
- Systems and components for AI remote entity takeover, slaved chassis control, and remote camera management.
- Alert integration for silicon status indicators.
