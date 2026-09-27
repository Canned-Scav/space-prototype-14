# Shitmed: Integration Recipes

Practical recipes for interacting with Shitmed from `_ScavPrototype`.

## Recipe 1: Configuring Armor Body Part Protection

Authoring a reinforced scavenger chestplate in `Resources/Prototypes/_ScavPrototype/Entities/Clothing/armor.yml` that correctly covers Shitmed target zones:

```yaml
- type: entity
  parent: ClothingOuterBase
  id: ScavClothingOuterArmorChestplate
  name: scavenger scrap chestplate
  description: Crude steel plates riveted together to protect vital organs.
  components:
  - type: Armor
    modifiers:
      coefficients:
        Blunt: 0.6
        Slash: 0.5
        Piercing: 0.5
        Heat: 0.8
  - type: ProtectedBodyParts
    parts:
    - Torso
    - Groin
```

## Recipe 2: Authoring a Weapon with High Fracture Chance

Configuring a scavenger sledgehammer that inflicts blunt fractures on limbs:

```yaml
- type: entity
  parent: BaseItem
  id: ScavWeaponSledgehammer
  name: heavy scrap sledgehammer
  description: A weighted industrial hammer capable of shattering bones.
  components:
  - type: MeleeWeapon
    attackRate: 0.8
    damage:
      types:
        Blunt: 35
        Structural: 40
  - type: BoneFractureOnHit
    chance: 0.45
```

## Recipe 3: Implementing an Improvised Scavenger Tourniquet

Defining an improvised rag tourniquet in `_ScavPrototype`:

```yaml
- type: entity
  parent: BaseItem
  id: ScavItemRagTourniquet
  name: improvised tourniquet
  description: A strip of sturdy cloth used to compress an arterial wound.
  components:
  - type: Tourniquet
    applicationTime: 4.0
    necrosisTime: 180.0
```
