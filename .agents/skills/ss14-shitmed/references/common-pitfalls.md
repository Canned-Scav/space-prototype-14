# Shitmed: Common Pitfalls and Anti-Patterns

This document records frequent design bugs and balancing errors when interfacing with Shitmed.

## 1. Omitting Body Part Coverage on Armor
- **Anti-pattern**: Creating a helmet, vest, or greaves without specifying `ProtectedBodyParts`.
- **Consequence**: The armor provides passive damage resistance on paper, but because incoming hits target specific body parts, attacks to that limb bypass the armor completely.
- **Rule**: Every piece of combat clothing must explicitly list its protected body parts.

## 2. Inappropriate Weapon Damage Scaling
- **Anti-pattern**: Giving a rapid-fire weapon 25-30 damage per bullet without realizing each limb has roughly 40-60 health.
- **Consequence**: Targets are instantly dismembered or decapitated within two bullets.
- **Rule**: Balance weapon damage with individual limb health pools in mind: rapid-fire weapons should deal 8-15 damage, heavy rifles 25-35, and sniper/heavy weapons 40-55.

## 3. Ignoring Tourniquet Necrosis
- **Anti-pattern**: Applying a tourniquet and leaving it on indefinitely.
- **Consequence**: Prolonged blood cutoff causes limb necrosis, forcing medical amputation.
- **Rule**: Tourniquets are emergency stabilization tools; wounds must be clamped or treated surgically, after which the tourniquet is removed.
