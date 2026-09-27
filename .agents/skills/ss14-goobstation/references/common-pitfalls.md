# Goob Station: Common Pitfalls and Anti-Patterns

This document records architectural mistakes, prediction traps, and file placement errors when working with Goob Station code.

## 1. File Placement into Upstream Assemblies
- **Anti-pattern**: Adding new C# files directly to `Content.Goobstation.Shared`, `Content.Goobstation.Server`, or `_Goobstation`.
- **Consequence**: Guaranteed merge conflicts when rebasing against Goob Station upstream.
- **Rule**: All new files, components, and prototypes belonging to space-prototype-14 must go into `_ScavPrototype`.

## 2. Directly Calling Server Antagonist Methods from Client
- **Anti-pattern**: Attempting to resolve `ChangelingSystem` or `HereticSystem` in shared code without checking for server-only context.
- **Consequence**: Client compilation errors or `NullReferenceException` at runtime during client startup.
- **Rule**: Use the `Shared*System` variant (`SharedChangelingSystem`, `SharedHereticSystem`) in shared code, or inject the server system exclusively inside `Content.Server/_ScavPrototype`.

## 3. Incorrect RichText Tag Escaping in UIKit
- **Anti-pattern**: Writing unescaped brackets or invalid prototype IDs in `[icon prototype="..."]`.
- **Consequence**: RichText parser crashes or renders broken placeholder text in chat and popups.
- **Rule**: Always verify that the prototype ID exists before injecting it into a dynamic format string.

## 4. Bypassing Shitmed When Calibrating Weapons
- **Anti-pattern**: Configuring Goob Station weapons assuming simple total entity HP damage.
- **Consequence**: Weapons feel ineffective or deal no damage because Goob Station routes damage through `_Shitmed` targeted body parts.
- **Rule**: Always review `ss14-shitmed` when balancing or creating weapons, projectiles, or damage containers.
