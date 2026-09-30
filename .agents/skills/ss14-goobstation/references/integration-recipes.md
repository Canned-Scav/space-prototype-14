# Goob Station: Integration Recipes

Practical recipes for interacting with Goob Station systems from `_ScavPrototype`.

## Recipe 1: Subscribing to Changeling Events

Protecting a Scavenger role or equipment piece from Changeling sting attacks:

```csharp
using Content.Goobstation.Shared.Changeling;
using Robust.Shared.GameObjects;

namespace Content.Shared._ScavPrototype.Protection;

public sealed partial class ScavAntiChangelingSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<ScavBioArmorComponent, ChangelingStingAttemptEvent>(OnStingAttempt);
    }

    private void OnStingAttempt(EntityUid uid, ScavBioArmorComponent component, ref ChangelingStingAttemptEvent args)
    {
        // Cancel the sting if the target is wearing Scav bio-armor
        if (component.Enabled)
        {
            args.Cancelled = true;
        }
    }
}
```

## Recipe 2: Adding a Custom Augment

Creating a Scavenger cybernetic implant in `_ScavPrototype`:

1. Define the component in C# (`Content.Shared/_ScavPrototype/Augments/ScavReflexAugmentComponent.cs`):
```csharp
using Robust.Shared.GameObjects;

namespace Content.Shared._ScavPrototype.Augments;

[RegisterComponent]
public sealed partial class ScavReflexAugmentComponent : Component
{
    [DataField("speedMultiplier")]
    public float SpeedMultiplier = 1.15f;
}
```

2. Register the prototype in YAML (`Resources/Prototypes/_ScavPrototype/Entities/augments.yml`):
```yaml
- type: entity
  parent: BaseAugment
  id: ScavAugmentReflex
  name: scavenger reflex booster
  description: An illicit overclocked neural implant that sharpens reflexes and increases sprint speed.
  components:
  - type: Augment
    slot: Head
  - type: ScavReflexAugment
    speedMultiplier: 1.2
```

## Recipe 3: Using UIKit RichText in Popups or Chat

Leveraging Goobstation's rich text tags to show inline icons in user feedback:

```csharp
using Content.Shared.Popups;
using Robust.Shared.Localization;
using Robust.Shared.GameObjects;

namespace Content.Shared._ScavPrototype.Scavenging;

public sealed partial class ScavLootSystem : EntitySystem
{
    [Dependency] private readonly SharedPopupSystem _popup = default!;

    public void NotifyScavenger(EntityUid user, string itemPrototypeId)
    {
        // Embeds an inline entity icon using Goobstation UIKit RichText
        var message = Loc.GetString("scav-loot-found", ("itemIcon", $"[icon prototype=\"{itemPrototypeId}\"]"));
        _popup.PopupEntity(message, user, user);
    }
}
```

> **Important**: Always verify that the prototype ID exists before injecting into a dynamic format string. Invalid IDs crash the RichText parser.

## Recipe 4: Inheriting from a Goob Station Prototype

Overriding a Goob Station weapon in `_ScavPrototype`:

```yaml
- type: entity
  parent: WeaponBoomerangSyndicate
  id: ScavWeaponBoomerangHeavy
  name: reinforced scavenger boomerang
  description: A weighted scrap boomerang fitted with jagged blades.
  components:
  - type: Sprite
    sprite: _ScavPrototype/Objects/Weapons/Guns/heavy_boomerang.rsi
    state: icon
  - type: DamageOtherOnHit
    damage:
      types:
        Slash: 25
        Structural: 15
```

## Recipe 5: Resolving a Server-Only Goob System Safely

When you need `ChangelingSystem` (server-only) from `_ScavPrototype` server code:

```csharp
using Content.Goobstation.Server.Changeling;
using Robust.Shared.GameObjects;

namespace Content.Server._ScavPrototype.Antag;

public sealed partial class ScavChangelingInteractionSystem : EntitySystem
{
    // Server-only system — only resolve in Content.Server/_ScavPrototype
    [Dependency] private readonly ChangelingSystem _changeling = default!;

    // ... server-only logic
}
```

For shared code, always use the `Shared*System` variant:

```csharp
using Content.Goobstation.Shared.Changeling;
using Robust.Shared.GameObjects;

namespace Content.Shared._ScavPrototype.Antag;

public sealed partial class ScavChangelingSharedSystem : EntitySystem
{
    // Shared variant — safe in both server and client assemblies
    [Dependency] private readonly SharedChangelingSystem _changeling = default!;

    // ... shared logic
}
```

## Recipe 6: Using FixedPoint for Damage Values

When interacting with Shitmed wound/damage values through Goob Station systems:

```csharp
using Content.Goobstation.Maths.FixedPoint;

// Correct — use FixedPoint2 for wound severity values
FixedPoint2 woundSeverity = FixedPoint2.New(15);
FixedPoint2 totalDamage = woundSeverity + FixedPoint2.New(10);

// Wrong — do NOT use raw float/int for Shitmed damage math
// float damage = 15.0f;  // Will cause type mismatch errors
```
