# DeltaV: Integration Recipes

Practical recipes for interacting with DeltaV features from `_ScavPrototype`.

## Recipe 1: Enabling Mouth Contraband Storage on a Scavenger Mob

Adding covert mouth storage for contraband in `Resources/Prototypes/_ScavPrototype/Entities/Mobs/scav_smuggler.yml`:

```yaml
- type: entity
  parent: BaseMobHumanoid
  id: MobScavengerSmuggler
  name: scavenger smuggler
  description: A seasoned salvager known for sneaking valuable trinkets past station customs.
  components:
  - type: MouthStorage
    maxItemSize: Tiny
```

> **Note**: Only `Tiny`-sized items can be stored. The `MouthStorage` component lives in `Content.Shared._DV.Storage.Components`.

## Recipe 2: Reacting to Carry Events

Writing a system in `_ScavPrototype` that modifies carrying behavior for entities with an exosuit:

```csharp
using Content.Shared._DV.Carrying;
using Content.Shared.Movement.Systems;
using Robust.Shared.GameObjects;

namespace Content.Shared._ScavPrototype.Rescue;

/// <summary>
/// Reduces movement penalties when carrying wounded allies while wearing a Scav exosuit.
/// </summary>
public sealed partial class ScavRescueSystem : EntitySystem
{
    [Dependency] private readonly MovementSpeedModifierSystem _movement = default!;

    public override void Initialize()
    {
        base.Initialize();

        // React to an entity starting to carry someone
        SubscribeLocalEvent<ScavExosuitComponent, CarryingSlowdownComponent.RefreshMovementSpeedModifiersEvent>(OnRefreshSpeed);
    }

    private void OnRefreshSpeed(EntityUid uid, ScavExosuitComponent component,
        CarryingSlowdownComponent.RefreshMovementSpeedModifiersEvent args)
    {
        if (component.Powered)
        {
            // Exosuit servos negate 50% of carrying slowdown
            args.ModifySpeed(1.0f, 1.0f);
        }
    }
}
```

> **Important**: Always drop carried entities via `CarryingSystem` before teleports or grid changes. Directly changing parent transforms causes desync.

## Recipe 3: Creating a Scavenger PDA with NanoChat

Authoring a customized Scavenger PDA with preloaded messaging in `Resources/Prototypes/_ScavPrototype/Entities/Objects/scav_pda.yml`:

```yaml
- type: entity
  parent: BasePDA
  id: ScavPDA
  name: scavenger data pad
  description: A battered PDA running unauthorized communication software.
  components:
  - type: CartridgeLoader
    installedCartridges:
    - NanoChatCartridge
  - type: NanoChatCard
```

> **Note**: Both `CartridgeLoader` and `NanoChatCard` are required. Without `CartridgeLoader`, the NanoChat UI cannot be opened.

## Recipe 4: Adding CrawlUnderObjects to a Custom Mob

Making a small scavenger drone that can crawl under tables and conveyors:

```yaml
- type: entity
  parent: BaseMobNonHuman
  id: MobScavengerDrone
  name: scavenger reconnaissance drone
  description: A low-profile drone designed to slip under obstacles during salvage operations.
  components:
  - type: CrawlUnderObjects
```

## Recipe 5: Using Cosmic Cult Prototypes for Custom Content

Inheriting from DeltaV Cosmic Cult items for a scavenger artifact:

```yaml
- type: entity
  parent: BaseCosmicCultItem  # Inherits from DeltaV cosmic cult base
  id: ScavCosmicFragment
  name: scavenged cosmic fragment
  description: A shard of alien origin pulsing with cosmic energy.
  components:
  - type: Sprite
    sprite: _ScavPrototype/Objects/Misc/cosmic_fragment.rsi
    state: icon
```

> **Note**: Cosmic Cult prototypes live in `Resources/Prototypes/_DV`. Always check existing prototypes before creating new ones.
