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

> **Important**: Every piece of combat clothing MUST explicitly list its protected body parts via `ProtectedBodyParts`. Without it, the armor provides no protection because incoming damage is routed to specific body parts through `WoundSystem`.

## Recipe 2: Authoring a Weapon with Trauma Interactions

Configuring a scavenger sledgehammer that inflicts blunt fractures and interacts with the trauma system:

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

### Damage Calibration Guidelines

Damage is applied to **individual limb health pools** (roughly 40-60 HP each), not total entity HP. Use these ranges:

| Weapon Category | Damage per Hit | Rationale |
| --- | --- | --- |
| Rapid-fire (pistol, SMG) | 8-15 | Multiple hits per second; higher values cause instant dismemberment |
| Standard melee | 15-25 | Moderate swing rate |
| Heavy melee / rifle | 25-35 | Slow attacks, significant per-hit impact |
| Sniper / heavy weapons | 40-55 | One-shot limb destruction at high end |

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

> **Warning**: Tourniquets have a necrosis timer. After `necrosisTime` seconds of continuous application, the limb begins necrotizing. Tourniquets are emergency stabilization only.

## Recipe 4: Creating a Full Armor Set with Complete Coverage

A complete scavenger armor set covering all major body zones:

```yaml
# Helmet — covers Head
- type: entity
  parent: ClothingHeadBase
  id: ScavClothingHeadHelmet
  name: scavenger welding helmet
  components:
  - type: Armor
    modifiers:
      coefficients:
        Blunt: 0.7
        Slash: 0.6
        Piercing: 0.7
  - type: ProtectedBodyParts
    parts:
    - Head

# Chestplate — covers Torso + Groin
- type: entity
  parent: ClothingOuterBase
  id: ScavClothingOuterChestplate
  name: scavenger chestplate
  components:
  - type: Armor
    modifiers:
      coefficients:
        Blunt: 0.5
        Slash: 0.4
        Piercing: 0.5
  - type: ProtectedBodyParts
    parts:
    - Torso
    - Groin

# Gauntlets — covers Arms + Hands
- type: entity
  parent: ClothingHandsBase
  id: ScavClothingHandsGauntlets
  name: scavenger gauntlets
  components:
  - type: Armor
    modifiers:
      coefficients:
        Blunt: 0.7
        Slash: 0.6
  - type: ProtectedBodyParts
    parts:
    - LeftArm
    - RightArm
    - LeftHand
    - RightHand

# Greaves — covers Legs + Feet
- type: entity
  parent: ClothingShoesBase
  id: ScavClothingShoesGreaves
  name: scavenger greaves
  components:
  - type: Armor
    modifiers:
      coefficients:
        Blunt: 0.7
        Slash: 0.6
  - type: ProtectedBodyParts
    parts:
    - LeftLeg
    - RightLeg
    - LeftFoot
    - RightFoot
```

> **Note**: A "full armor" set requires helmet + chestplate + gauntlets + greaves to cover all 11 target zones. Missing any piece leaves those body parts completely unarmored.

## Recipe 5: Interacting with the Wound System from C#

Reading wound state on a body part from `_ScavPrototype`:

```csharp
using Content.Shared._Shitmed.Medical.Surgery.Wounds.Systems;
using Content.Shared._Shitmed.Medical.Surgery.Wounds.Components;
using Content.Shared.Body.Part;
using Content.Shared.Body.Systems;
using Content.Goobstation.Maths.FixedPoint;
using Robust.Shared.GameObjects;

namespace Content.Shared._ScavPrototype.Medical;

public sealed partial class ScavWoundAssessmentSystem : EntitySystem
{
    [Dependency] private readonly SharedBodySystem _body = default!;

    /// <summary>
    /// Checks if any body part on the target has wounds exceeding the severity threshold.
    /// </summary>
    public bool HasSevereWounds(EntityUid target, FixedPoint2 severityThreshold)
    {
        // Iterate through body parts and check wound components
        foreach (var part in _body.GetBodyChildren(target))
        {
            if (TryComp<WoundableComponent>(part.Id, out var woundable))
            {
                if (woundable.TotalWoundSeverity >= severityThreshold)
                    return true;
            }
        }

        return false;
    }
}
```

> **Important**: All wound severity values use `FixedPoint2` from `Content.Goobstation.Maths.FixedPoint`, not `float` or `int`.
