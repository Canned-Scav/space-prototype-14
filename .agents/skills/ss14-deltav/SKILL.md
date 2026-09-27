---
name: ss14-deltav
description: Architecture and features of DeltaV (`_DV`): Harpy and Feroxi species abilities, NanoChat and Cartridges, carrying mechanics, Cosmic Cult, and equipment systems.
---

# DeltaV Architecture and Features

This skill covers the DeltaV layer located in `_DV` folders across `Content.Shared`, `Content.Server`, `Content.Client`, and `Resources/Prototypes`.

## 1. Scope and Boundaries

1. This skill covers:
   - Species mechanics: Harpy (vocal syrinx, singing) and Feroxi (hydration system).
   - NanoChat and PDA cartridge communication applications.
   - Physical interactions: carrying/dragging fallen allies, crawling under objects, mouth contraband storage.
   - Cosmic Cult antagonist, monument systems, and glyphs.
   - Mining point vendors and holosign projectors.
2. For medical systems and cybernetics, consult `ss14-shitmed`.
3. For base inventory and hands systems, consult `Content.Shared.Hands`.

## 2. Resource Reading Order

Read resources in this sequence:
1. `references/components-and-systems.md`: Full catalog of DeltaV components, systems, and mechanics.
2. `references/integration-recipes.md`: Practical recipes for PDA messaging, carrying interactions, and contraband storage.
3. `references/common-pitfalls.md`: Common bugs, carrying desyncs, and cartridge setup errors.

## 3. Mental Model and Directory Organization

- `Content.Shared/_DV`: Species mechanics, carrying logic, NanoChat shared structures, holosign components.
- `Content.Server/_DV`: Cosmic cult game rule, PDA chat network, mining voucher systems, weather schedulers.
- `Content.Client/_DV`: Client UI overlays, Harpy visual systems, cartridge interfaces.
- `Resources/Prototypes/_DV`: Prototypes for species, PDA programs, mining vendors, and cosmic cult items.

## 4. Fundamental Rules for `_ScavPrototype`

1. **Isolation**: Never add files to `_DV`. Keep all custom content in `_ScavPrototype`.
2. **Mob Rescue**: When designing scavenging rescue or traversal gear, leverage the carrying system rather than writing redundant dragging code.
3. **Contraband**: Use `MouthStorageComponent` for small concealable salvage items.
