# Corvax: Components and Systems Register

Complete catalog of systems, components, and interfaces across Corvax modules.

## 1. CorvaxGoob Subsystems (`_CorvaxGoob`)

### 1.1 Station Announcer
- **Namespace**: `Content.Server._CorvaxGoob.Announcer`
- **Systems**:
  - `AnnouncerSystem`: Manages station announcement audio queues, alert levels, and priority overrides.
- **Prototypes**:
  - `AnnouncerPrototype`: Defines voice lines, alert sounds, and background chimes.
- **Methods**:
  - `SendAnnouncement(...)`: Plays audio and sends a chat announcement across all station frequencies.

### 1.2 Criminal Records
- **Namespace**: `Content.Shared._CorvaxGoob.CriminalRecords`, `Content.Server._CorvaxGoob.CriminalRecords`
- **Systems**:
  - `CriminalRecordsSystem`: Server-side state holder for character criminal histories.
  - `CriminalRecordsConsoleSystem`: Coordinates security console BUI for editing and reviewing records.
- **Components**:
  - `CriminalRecord`: Record data containing character name, status (None, Wanted, Incarcerated, Released), and history entries.
  - `CriminalRecordsConsoleComponent`: Attached to security computers.

### 1.3 Corvax Footprints
- **Namespace**: `Content.Shared._CorvaxGoob.CorvaxFootPrint`, `Content.Server._CorvaxGoob.CorvaxFootPrint`
- **Systems**:
  - `CorvaxFootPrintSystem`: Spawns decal tracks on floor tiles when entities step into blood, water, or chemical puddles.
- **Components**:
  - `CorvaxFootPrintComponent`: Configures footprint appearance, stride distance, and fading rates.

### 1.4 Utilities and Mechanics
- `OfferItemSystem`: Enables player-to-player item offering popups without dropping items on the ground.
- `BluespaceHarvesterSystem`: Bluespace energy extractor machine with risk/reward generation.
- `QuantumTelepadSystem`: Paired quantum teleportation pads.
- `MedipenRefillerSystem`: Medical device that refills depleted autoinjectors.

## 2. Corvax Core Interfaces (`Corvax/`)

- `ISharedSponsorsManager`: Queries player tier, sponsor markings, ghost trails, and loadout unlocks.
- `IServerJoinQueueManager`: Controls server connection queueing when population is near cap.
- `IServerDiscordAuthManager`: Validates player Discord linking and verification.
- `GuideGenerator`: Automated wiki documentation and prototype JSON generator.

## 3. CorvaxNext: Silicon Remote Control (`_CorvaxNext`)

- `AiRemoteControlSystem`: Server and client systems handling remote entity takeover.
- `AiRemoteBrainComponent`: Attached to the controlling AI entity.
- `SharedAiRemoteControllerComponent`: Attached to slaved chassis or remote cameras.
- `RemoteDevicesBoundUserInterface`: BUI for switching between slaved devices.
