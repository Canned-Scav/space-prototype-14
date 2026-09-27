---
name: ss14-shitmed
description: Architecture and subsystems of Shitmed surgery, body and organ simulation (`_Shitmed`): targeted wounds, limb amputation, organ mechanics, cybernetics, Autodoc, and tourniquets.
---

# Shitmed Architecture: Surgery, Body and Damage

This skill covers the Shitmed medical and anatomical system located in `_Shitmed` across `Content.Shared`, `Content.Server`, `Content.Client`, and `Resources/Prototypes`.

In Goob Station, Shitmed completely replaces vanilla SS14 monolithic entity health with realistic localized limb damage, organ simulation, bleeding, and surgical procedures.

## 1. Scope and Boundaries

1. This skill covers:
   - Targeting system and directed combat damage routing.
   - Per-limb health, bone fractures, dismemberment, and arterial bleeds.
   - Internal organ simulation (brain, heart, lungs, eyes) and failure states.
   - Surgical procedures, tool qualities, and the Autodoc automated booth.
   - Cybernetic limbs and prosthetic organ replacements.
2. For base chemistry and reagent metabolism, consult `Content.Shared.Chemistry`.
3. For base SS14 damage containers, consult `ss14-ecs-prototypes`.

## 2. Resource Reading Order

Read resources in this sequence:
1. `references/components-and-systems.md`: Register of targeting components, body part statuses, surgical systems, and cybernetics.
2. `references/integration-recipes.md`: Recipes for authoring armor part coverage, heavy weapons with fracture chances, and tourniquets.
3. `references/common-pitfalls.md`: Frequent design traps when balancing combat or healing against Shitmed.

## 3. Mental Model and Directory Organization

- `Content.Shared/_Shitmed`: Targeting components, surgery do-after events, body part statuses, cybernetics definitions.
- `Content.Server/_Shitmed`: Surgery resolution, Autodoc logic, organ failure, amputation, arterial bleeding.
- `Content.Client/_Shitmed`: Targeting HUD UI, surgery radial menu, body part damage indicators.
- `Resources/Prototypes/_Shitmed`: Surgical operations, tools, cybernetic implants, body parts, and damage containers.

## 4. Fundamental Rules for `_ScavPrototype`

1. **Armor Coverage**: Armor in `_ScavPrototype` must explicitly define which Shitmed body parts it shields. A vest protecting only `Torso` leaves `Groin` and `Arms` vulnerable.
2. **Weapons**: Calibrate damage values against individual limb health pools rather than total entity HP.
3. **Medical Gear**: Healing items and field dressings must address localized wounds, fractures, or arterial bleeding.
4. **Isolation**: Never edit `_Shitmed` files directly. Keep custom medical items and armor in `_ScavPrototype`.
