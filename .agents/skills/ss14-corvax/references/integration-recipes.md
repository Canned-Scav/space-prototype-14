# Corvax: Integration Recipes

Practical recipes for using Corvax features from `_ScavPrototype`.

## Recipe 1: Broadcasting a Faction Audio Announcement

Playing a station-wide siren and audio announcement for a Scavenger raid:

```csharp
using Content.Server._CorvaxGoob.Announcer;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;

namespace Content.Server._ScavPrototype.Events;

public sealed partial class ScavRaidRuleSystem : EntitySystem
{
    [Dependency] private readonly AnnouncerSystem _announcer = default!;

    public void TriggerScavengerRaidAlert()
    {
        var message = Loc.GetString("scav-raid-announcement-text");
        var title = Loc.GetString("scav-raid-announcement-title");

        // Uses Corvax AnnouncerSystem to broadcast audio with visual alert
        _announcer.SendAnnouncement(
            announcerId: "Automated",
            message: message,
            sender: title,
            colorOverride: Color.Red
        );
    }
}
```

## Recipe 2: Interacting with Criminal Records

Checking if an infiltrator or scavenger is marked as Wanted by security:

```csharp
using Content.Shared._CorvaxGoob.CriminalRecords;
using Robust.Shared.GameObjects;

namespace Content.Server._ScavPrototype.Security;

public sealed partial class ScavUndercoverSystem : EntitySystem
{
    [Dependency] private readonly CriminalRecordsSystem _records = default!;

    public bool IsTargetWanted(string characterName)
    {
        if (_records.TryGetRecord(characterName, out var record))
        {
            return record.Status == SecurityStatus.Wanted;
        }

        return false;
    }
}
```

## Recipe 3: Offering an Item via `OfferItemSystem`

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
