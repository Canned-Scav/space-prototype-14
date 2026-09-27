---
trigger: always_on
---

# Rule: Defining the codebase prefix, project folder and edit markers

This rule is mandatory for any task in the Space Prototype 14 repository (`space-prototype-14`).

## 1. What you need to determine before starting work

Before analyzing, planning paths and making changes, always fix three values:

1. Active codebase prefix (`ScavPrototype`, `Scav`).
2. Forked project folder (`_ScavPrototype`).
3. The text of the edit marker that should be used in comments (`scav-edit`, `Scav-edit`).

Don't start editing vanilla or upstream files until these three values are defined.

## 2. How to determine the project and fork context

Define the project identity by collecting concrete signals:

1. Git remote repository slug: `space-prototype-14` in `Canned-Scav/space-prototype-14`.
2. The name of the repository root folder and the path of the working directory: `space-prototype-14`.
3. Which forked project folder is used for custom changes: `_ScavPrototype`.
4. The nearest existing edit markers in the adjacent code: `scav-edit`, `Scav-edit`, `// scav-edit start` / `// scav-edit end`.

### Upstream and inherited code:
- Upstream repository is Goob Station (`space-syndicate/Goob-Station`), using an AGPL-3.0 codebase.
- The repository structure consists of:
  1. Base SS14 projects (`Content.Server`, `Content.Shared`, `Content.Client`, `Resources/Prototypes/Entities`, etc.).
  2. Goob Station C# projects (`Content.Goobstation.Server`, `Content.Goobstation.Shared`, `Content.Goobstation.Client`, `Content.Goobstation.Common`, `Content.Goobstation.Maths`, `Content.Goobstation.UIKit`).
  3. Inherited upstream folders (`_Goobstation`, `_DV`, `_Corvax`, `_EinsteinEngines`, `_Shitcode`, `_Shitmed`, etc.).
- **Critical rule for new code:** All new custom features, systems, components, prototypes, and assets belonging to our project must strictly go into `_ScavPrototype`. Never place new custom files into `Content.Goobstation.*`, `_Goobstation`, `_DV`, `_Corvax`, or vanilla directories.
- **Rule for modifying upstream files:** Modifying upstream files (both vanilla and Goob Station / DeltaV / Corvax) is **allowed when unavoidable** (e.g. hooking an event deep inside an upstream method or modifying an upstream prototype where inheritance/migration is insufficient), but edits must remain **minimal** (ideally 1–2 lines raising an event or calling a hook) and must strictly be marked with `scav-edit` markers.

## 3. Correspondence map

Select the line matching our project configuration:

| Match | Prefix | Project folder | Single-line marker | Block markers | Note |
| --- | --- | --- | --- | --- | --- |
| `Canned-Scav/space-prototype-14`, `space-prototype-14`, `_ScavPrototype`, `ScavPrototype` | `ScavPrototype` / `Scav` | `_ScavPrototype` | `scav-edit` / `Scav-edit` | `scav-edit start` / `scav-edit end`, `scav added start` / `scav added end` | Primary project configuration. Default single-line marker is `scav-edit`. |

## 4. How to apply a marker in a specific file

The general rules for marking edits:

1. Use `Prefix` (`ScavPrototype` / `Scav`), `Project folder` (`_ScavPrototype`) and `marker` (`scav-edit` / `Scav-edit`).
2. Do not change the marker text, just adapt the comment syntax to the file language.
3. If the file already uses an existing local case/variant (e.g. `#Scav-edit` or `// scav-edit start`), follow the local file style.

Select the comment syntax to match the file language:

- C#, C++, Java: `// scav-edit`, `// scav-edit start - reason` ... `// scav-edit end`
- YAML, FTL, Python, Shell: `# scav-edit`, `# scav-edit start - reason` ... `# scav-edit end`
- XML, HTML: `<!-- scav-edit -->`, only if comments in this format are allowed and really needed

## 5. How does this affect the structure of edits?

1. Place all new project files, systems, components, prototypes, and assets in `_ScavPrototype` (e.g. `Content.Shared/_ScavPrototype`, `Content.Server/_ScavPrototype`, `Content.Client/_ScavPrototype`, `Resources/Prototypes/_ScavPrototype`, `Resources/Locale/*/_ScavPrototype`).
2. Mark minimal hooks in upstream/vanilla files with the edit marker `scav-edit`.
3. Never put custom space-prototype-14 code into `Content.Goobstation.*`, `_Goobstation`, `_DV`, `_Corvax`, or vanilla folders.
4. Keep modifications to upstream and vanilla files minimal to prevent merge conflicts when rebasing or merging upstream updates.
