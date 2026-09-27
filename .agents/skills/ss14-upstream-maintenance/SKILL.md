---
name: ss14-upstream-maintenance
description: Guide to working with Space Station 14 forks with project-folder pattern (`_ScavPrototype`) to minimize merge conflicts with the upstream. Use when modifying vanilla code or prototypes.
---

# Working with Upstream code and minimizing conflicts

This skill describes the standards and patterns adopted in Space Station 14 forks with project-folder isolation (`_ScavPrototype`) for working with code inherited from the upstream.
**Main goal:** Maintain the ability to easily receive updates from the upstream (merge), minimizing manual edits in case of conflicts.

Before making changes, first determine the active codebase prefix, project folder and edit markers using the `ss14-codebase-prefix-detection` rule.

After the active fork is determined, keep all new fork-owned code in that fork's project folder (`_ScavPrototype`). Do not switch folders because the touched behavior is vanilla, inherited from another fork (such as GoobStation, DeltaV, Corvax), or looks "not fork-specific". For space-prototype-14, the correct folder is `_ScavPrototype`; new code, prototypes and assets go there even when the hook or parent prototype lives in vanilla or in an upstream folder.

##  Golden rule

> [!IMPORTANT]
> **Minimizing changes to vanilla and upstream files is MORE IMPORTANT than "pretty" architecture.**
> It's better to leave the "dirty" hack in one line of the vanilla file than to rewrite half the system, creating hell when merging.

## Folder structure and Project Folder

To clearly separate upstream/vanilla code from our modifications, a special project folder is used: `_ScavPrototype`.

**Why `_`?**
- The folder is always at the top of the file list and is easy to find.
- Visually separates “our” code from “their” (upstream/vanilla) code.

**What should I put in `_ScavPrototype`?**
1. **New files:** Completely new systems, components, prototypes, and localization.
2. **Partial classes:** Extensions of vanilla and upstream classes (see below).
3. **Assets:** New sprites, sounds, textures.

The selected project folder is `_ScavPrototype`. Do not move new files to `Content.Goobstation.*`, `_Goobstation`, `_DV`, `_Corvax`, or vanilla just because the original code came from GoobStation/upstream, or the change connects to it.

> [!TIP]
> **Isolation principle:**
> Try to keep 99% of your unique code inside the `_ScavPrototype` folder.
> Only **minimal** edits (hooks, events) that connect vanilla or upstream code to yours should remain in external folders.

## Upstream Ecosystem and Specialized Skills

This repository inherits multiple layered upstreams. Refer to dedicated skills for deep dives:

| Upstream Base | Location in Repo | Dedicated Skill | Key Focus |
| --- | --- | --- | --- |
| **Goob Station** | `Content.Goobstation.*`, `_Goobstation` | `ss14-goobstation` | Primary upstream: Changeling, Heretic, Blob, Augments, UIKit |
| **Einstein Engines** | `_EinsteinEngines` | `ss14-einstein-engines` | Languages, Silicon/IPC, InteractionVerbs, Flight, Contests |
| **Corvax Ecosystem** | `_CorvaxGoob`, `_Corvax`, `_CorvaxNext`, `Corvax/` | `ss14-corvax` | Announcer, CriminalRecords, Footprints, Russian grammar, Remote Silicons |
| **Shitmed** | `_Shitmed` | `ss14-shitmed` | Targeted wounds, surgery, Autodoc, organs, cybernetics, bleeding |
| **DeltaV** | `_DV` | `ss14-deltav` | Harpy, Feroxi, NanoChat, Carrying, CosmicCult |

## Modification of C# code

When modifying existing vanilla or upstream code (outside the project folder), use the following patterns.

### 1. Pattern `Edit Start` / `Edit End`

Used when you need to change existing logic inside a method or property.
Makes it easy to see your changes against the background of vanilla/upstream code.

**Format:**
```csharp
// scav-edit start - brief reason for changes
...your code..
// scav-edit end
```

**Example (change value):**
```csharp
component.Field2 = 321;
// scav-edit start - increasing radius for balance
component.Field = 123;
// scav-edit end
```

**Example (changing logic):**
```csharp
// scav-edit start - fixing double gateways
if (TryComp<AirlockComponent>(uid, out var airlock))
{
    // ...new logic..
}
// scav-edit end
```

### 2. Pattern `Added Start` / `Added End`

Used when you add a **new** block of code (eg calling an event, checking) that was not in the original.

**Format:**
```csharp
// scav added start - brief reason for adding
...new code..
// scav added end
```

**Example:**
```csharp
// scav added start - to know when entity was hit
_eventBus.RaiseLocalEvent(uid, new ProjectileHitEvent(projectile, entity));
// scav added end
```

### 3. Partial Classes

If you need to add a **new field, property or method** to an existing class or system, **DO NOT** write it directly in a vanilla or upstream file (such as `Content.Server` or `Content.Goobstation.Server`).
Instead, create a `partial` class in your project folder (`_ScavPrototype`).

**Pattern:**
1. Find the target class (eg `SharedAirlockSystem`).
2. Create a file in your folder: `Content.Shared/_ScavPrototype/Doors/Systems/SharedAirlockSystem.Custom.cs`.
3. Declare the class as `partial` with the same namespace.
4. **Important:** Suppress the namespace mismatch warning if necessary.

**Example:**
*Original file (`Content.Shared/Doors/Systems/SharedAirlockSystem.cs`):*
```csharp
namespace Content.Shared.Doors.Systems;

public abstract partial class SharedAirlockSystem : EntitySystem
{
    // Original code..
}
```

*Your file (`Content.Shared/_ScavPrototype/Doors/Systems/SharedAirlockSystem.Custom.cs`):*
```csharp
using Content.Shared.Doors.Systems; // We use target namespace

// We suppress warning, since the file is physically located in another folder
#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Content.Shared.Doors.Systems;

public abstract partial class SharedAirlockSystem
{
    // Your new logic, accessible "as if" inside the original class
    public void MyNewMethod() { ... }
}
```

## Modifying Prototypes (YAML)

Changing vanilla or upstream YAML files (`Resources/Prototypes/Entities/...`, `Resources/Prototypes/_Goobstation/...`, `Resources/Prototypes/_DV/...`, etc.) is **BAD PRACTICE**. This is a guaranteed conflict with any change to this file in the upstream.

### Ideal Prototype Change Pattern

Instead of editing the original, we create a **replacement heir**.

**Algorithm:**
1. Find the entity ID (for example, `AirlockHatchSyndicate`).
2. Create a **new** YAML file in your project folder (`Resources/Prototypes/_ScavPrototype/.../access.yml`).
3. Create a new entity:
    - `id`: Add a suffix or prefix (for example, `AirlockHatchSyndicateLocked`).
    - `parent`: Specify an upstream/vanilla ID.
    - Make the necessary changes (add components, change fields).
4. **Migration Magic:** Register the replacement in `Resources/migration.yml`.

**Implementation example:**

*1. New prototype (`Resources/Prototypes/_ScavPrototype/Entities/Structures/Doors/Airlocks/access.yml`):*
```yaml
- type: entity
  parent: AirlockHatchSyndicate  # Inherit from the original
  id: AirlockHatchSyndicateLocked # New ID
  suffix: Syndicate, Locked
  categories: [ HideSpawnMenu ] # scav added - hide from spawn if this is a technical entity
  components:
  - type: AccessReader
    access: [["SyndicateAgent"]] # Add the required changes
```

*2. Migration file (`Resources/migration.yml`):*
Add an entry to the end of the file or to the appropriate section.
```yaml
# ... existing migrations ..

# scav-edit
AirlockHatchSyndicate: AirlockHatchSyndicateLocked
```

**Result:**
When loading the map, the engine will automatically replace all `AirlockHatchSyndicate` with `AirlockHatchSyndicateLocked`.
When updating the upstream, if new components are added to `AirlockHatchSyndicate`, your `AirlockHatchSyndicateLocked` will automatically receive them through inheritance (`parent`). File conflicts - **0**.

> [!WARNING]
> **Migration DOES NOT update links in other prototypes!**
> The `migration.yml` file tells the engine to replace the entity ONLY when spawning on the map and in saves.
> If the old ID (`AirlockHatchSyndicate`) is used in:
> - Spawn Pools
> - Crafting recipes
> - Fields of other components (for example, `SpawnOnDeath`)
>
> ...the old essence will remain there! You need to find all uses of the old ID and replace them with the new one manually (via the `edit` pattern or overriding).

### Minor changes in vanilla / upstream files

If migration is not possible (but try to use it!), use `edit` comments directly in YAML, but try to do it on one line.

```yaml
- type: entity
  id: VanillaEntity
  components:
  - type: Item
    path: _ScavPrototype/Objects/123.rsi # scav-edit - sprite replacement
```

### Anti-patterns (What NOT to do)

 **Direct code removal.**
Instead of deleting, comment out the code and leave the mark `edit`.
```csharp
// BAD:
// public void DeletedMethod() { }

// GOOD:
// scav-edit start - deleted because interferes with mechanics X
// public void DeletedMethod() { ... }
// scav-edit end
```

 **Rewriting entire files.**
If you copy the entire file into your folder and disable the original, you lose all future updates to that file. DO this ONLY if the logic changes fundamentally and irreversibly.

 **Replacement of entity ID without inheritance.**
If you simply copy the YAML of an entity and change it, you will not receive updates to the parent components from upstream. Always use `parent`.
