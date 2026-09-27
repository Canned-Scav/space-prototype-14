# EinsteinEngines: Components and Systems Register

This catalog details the components, systems, and prototype classes in `_EinsteinEngines`.

## 1. Language Subsystem

### 1.1 Systems
- `SharedLanguageSystem` / `LanguageSystem`: Handles speaking, message obfuscation, language selection, and events.
- `SharedTranslatorSystem` / `TranslatorSystem`: Manages real-time translation equipment and implants.

### 1.2 Components
- `LanguageSpeakerComponent`: Stores known languages, the currently selected spoken language, and speech parameters.
- `LanguageKnowledgeComponent`: Server-side state tracking known languages and comprehension levels.
- `UniversalLanguageSpeakerComponent`: Grants ability to understand and speak all registered languages.
- `TranslatorImplantComponent`: Subdermal implant granting translation of specified languages.
- `HoldsTranslatorComponent`: Applied to mobs holding or wearing an active translator device.
- `HandheldTranslatorComponent`: Handheld translator gadget translating nearby speech.
- `IntrinsicTranslatorComponent`: Innate translation capability (e.g. for specialized alien species or station AI).

### 1.3 Prototypes and Commands
- `LanguagePrototype`: Defines language ID, display name, syllables for obfuscation, font, and chat icon.
- Commands: `/say :<language_id>` (speak in specific language), `/listlanguages` (list known languages), `/selectlanguage` (choose active language).

## 2. Silicon and IPC Subsystem

### 2.1 Systems
- `SiliconChargeSystem`: Decrements charge over time based on entity activity, motors, and attached components.
- `SiliconChargeDeathSystem`: Triggers critical down and death states when power reaches zero.
- `BatteryDrinkerSystem`: Implements draining power from external power sources (APCs, power cells, substations).
- `SharedDeadStartupButtonSystem`: Coordinates rebooting powered silicons.
- `BlindHealingSystem`: Allows repairing silicon components without full surgical illumination.

### 2.2 Components
- `SiliconComponent`: Base marker and parameter container for synthetic entities.
- `BatteryDrinkerComponent`: Allows the entity to drain charge from external batteries or APCs.
- `BatteryDrinkerSourceComponent`: Marks power entities that can be tapped for charge.
- `SiliconDownOnDeadComponent`: Forces the entity down when power runs out or when destroyed.
- `DeadStartupButtonComponent`: Adds a contextual interaction button to restart a dead silicon.
- `BatterySlotRequiresLockComponent`: Requires unlocking before an IPC's power cell can be accessed.

## 3. Interaction Verbs Subsystem

### 3.1 Components
- `InteractionVerbsComponent`: Attached to an entity to receive target-directed verbs.
- `OwnInteractionVerbsComponent`: Gives an entity unique verbs it can perform on others.

### 3.2 Prototypes and Actions
- `InteractionVerbPrototype`: Defines verb label, category, icon, do-after delay, and actions.
- Built-in Action Types (`Content.Shared._EinsteinEngines.InteractionVerbs.Actions`):
  - `ChatMessageAction`: Sends custom chat messages or popups.
  - `ChangeStandingStateAction`: Knocks down or helps up the target.
  - `ModifyHealthAction`: Applies healing or damage.
  - `ModifyStatusEffectAction`: Applies or removes stun, sleep, or jitter.
  - `ToggleSleepingAction`: Puts target to sleep or wakes them.
  - `RaiseEventAction`: Raises a custom entity event for further processing.
- Requirements (`Content.Shared._EinsteinEngines.InteractionVerbs.Requirements`):
  - Distance checks, tool checks, conscious/unconscious target checks, hand availability checks.

## 4. Flight Subsystem

- `SharedFlightSystem` / `FlightSystem`: Handles activating flight, gravity interactions, and fixture mask toggling.
- `FlyingVisualizerSystem`: Client rendering of flight elevation and shadows.
- `FlightComponent`: Defines flight parameters (speed bonus, collision mask overrides, elevation).
- `FlightVisualsComponent`: Client-side visual state.

## 5. Contests and Height Adjustment

- `ContestsSystem`: Standardized contest checks between two entities (e.g. comparing mass, strength, or grappling power).
- `HeightAdjustSystem`: Handles character height sliders in character setup, adjusting eye height, sprite scaling, and hitboxes.
- `BloodstreamAffectedByMassComponent` / `BloodstreamAdjustSystem`: Scales reagent metabolism and blood capacity based on entity mass.
