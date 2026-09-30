# Shitmed: Components and Systems Register

Complete catalog of Shitmed systems, components, and anatomical structures.

## 1. Wound System (Core Damage Router)

- **Namespace**: `Content.Shared._Shitmed.Medical.Surgery.Wounds.Systems`
- **System**: `WoundSystem` (sealed partial, 431+ lines, 17.8KB) — The central damage routing system.
  - Creates wound entities on targeted body parts when damage is received.
  - Manages wound healing ticks (configurable via `_medicalHealingTickrate`).
  - Triggers trauma via `TraumaSystem` when damage thresholds are exceeded.
  - Handles wound appearance updates via `SharedAppearanceSystem`.
  - Uses job queues (`IJobQueue`) for heavy processing.
- **Dependencies**: `SharedBodySystem`, `DamageableSystem`, `MobStateSystem`, `ThrowingSystem`, `InventorySystem`, `TraumaSystem`, `SharedHandsSystem`, `SharedAudioSystem`, `SharedPopupSystem`, `SharedContainerSystem`, `SharedTransformSystem`.
- **Components**: Located in `Content.Shared._Shitmed.Medical.Surgery.Wounds.Components`.

## 2. Trauma System

- **Namespace**: `Content.Shared._Shitmed.Medical.Surgery.Traumas.Systems`
- **System**: `TraumaSystem` (sealed partial, multi-partial via `InitProcess`, `InitBones`, `InitOrgans`) — Manages trauma application and resolution.
- **Predefined Trauma Types** (static `ProtoId<TraumaTypePrototype>` constants):

| Constant | Prototype ID | Effect |
| --- | --- | --- |
| `BoneDamage` | `"BoneDamage"` | Fractures, broken bone alerts, movement speed penalty |
| `OrganDamage` | `"OrganDamage"` | Internal organ degradation, failure states |
| `NerveDamage` | `"NerveDamage"` | Nerve impairment, reduced manipulation, item fumbling |
| `Dismemberment` | `"Dismemberment"` | Limb separation, entity drop, critical blood loss |
| `VeinsDamage` | `"VeinsDamage"` | Arterial bleeding, rapid blood loss requiring tourniquet |
| `Braindeath` | `"Braindeath"` | Irreversible brain death |

- **Dependencies**: `SharedStunSystem`, `MovementModStatusSystem`, `InventorySystem`, `WoundSystem`, `PainSystem`, `ConsciousnessSystem`, `MovementSpeedModifierSystem`, `StandingStateSystem`, `SharedBodySystem`, `SharedVirtualItemSystem`, `SharedAudioSystem`, `MobStateSystem`, `SharedPopupSystem`, `AlertsSystem`.
- **Internal**: Subscribes to broken bones alert ID `"BrokenBones"`.

## 3. Pain System

- **Namespace**: `Content.Shared._Shitmed.Medical.Surgery.Pain.Systems`
- **System**: `PainSystem` (sealed partial, 280 lines, 10.6KB) — Pain accumulation and effects.
- **Component**: `NerveComponent` (networked via `ComponentGetState`/`ComponentHandleState`).
- **Behavior**:
  - Pain accumulates per body part via `NerveComponent`.
  - Jitter at low pain thresholds.
  - Screaming at medium thresholds (~20% chance per tick, configurable).
  - Stun/knockdown at high thresholds via `SharedStunSystem`.
  - Consciousness loss via `ConsciousnessSystem` at extreme levels.
  - Falls via `StandingStateSystem`.
- **Update Interval**: `PainUpdateInterval = TimeSpan.FromSeconds(0.2)`.
- **Dependencies**: `SharedBodySystem`, `SharedAudioSystem`, `SharedPopupSystem`, `SharedJitteringSystem`, `SharedStunSystem`, `MobStateSystem`, `StandingStateSystem`, `WoundSystem`, `ConsciousnessSystem`, `TraumaSystem`.

## 4. Consciousness System

- **Namespace**: `Content.Shared._Shitmed.Medical.Surgery.Consciousness.Systems`
- **System**: `ConsciousnessSystem` (sealed partial, multi-file):
  - `ConsciousnessSystem.cs` (847B): Core initialization.
  - `ConsciousnessSystem.Helpers.cs` (19.6KB): Extensive helper methods for consciousness evaluation.
  - `ConsciousnessSystem.Process.cs` (8.4KB): Consciousness tick processing.
  - `ConsciousnessSystem.Networking.cs` (3KB): Network state synchronization.
- **Purpose**: Replaces much of vanilla `MobStateSystem` for determining conscious/unconscious state. Evaluates based on cumulative pain, blood loss, organ status, and external factors.
- **Serialization**: `ConsciousnessSerializable.cs` (1.5KB) — Network serialization for consciousness state.

## 5. Bloodstream System Override

- **File**: `Content.Shared._Shitmed.Surgery.SharedBloodstreamSystem.cs` (14KB)
- **Purpose**: Major override of vanilla bloodstream behavior. Integrates wound-based bleeding, arterial bleed rates from `VeinsDamage` trauma, and body-part-specific blood loss.

## 6. Targeting Subsystem

- **Namespace**: `Content.Shared._Shitmed.Targeting`
- **System**: `SharedTargetingSystem` (abstract → server `TargetingSystem`).
- **Component**: `TargetingComponent` — Tracks the player's currently selected target zone.
- **Target Zones**: `Head`, `Torso`, `Groin`, `LeftArm`, `RightArm`, `LeftHand`, `RightHand`, `LeftLeg`, `RightLeg`, `LeftFoot`, `RightFoot`.

## 7. Surgery System

- **Namespace**: `Content.Shared._Shitmed.Surgery`
- **System**: `SharedSurgerySystem` (abstract partial, 67KB+ across 3 files):
  - `SharedSurgerySystem.cs` (21.8KB): Core surgery logic, component management, tool handling.
  - `SharedSurgerySystem.Steps.cs` (45.6KB): Individual surgical step resolution, validation, completion.
  - `SharedSurgerySystem.Start.cs` (2.4KB): Surgery initiation and target validation.
- **Components**:
  - `SurgeryComponent`: Attached to surgery entities.
  - `SurgeryTargetComponent`: Marks entities eligible for surgery.
  - `SurgeryIgnoreClothingComponent`: Bypasses clothing removal requirement.
  - `SurgerySpeedModifierComponent`: Modifies surgery step speed.
  - `OperatingTableComponent`: Marks operating table surfaces.
  - `SanitizedComponent`: Marks sanitized surgical areas.
- **Events**: `SurgeryDoAfterEvent`, `SurgeryStepEvent`, `SurgeryStepDamageEvent`, `SurgeryStepDamageChangeEvent`, `SurgeryEvents`, `SurgeryUiRefreshEvent`.
- **UI**: `SurgeryUI.cs` — BUI state and messages.
- **Subdirectories**: `Steps/` (step type definitions), `Conditions/` (step prerequisites), `Effects/` (step consequences), `Tools/` (tool qualifications).

### 7.1 Surgical Steps and Tool Qualities

| Step | Tool Quality | Typical Tool |
| --- | --- | --- |
| Incision | Cutting | Scalpel |
| Retraction | Retracting | Retractor |
| Clamping | Clamping | Hemostat |
| Sawing | Sawing | Bone saw |
| BoneSetting | BoneSetting | Bone gel |
| Cauterization | Cauterizing | Cautery |
| Suturing | Suturing | Surgical suture / needle |

### 7.2 Surgery Tool Systems
- `SurgeryToolConditionsSystem` (sealed, 16 deps): Validates whether a tool meets step requirements.
- `SurgeryToolExamineSystem` (sealed, 14 deps): Adds tool quality information to examine text.

## 8. Autodoc (Automated Surgery)

- **Shared**: `SharedAutodocSystem` (abstract, `Content.Shared._Shitmed.Autodoc.Systems`): Automated surgery pod logic.
- **Shared**: `HandsFillSystem` (sealed, `Content.Shared._Shitmed.Autodoc.Systems`): Fills autodoc hands with required surgical tools.
- **Server**: Autodoc server-side processing in `Content.Server._Shitmed.Autodoc`.

## 9. Body Parts, Organs, and Effects

### 9.1 Shared
- **Namespace**: `Content.Shared._Shitmed.Body`
- **Directories**: `Components/`, `Events/`, `Organ/`, `Part/`, `Systems/`.
- `BodyPartEffectSystem` (sealed partial, `Content.Shared._Shitmed.BodyEffects`): Applies/removes effects based on body part status.
- `OrganEffectSystem` (sealed partial, `Content.Shared._Shitmed.BodyEffects`): Applies/removes effects based on organ status.

### 9.2 Server Organ Systems
- `HeartSystem` (sealed, `Content.Server._Shitmed.Body.Organ`): Heart circulation simulation, cardiac arrest.
- `EyesSystem` (sealed, `Content.Server._Shitmed.Body.Systems`): Vision processing, blindness on eye damage.
- `StatusEffectOrganSystem` (sealed, `Content.Server._Shitmed.Body.Organ`): Status effects from organ states.
- `DebrainedSystem` (sealed, `Content.Server._Shitmed.Body.Systems`): Brain removal consequences.
- `GenerateChildPartSystem` (sealed, `Content.Server._Shitmed.Body.BodyEffects.Subsystems`): Generates child body parts.
- `RandomStatusActivationSystem` (sealed, `Content.Server._Shitmed.Body.BodyEffects.Subsystems`): Random status effect triggers from body states.

## 10. Tourniquets and Bleeding Control

- **Shared**: `Content.Shared._Shitmed.Tourniquet` — `TourniquetComponent` definition.
- **Server**: `TourniquetSystem` (sealed, `Content.Server._Shitmed.Medical.Tourniquet`, 31 deps) — Handles applying, tightening, and removing tourniquets. Manages necrosis timer.

## 11. Part Status

- **Server**: `PartStatusSystem` (sealed, `Content.Server._Shitmed.PartStatus`, 32 deps) — Evaluates limb health thresholds, fractures, and amputations. The primary server-side part state machine.
- **Part Statuses**: `Intact`, `Fractured`, `ArterialBleed`, `Severed`.

## 12. Server-Side Medical Systems

- `GhettoSurgerySystem` (sealed partial, `Content.Server._Shitmed.Medical.Surgery`): Improvised surgery without proper tools.
- `TraumaBloodlossSystem` (sealed, `Content.Server._Shitmed.Medical.Trauma`): Blood loss processing from trauma.
- `TraumaCauseSystem` (sealed, `Content.Server._Shitmed.Medical.Trauma`): Trauma cause identification and logging.
- `BraindeathSystem` (sealed, `Content.Server._Shitmed.Medical.Trauma`): Brain death processing and consequences.

## 13. Miscellaneous

- `FumbleOnDamageSystem` (sealed, `Content.Shared._Shitmed.Weapons.Systems`): Drop held weapons when receiving damage to arms/hands. Used for nerve damage fumbling.
- `SharedItemSwitchSystem` (abstract, `Content.Shared._Shitmed.ItemSwitch`): Item mode switching.
- `SharedOnHitSystem` (abstract, `Content.Shared._Shitmed.OnHit`): On-hit effect processing for weapons.
- `SharedRestrictSystem` (sealed partial, `Content.Shared._Shitmed.Restrict`): Equipment restriction by species/body type.
- `BodyScannerSystem` (sealed partial, `Content.Server._Shitmed.Analyzer`): Body scanner diagnostic tool.
- `DelayedDeathSystem` (partial, `Content.Server._Shitmed.DelayedDeath`): Delayed death processing.
- `SharedAbductorSystem` (abstract, `Content.Shared._Shitmed.Antags.Abductor`): Abductor antagonist system.

## 14. Status Effect Systems (Server)

- `ScrambleDnaEffectSystem`: DNA scrambling effect.
- `ExpelGasEffectSystem`: Gas expulsion effect.
- `ScrambleLocationEffectSystem`: Location scrambling effect.
- `SpawnEntityEffectSystem`: Entity spawning effect.
- `ActivateArtifactEffectSystem`: Artifact activation effect.

## 15. Cybernetics and Prosthetics

- **Namespace**: `Content.Shared._Shitmed.Cybernetics`
- **Components**:
  - `CyberneticsComponent`: Applied to cybernetic limbs or prosthetic organs.
  - Grants damage type immunities (no biological bleeding, immune to suffocation), but introduces vulnerability to EMP and electrical overload.
