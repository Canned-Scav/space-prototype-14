# Goob Station: Common Pitfalls and Anti-Patterns

This document records architectural mistakes, prediction traps, and file placement errors when working with Goob Station code.

## 1. File Placement into Upstream Assemblies

- **Anti-pattern**: Adding new C# files directly to `Content.Goobstation.Shared`, `Content.Goobstation.Server`, or `_Goobstation`.
- **Consequence**: Guaranteed merge conflicts when rebasing against Goob Station upstream.
- **Rule**: All new files, components, and prototypes belonging to space-prototype-14 must go into `_ScavPrototype`.

## 2. Directly Calling Server Antagonist Methods from Shared Code

- **Anti-pattern**: Attempting to resolve `ChangelingSystem`, `HereticSystem`, or `BlobRuleSystem` in shared code without checking for server-only context.
- **Consequence**: Client compilation errors or `NullReferenceException` at runtime during client startup.
- **Rule**: Use the `Shared*System` variant (`SharedChangelingSystem`, `SharedHereticSystem`, `SharedBlobSystem`) in shared code. Inject the server-only system exclusively inside `Content.Server/_ScavPrototype`.

## 3. Incorrect RichText Tag Escaping in UIKit

- **Anti-pattern**: Writing unescaped brackets or invalid prototype IDs in `[icon prototype="..."]`.
- **Consequence**: RichText parser crashes or renders broken placeholder text in chat and popups.
- **Rule**: Always verify that the prototype ID exists before injecting it into a dynamic format string. Escape special characters properly.

## 4. Bypassing Shitmed When Calibrating Weapons

- **Anti-pattern**: Configuring Goob Station weapons assuming simple total entity HP damage.
- **Consequence**: Weapons feel ineffective or deal no damage because Goob Station routes damage through `_Shitmed` targeted body parts via `WoundSystem`.
- **Rule**: Always review `ss14-shitmed` when balancing or creating weapons, projectiles, or damage containers. Use `FixedPoint2` for damage values.

## 5. Using Float/Int Instead of FixedPoint2 for Damage

- **Anti-pattern**: Using `float` or `int` for wound severity, pain thresholds, or damage amounts when interacting with Shitmed APIs.
- **Consequence**: Type mismatch compilation errors. All Shitmed wound and trauma math uses `FixedPoint2` from `Content.Goobstation.Maths.FixedPoint`.
- **Rule**: Import `Content.Goobstation.Maths.FixedPoint` and use `FixedPoint2.New(value)` for all health-related numeric values.

## 6. Ignoring the Scale of `Content.Goobstation.Shared`

- **Anti-pattern**: Assuming Goob Station only has Changeling, Heretic, and Blob antagonists.
- **Consequence**: Missing 10+ other antagonist systems (Devil, DarkLord, Shadowling, Wraith, Nightmare, Slasher, ChronoLegionnaire, Xenomorph, Hastur, Pirates, SlaughterDemon) that may interact with your code through events or shared components.
- **Rule**: Before implementing features that interact with antagonists, combat, or special abilities, check `Content.Goobstation.Shared` (177+ directories) for existing systems that might already handle the use case or raise conflicting events.

## 7. Forgetting to Check `Content.Goobstation.Common` for Traits

- **Anti-pattern**: Creating new trait-like components without checking `Content.Goobstation.Common.Traits`.
- **Consequence**: Duplicating existing trait infrastructure. DeltaV's `CarryingSystem` already references `Content.Goobstation.Common.Traits` for carry-related traits.
- **Rule**: Check `Content.Goobstation.Common.Traits` before implementing character traits or passive abilities.

## 8. Duplicate MartialArts or Grab Logic

- **Anti-pattern**: Implementing custom melee combo systems without checking `Content.Goobstation.Shared.MartialArts` and `Content.Goobstation.Shared.Grab`.
- **Consequence**: Conflicting grab intent handling, broken combo chains, or desync between custom and Goob grab systems.
- **Rule**: Extend or hook into existing `MartialArts` and `Grab` systems rather than building parallel melee combat logic.
