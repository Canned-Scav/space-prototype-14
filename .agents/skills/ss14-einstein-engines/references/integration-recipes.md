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
```

Attaching the language to a scavenger mob prototype:

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
  - type: BatterySlotRequiresLock
  - type: ItemSlots
    slots:
      battery_slot:
        name: Power Cell
        startingItem: PowerCellHigh
        whitelist:
          tags:
          - HighPowerCell
```

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
