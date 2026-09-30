# EinsteinEngines: Components and Systems Register

This catalog details the components, systems, and prototype classes in `_EinsteinEngines`.

## 1. Language Subsystem

### 1.1 Systems
- `SharedLanguageSystem` (abstract, `Content.Shared._EinsteinEngines.Language.Systems`) → `LanguageSystem` (server): Handles speaking, message obfuscation, language selection, and events.
- `SharedTranslatorSystem` (abstract, `Content.Shared._EinsteinEngines.Language.Systems`) → `TranslatorSystem` (server): Manages real-time translation equipment and implants.

### 1.2 Components
- `LanguageSpeakerComponent`: Stores known languages, the currently selected spoken language, and speech parameters. **Required on shared/client side** for UI.
- `LanguageKnowledgeComponent`: Server-side authoritative state tracking known languages and comprehension levels. **Required on server side** for actual obfuscation.
- `UniversalLanguageSpeakerComponent`: Grants ability to understand and speak all registered languages.
- `TranslatorImplantComponent`: Subdermal implant granting translation of specified languages.
- `HoldsTranslatorComponent`: Applied to mobs holding or wearing an active translator device.
- `HandheldTranslatorComponent`: Handheld translator gadget translating nearby speech.
- `IntrinsicTranslatorComponent`: Innate translation capability (e.g. for specialized alien species or station AI).

### 1.3 Prototypes and Commands
- `LanguagePrototype`: Defines language ID, display name, syllables for obfuscation, font, and chat icon. Must have at least 4-8 syllables in `replacement.syllables`.
- Commands: `/say :<language_id>` (speak in specific language), `/listlanguages` (list known languages), `/selectlanguage` (choose active language).

### 1.4 Architecture Notes
The language system has two parallel component paths that **both must be present** on sentient mobs:
1. `LanguageSpeakerComponent` (shared) → Used by client UI for language menu and active language display.
2. `LanguageKnowledgeComponent` (server) → Used by server for authoritative obfuscation and comprehension checks.

## 2. Silicon and IPC Subsystem

### 2.1 Systems
| System | Class | Side | Role |
| --- | --- | --- | --- |
| Charge | `SharedSiliconChargeSystem` | Shared (sealed) | Decrements charge based on entity activity, motors, and components |
| Death | `SharedSiliconDeathSystem` | Shared (abstract → server) | Triggers critical/death states when power reaches zero |
| Battery Drinker | `SharedBatteryDrinkerSystem` | Shared (abstract → server) | Draining power from APCs, cells, substations |
| Dead Startup | `SharedDeadStartupButtonSystem` | Shared (abstract partial → server) | Rebooting powered-down silicons |
| Blind Healing | `SharedBlindHealingSystem` | Shared (abstract partial → server) | Repairing silicon components without surgical illumination |

### 2.2 Components
- `SiliconComponent`: Base marker and parameter container for synthetic entities.
- `BatteryDrinkerComponent`: Allows the entity to drain charge from external batteries or APCs.
- `BatteryDrinkerSourceComponent`: Marks power entities that can be tapped for charge.
- `SiliconDownOnDeadComponent`: Forces the entity down when power runs out or when destroyed.
- `DeadStartupButtonComponent`: Adds a contextual interaction button to restart a dead silicon.
- `BatterySlotRequiresLockComponent`: Requires unlocking before an IPC's power cell can be accessed. **Always include on playable silicons.**

### 2.3 Architecture Notes
All core silicon systems follow the abstract-concrete pattern:
- Abstract system in `Content.Shared._EinsteinEngines` defines shared logic and events.
- Concrete sealed system in `Content.Server._EinsteinEngines` handles server-only behavior.
- The exception is `SharedSiliconChargeSystem` which is sealed and fully shared.

## 3. Interaction Verbs Subsystem

### 3.1 System
- `SharedInteractionVerbsSystem` (abstract, 18.5KB, `Content.Shared._EinsteinEngines.InteractionVerbs`): Core verb resolution, do-after management, requirement checking, and action execution.

### 3.2 Components
- `InteractionVerbsComponent`: Attached to an entity to receive target-directed verbs.
- `OwnInteractionVerbsComponent` (852 bytes): Gives an entity unique verbs it can perform on others.

### 3.3 Prototypes
- `InteractionVerbPrototype` (9.7KB): Defines verb label, category, icon, do-after delay, requirements, and actions. Complex YAML-first design.
- `InteractionPopupPrototype` (2.8KB): Popup display configuration for verb interactions.

### 3.4 Action Types (11 total)
Located in `Content.Shared._EinsteinEngines.InteractionVerbs.Actions`:

| Action | File Size | Purpose |
| --- | --- | --- |
| `ChatMessageAction` | 2.4KB | Sends custom chat messages or popups |
| `ChangeStandingStateAction` | 1.9KB | Knocks down or helps up the target |
| `ModifyHealthAction` | 1.2KB | Applies healing or damage |
| `ModifyStatusEffectAction` | 1.9KB | Applies or removes stun, sleep, jitter |
| `ToggleSleepingAction` | 1.8KB | Puts target to sleep or wakes them |
| `RaiseEventAction` | 1.8KB | Raises a custom entity event |
| `JitterAction` | 887B | Applies jitter effect |
| `ComplexAction` | 2.4KB | Composes multiple sub-actions into a sequence |
| `ConditionalAction` | 2.5KB | Executes different actions based on runtime conditions |
| `NoOpAction` | 1KB | Placeholder (useful in conditional branches) |
| `OnUserAction` | 1.4KB | Executes action on the **user**, not the target |

### 3.5 Requirements (3 files)
Located in `Content.Shared._EinsteinEngines.InteractionVerbs.Requirements`:

| File | Contents |
| --- | --- |
| `AssortedRequirements.cs` (2.9KB) | Distance checks, tool quality checks, consciousness checks, hand availability |
| `ComplexRequirement.cs` (963B) | Logical AND/OR combination of sub-requirements |
| `UtilityRequirements.cs` (513B) | Miscellaneous utility condition checks |

### 3.6 Supporting Types
- `InteractionAction.cs` (2.4KB): Base abstract class for all actions.
- `InteractionArgs.cs` (2.7KB): Arguments passed to action execution.
- `InteractionRequirement.cs` (932B): Base abstract class for all requirements.
- Events directory: Verb-related events for the event bus.

## 4. Flight Subsystem

- `SharedFlightSystem` (abstract, `Content.Shared._EinsteinEngines.Flight`) → `FlightSystem` (server): Handles activating flight, gravity interactions, and fixture mask toggling.
- `FlyingVisualizerSystem` (client): Client rendering of flight elevation and shadows.
- `FlightComponent`: Defines flight parameters (speed bonus, collision mask overrides, elevation).
- `FlightVisualsComponent`: Client-side visual state.

## 5. Contests and Height Adjustment

- `ContestsSystem` (sealed partial, `Content.Shared._EinsteinEngines.Contests`): Standardized contest checks between two entities (mass, strength, grappling power). Used by DeltaV's `CarryingSystem`.
- `HeightAdjustSystem` (sealed, `Content.Shared._EinsteinEngines.HeightAdjust`): Character height sliders in character setup, adjusting eye height, sprite scaling, and hitboxes.
- `BloodstreamAffectedByMassComponent` / `BloodstreamAdjustSystem`: Scales reagent metabolism and blood capacity based on entity mass.

## 6. Forensics

- `SharedScentTrackerSystem` (abstract, `Content.Shared._EinsteinEngines.Forensics.Systems`): Scent tracking for bloodhound-style mechanics.

## 7. Self-Extinguisher

- `SharedSelfExtinguisherSystem` (abstract partial, `Content.Shared._EinsteinEngines.SelfExtinguisher`): Automatic fire suppression for entities with self-extinguishing capability.

## 8. Psionics

- `TelepathyComponent` (`Content.Shared._EinsteinEngines.Psionics`): Enables psionic telepathic communication channel.

## 9. Combat and Items

- `RestrictedMeleeSystem` (sealed, `Content.Shared._EinsteinEngines.Items`): Restricts melee weapon usage to entities meeting specific conditions.
- `TelescopicBaton` namespace: Telescopic baton mechanics.

## 10. Humanoid

- `RomanNamingSystem` (sealed partial, `Content.Shared._EinsteinEngines.Humanoid`): Roman-style name generation for specific species/cultures.

## 11. Revolutionary

- `RevolutionaryConverterSystem` (sealed, `Content.Shared._EinsteinEngines.Revolutionary`): Extended revolutionary conversion mechanics.

## 12. Configuration

- `Content.Shared._EinsteinEngines.CCVar`: EinsteinEngines-specific configuration variables.
- `Content.Shared._EinsteinEngines.Medical`: Medical system extensions.
