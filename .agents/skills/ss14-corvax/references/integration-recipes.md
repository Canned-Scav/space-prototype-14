# Corvax: Integration Recipes

Practical recipes for using Corvax features from `_ScavPrototype`.

## Recipe 1: Adding TTS Voice to a Custom Entity

Attaching a TTS voice to a scavenger NPC in `Resources/Prototypes/_ScavPrototype/Entities/Mobs/scav_npc.yml`:

```yaml
- type: entity
  parent: BaseMobHumanoid
  id: MobScavengerGuard
  name: scavenger guard
  description: A scarred veteran who communicates in gruff radio chatter.
  components:
  - type: TTS
    voice: male_1  # Must match a valid TTSVoicePrototype ID
```

## Recipe 2: Forcing a Specific Announcer for a Custom Game Rule

Using `AnnouncerSystem.ForceSetAnnouncer(...)` to set a raid-specific announcer voice pack for 3 rounds:

```csharp
using Content.Server._CorvaxGoob.Announcer;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;

namespace Content.Server._ScavPrototype.GameRules;

public sealed partial class ScavRaidRuleSystem : EntitySystem
{
    [Dependency] private readonly AnnouncerSystem _announcer = default!;
    [Dependency] private readonly IPrototypeManager _prototype = default!;

    public void ActivateRaidAnnouncer()
    {
        // Force a specific announcer voice pack for 3 rounds
        if (_prototype.TryIndex<AnnouncerPrototype>("RaidAnnouncer", out var proto))
        {
            _announcer.ForceSetAnnouncer(proto, rounds: 3);
        }
    }
}
```

> **Note**: To broadcast station-wide messages, use vanilla `ChatSystem.DispatchStationAnnouncement(...)`, not Corvax `AnnouncerSystem`.

## Recipe 3: Broadcasting a Station Announcement (Vanilla API)

The correct way to send a station-wide alert for a Scavenger event:

```csharp
using Content.Server.Chat.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;

namespace Content.Server._ScavPrototype.Events;

public sealed partial class ScavRaidAlertSystem : EntitySystem
{
    [Dependency] private readonly ChatSystem _chat = default!;

    public void TriggerScavengerRaidAlert(EntityUid? station)
    {
        var message = Loc.GetString("scav-raid-announcement-text");

        // Uses vanilla ChatSystem for station-wide broadcast
        _chat.DispatchStationAnnouncement(
            station ?? EntityUid.Invalid,
            message,
            Loc.GetString("scav-raid-announcement-sender"),
            colorOverride: Color.Red
        );
    }
}
```

## Recipe 4: Offering an Item via `SharedOfferItemSystem`

Triggering an item offer interaction when bartering with a player:

```csharp
using Content.Shared._CorvaxGoob.OfferItem;
using Robust.Shared.GameObjects;

namespace Content.Shared._ScavPrototype.Barter;

public sealed partial class ScavBarterSystem : EntitySystem
{
    [Dependency] private readonly SharedOfferItemSystem _offerItem = default!;

    public void OfferScavLoot(EntityUid user, EntityUid target, EntityUid item)
    {
        _offerItem.TryOfferItem(user, target, item);
    }
}
```

## Recipe 5: Checking Criminal Record via Examine Extension

Reading the criminal record examine text. Note that `CriminalRecordExamineSystem` is server-only and only extends examine, not a full records API. For actual criminal status, use the vanilla `CriminalRecordsSystem`:

```csharp
using Content.Server.CriminalRecords.Systems;
using Content.Shared.CriminalRecords;
using Robust.Shared.GameObjects;

namespace Content.Server._ScavPrototype.Security;

public sealed partial class ScavUndercoverSystem : EntitySystem
{
    [Dependency] private readonly CriminalRecordsSystem _records = default!;

    public bool IsTargetWanted(EntityUid station, string characterName)
    {
        // Use vanilla CriminalRecordsSystem, not CorvaxGoob's examine-only extension
        if (_records.TryGetRecord(station, characterName, out _, out var record))
        {
            return record.Status == SecurityStatus.Wanted;
        }

        return false;
    }
}
```
