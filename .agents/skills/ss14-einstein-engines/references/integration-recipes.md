# EinsteinEngines: Integration Recipes

Practical recipes for using EinsteinEngines features from `_ScavPrototype`.

## Recipe 1: Defining a Custom Language

Creating a Scavenger dialect in `Resources/Prototypes/_ScavPrototype/Languages/scav_language.yml`:

```yaml
- type: language
  id: ScavengerJargon
  name: language-scavenger-jargon-name
  description: language-scavenger-jargon-desc
  speechVerb: Default
  icon:
    sprite: _ScavPrototype/Interface/Languages/scav_lang.rsi
    state: icon
  replacement:
    syllables:
    - krr
    - zik
    - tak
    - vosh
    - glik
    - skrav
    - brek
    - nal
```

> **Important**: Always provide at least 4-8 distinct syllables. Empty or too-short syllable lists cause null reference exceptions during chat obfuscation.

Attaching the language to a scavenger mob prototype (requires **both** `LanguageSpeaker` and `LanguageKnowledge`):

```yaml
- type: entity
  parent: BaseMobHumanoid
  id: MobScavengerVeteran
  name: scavenger veteran
  components:
  - type: LanguageSpeaker
    languages:
    - GalacticCommon
    - ScavengerJargon
    currentLanguage: ScavengerJargon
  - type: LanguageKnowledge
    speaks:
    - GalacticCommon
    - ScavengerJargon
    understands:
    - GalacticCommon
    - ScavengerJargon
```

> **Note**: `LanguageSpeaker` is for shared/client (UI, language menu). `LanguageKnowledge` is for server (authoritative obfuscation). Both are required on sentient speaking mobs.

## Recipe 2: Authoring a Scavenger IPC / Robot Mob

Creating a synthetic scavenger chassis powered by a battery with power drain:

```yaml
- type: entity
  parent: BaseMobSiliconHumanoid
  id: MobScavengerIPC
  name: repurposed industrial chassis
  description: A battered synthetic chassis scavenged from industrial wreckage.
  components:
  - type: Silicon
    entityType: Drone
  - type: SiliconDownOnDead
  - type: DeadStartupButton
  - type: BatteryDrinker
    drinkSpeed: 100
  - type: BatterySlotRequiresLock  # Always include — prevents trivial power cell theft
  - type: ItemSlots
    slots:
      battery_slot:
        name: Power Cell
        startingItem: PowerCellHigh
        whitelist:
          tags:
          - HighPowerCell
```

> **Important**: Always add `BatterySlotRequiresLock`. Without it, any adjacent player can freely yank the power cell mid-combat.

## Recipe 3: Creating a YAML Interaction Verb

Defining a "field triage" verb that patches up wounded targets without writing new C# code:

```yaml
- type: interactionVerb
  id: ScavFieldPatchVerb
  verb:
    text: scav-verb-field-patch
    icon:
      sprite: _ScavPrototype/Interface/Verbs/patch.rsi
      state: patch
    category: VerbCategoryMedical
  requirements:
  - !type:HandsRequirement
    freeHands: 1
  - !type:ToolRequirement
    qualities:
    - Rolling
  doAfter:
    delay: 3.0
    breakOnDamage: true
    breakOnMove: true
  actions:
  - !type:ModifyHealthAction
    damage:
      types:
        Slash: -10
        Blunt: -10
  - !type:ChatMessageAction
    message: scav-popup-patched-up
    type: Emote
```

## Recipe 4: Complex Verb with Conditional Actions

Using `ConditionalAction` and `ComplexAction` for a verb that behaves differently based on target state:

```yaml
- type: interactionVerb
  id: ScavInspectVerb
  verb:
    text: scav-verb-inspect-target
    icon:
      sprite: _ScavPrototype/Interface/Verbs/inspect.rsi
      state: inspect
    category: VerbCategoryExamine
  requirements:
  - !type:DistanceRequirement
    maxDistance: 2.0
  actions:
  - !type:ConditionalAction
    condition: !type:ConsciousRequirement {}
    ifTrue:
      - !type:ChatMessageAction
        message: scav-popup-target-conscious
        type: Popup
    ifFalse:
      - !type:ComplexAction
        actions:
        - !type:ChatMessageAction
          message: scav-popup-target-unconscious
          type: Popup
        - !type:ModifyHealthAction
          damage:
            types:
              Blunt: -5
```

## Recipe 5: Using ContestsSystem for Strength-Based Interactions

Leveraging contests for a salvage-specific strength check:

```csharp
using Content.Shared._EinsteinEngines.Contests;
using Robust.Shared.GameObjects;

namespace Content.Shared._ScavPrototype.Salvage;

public sealed partial class ScavHeavyLiftSystem : EntitySystem
{
    [Dependency] private readonly ContestsSystem _contests = default!;

    public bool CanLiftHeavyDebris(EntityUid lifter, EntityUid debris)
    {
        // Use EinsteinEngines contest system for mass-based strength comparison
        var massContest = _contests.MassContest(lifter, debris);
        return massContest >= 0.8f; // Lifter must be at least 80% of debris mass
    }
}
```

## Recipe 6: Using OnUserAction in a Verb

Creating a verb where the action affects the **user** performing the verb, not the target:

```yaml
- type: interactionVerb
  id: ScavSalvageBreathVerb
  verb:
    text: scav-verb-take-breath
    category: VerbCategorySelf
  requirements:
  - !type:DistanceRequirement
    maxDistance: 1.5
  doAfter:
    delay: 2.0
    breakOnMove: true
  actions:
  - !type:OnUserAction
    action:
      !type:ModifyStatusEffectAction
      effect: Stun
      duration: 0  # Remove stun from the user
      remove: true
  - !type:ChatMessageAction
    message: scav-popup-caught-breath
    type: Emote
```
