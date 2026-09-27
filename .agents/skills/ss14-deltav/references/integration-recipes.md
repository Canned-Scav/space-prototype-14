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

## Recipe 2: Interacting with the Carrying System

Writing a system in `_ScavPrototype` that speeds up carrying wounded allies:

```csharp
using Content.Shared._DV.Carrying;
using Robust.Shared.GameObjects;

namespace Content.Shared._ScavPrototype.Rescue;

public sealed partial class ScavRescueSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<ScavExosuitComponent, CarryAttemptEvent>(OnCarryAttempt);
    }

    private void OnCarryAttempt(EntityUid uid, ScavExosuitComponent component, ref CarryAttemptEvent args)
    {
        // Exosuit servos negate movement penalties while carrying wounded
        if (component.Powered)
        {
            args.SpeedPenaltyModifier = 0.0f;
        }
    }
}
```

## Recipe 3: Creating a Scavenger PDA with NanoChat

Authoring a customized Scavenger PDA with preloaded messaging:

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
