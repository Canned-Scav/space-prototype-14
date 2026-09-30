---
name: ss14-corvax
description: "Corvax Ecosystem: CorvaxGoob (calendar Announcer, TTS voice synthesis, Criminal Records, footprints, Skills, Nuclear Reactor, OfferItem, Russian grammar), Corvax Core interfaces (sponsors, Discord auth, queue), and CorvaxNext (AI remote silicon control)."
---

# Corvax Architecture and Ecosystem

This skill covers the Corvax layer in the repository, consisting of `_CorvaxGoob`, `_Corvax`, `_CorvaxNext`, and the root `Corvax/` interface libraries.

## 1. Scope and Boundaries

1. This skill covers:
   - `_CorvaxGoob`: Calendar-based announcer selection, TTS (Text-to-Speech) voice synthesis, Criminal Records examination, floor footprints with cleanable decal tracking, item offering, Skills subsystem, Nuclear Reactor / Fission Generator complex, station utilities (GhostBar, BookOfGreentext, MedipenRefiller, QuantumTelepad, SprayableWall, ChameleonStamp, ExecutionChair, DocumentPrinter, SecApartment, StationGoalPaper, ShuttleDroneLink, DiceOfFate, GrapplingGunHunter, PlantAnalyzer, MRP job management).
   - `_Corvax` & `Corvax/`: Shared interfaces for sponsors, Discord authentication, queue management, and the GuideGenerator wiki exporter.
   - `_CorvaxNext`: AI remote device and borg control, remote view BUI, alert integration.
2. For base SS14 networking and events, consult `ss14-netcode` and `ss14-events`.
3. For localization string standards, consult `ss14-localization-strings`.

## 2. Resource Reading Order

Read resources in this sequence:
1. `references/components-and-systems.md`: Full catalog of Corvax systems, components, and interfaces.
2. `references/integration-recipes.md`: Practical recipes for TTS integration, criminal records, and item offering.
3. `references/common-pitfalls.md`: Frequent pitfalls, TTS misuse, announcer misunderstanding, grammatical formatting bugs, and interface decoupling rules.

## 3. Mental Model and Directory Organization

| Layer | Path | Core Role |
| --- | --- | --- |
| `_CorvaxGoob` | `Content.Server/_CorvaxGoob`, `Content.Shared/_CorvaxGoob`, `Content.Client/_CorvaxGoob` | The largest module: Calendar Announcer, TTS voice synthesis, Criminal Records, Footprints, Skills, Nuclear Reactor/Turbine, Russian grammar, photo, station utilities |
| `_Corvax` | `Content.*/_Corvax` | Guide generator (wiki tool), sprite export services, wiki CVars |
| `Corvax/` | Root directory | Interface assemblies (`Content.Corvax.Interfaces.*`) for sponsors, Discord auth, join queue |
| `_CorvaxNext` | `Content.*/_CorvaxNext` | AI remote control of slaved borgs and remote devices, alert integration |

## 4. Key Architectural Notes

### 4.1 AnnouncerSystem Is NOT a Broadcast System

`AnnouncerSystem` in `Content.Server._CorvaxGoob.Announcer` is a **calendar-based announcer selection** system. It picks an `AnnouncerPrototype` for the current day/round based on a deterministic calendar algorithm, not on demand. Its public API:

- `ForceSetAnnouncer(AnnouncerPrototype announcer, int rounds = 1)`: Override the announcer for N rounds.
- `GetAnnouncerToday()`: Returns the currently selected `AnnouncerPrototype?`.
- `TryGetAnnouncerToday(out AnnouncerPrototype?)`: Safe accessor.

To broadcast station-wide chat messages, use `ChatSystem.DispatchStationAnnouncement(...)` from vanilla `Content.Server.Chat.Systems`, **not** the Corvax `AnnouncerSystem`.

### 4.2 TTS (Text-to-Speech)

The `TTSSystem` in `Content.Server._CorvaxGoob.TTS` is a multi-partial-class system (`.cs`, `.Announcements.cs`, `.RateLimit.cs`, `.Sanitize.cs`, `.SSML.cs`) providing server-side voice synthesis for chat and announcements. Shared types include `TTSComponent`, `TTSVoicePrototype`, `PlayTTSEvent`, `TTSAnnounceEvent`, and `TransformSpeakerVoiceEvent`.

### 4.3 Fission Nuclear Reactor Complex

Located in `Content.Server._CorvaxGoob.Power.Generation.FissionGenerator` and its shared counterpart, this is a multi-system engineering feature: `NuclearReactorSystem` (extends `SharedNuclearReactorSystem`), `TurbineSystem` (extends `SharedTurbineSystem`), `ReactorPartSystem` (extends `SharedReactorPartSystem`), `NuclearCentrifugeSystem`, `NuclearReactorMonitorSystem`, `GasTurbineMonitorSystem`.

## 5. Fundamental Rules for `_ScavPrototype`

1. **Isolation**: Never place new files in `_CorvaxGoob`, `_Corvax`, `_CorvaxNext`, or `Corvax/`. Place all custom content in `_ScavPrototype`.
2. **Localization & Grammar**: When authoring Russian `.ftl` strings for scavenger roles or items, utilize Corvax grammatical gender and case markers where appropriate.
3. **Station Broadcasts**: For station-wide audio announcements, use `ChatSystem.DispatchStationAnnouncement(...)` from vanilla, not the Corvax `AnnouncerSystem` which only selects which announcer voice pack to use.
4. **TTS**: To attach TTS voice to custom entities, add `TTSComponent` with a valid `TTSVoicePrototype` ID.
