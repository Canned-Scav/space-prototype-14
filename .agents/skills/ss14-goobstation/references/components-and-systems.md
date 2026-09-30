# Goob Station: Components and Systems Register

This catalog details the key systems, components, and UI controls introduced in `Content.Goobstation.*`.

## 1. Antagonists

### 1.1 Changeling
- **Namespace**: `Content.Goobstation.Shared.Changeling`, `Content.Goobstation.Server.Changeling`
- **Systems**:
  - `SharedChangelingSystem`: Handles chemical generation loop, sting attempts, DNA extraction, and transformation abilities.
  - `ChangelingSystem` (Server): Manages objective tracking, soul absorption, and game rule lifecycle.
- **Components**:
  - `ChangelingComponent`: Stores current chemicals, max chemicals, DNA profiles, current active sting, and unlocked powers.
- **Events**:
  - `ChangelingAbsorbAttemptEvent`: Raised before an entity is absorbed. Cancelable.
  - `ChangelingStingAttemptEvent`: Raised before applying a chemical sting. Cancelable.
  - `ChangelingTransformEvent`: Fired when a changeling morphs into another saved identity.

### 1.2 Heretic
- **Namespace**: `Content.Goobstation.Shared.Heretic`, `Content.Goobstation.Server.Heretic`
- **Systems**:
  - `SharedHereticSystem`: Manages paths of knowledge (Ash, Flesh, Void, Blade), Mansus runes, eldritch spells, and transmutation.
  - `HereticSystem` (Server): Tracks sacrifices, ascension rites, and influence points.
- **Components**:
  - `HereticComponent`: Stores path progression, knowledge points, selected spells, and sacrifice targets.
  - `MansusRuneComponent`: Placed on ritual rune entities drawn on the floor.

### 1.3 Blob
- **Namespace**: `Content.Goobstation.Shared.Blob`, `Content.Goobstation.Server.Blob`
- **Systems**:
  - `SharedBlobSystem`: Handles blob tile expansion, pulse synchronization, and damage calculations.
  - `BlobRuleSystem` (Server): Blob game rule, stage transitions, and victory conditions.
- **Components**:
  - `BlobCoreComponent`: Central consciousness and spawn point.
  - `BlobNodeComponent`: Relays pulses across the infection network.
  - `BlobTileComponent`: Applied to every infected grid tile.
  - `BlobResourceComponent`, `BlobFactoryComponent`: Specialized node upgrades.

### 1.4 Other Antagonists
The following antagonist systems exist in `Content.Goobstation.Shared`:

| Directory | Antagonist Type |
| --- | --- |
| `Devil` | Devil antagonist with contracts and soul trading |
| `DarkLord` | Dark Lord with minion summoning |
| `Shadowling` | Shadowling with thrall conversion and light vulnerability |
| `Wraith` | Wraith with phase shifting and invisibility |
| `Nightmare` | Nightmare creature with fear mechanics |
| `Slasher` | Slasher horror antagonist |
| `ChronoLegionnaire` | Time-manipulating antagonist |
| `Xenomorph` | Xenomorph with hive mechanics |
| `Hastur` | Lovecraftian antagonist |
| `Pirates` | Pirate faction system |
| `Traitor` | Extended traitor system |
| `Revolutionary` | Extended revolutionary system |
| `SlaughterDemon` | Bluespace demon |

## 2. Combat, Weapons and Movement

### 2.1 Core Combat Systems
| System | Namespace | Purpose |
| --- | --- | --- |
| `MartialArts` | `Content.Goobstation.Shared.MartialArts` | Martial arts combat styles with combo chains |
| `Grab` / `GrabIntent` / `GrabReleaseBind` | `Content.Goobstation.Shared.Grab` | Enhanced grab mechanics with aggressive/passive modes |
| `BerserkerImplantSystem` | `Content.Goobstation.Shared.BerserkerImplant` | Rage state on low health, pain immunity, speed boost |
| `BloodtrakSystem` | `Content.Goobstation.Shared.Bloodtrak` | Scent tracking and bloodhound mechanics |
| `BoomerangSystem` | `Content.Goobstation.Shared.Boomerang` | Curved trajectory physics and catching mechanics |
| `MantisBlades` | `Content.Goobstation.Shared.MantisBlades` | Retractable cybernetic arm blades |
| `ContractorBaton` | `Content.Goobstation.Shared.ContractorBaton` | Syndicate contractor stun baton |
| `RecoilAbsorber` | `Content.Goobstation.Shared.RecoilAbsorber` | Firearm recoil reduction |
| `SmartLinkImplant` | `Content.Goobstation.Shared.SmartLinkImplant` | Smart-linked weapon accuracy bonus |
| `TableSlam` | `Content.Goobstation.Shared.TableSlam` | Slamming entities into tables |
| `Weapons` | `Content.Goobstation.Shared.Weapons` | Extended weapon mechanics |
| `Projectiles` | `Content.Goobstation.Shared.Projectiles` | Extended projectile systems |

### 2.2 Movement Systems
| System | Purpose |
| --- | --- |
| `Dash` | Short-range teleport dash ability |
| `Sandevistan` | Cyberpunk-style time dilation movement |
| `PhaseShift` | Phase through solid objects temporarily |
| `Vehicles` | Rideable vehicle entities |
| `Sprinting` | Extended sprint mechanics |
| `MomentumSteering` | Inertia-based movement control |
| `Waddle` | Waddling movement animation |
| `Stealth` | Stealth/invisibility mechanics |

## 3. Cybernetics and Augmentations

- **Namespace**: `Content.Goobstation.Shared.Augments`, `Content.Goobstation.Shared.Autosurgeon`
- **Systems**:
  - `AutosurgeonSystem`: Coordinates automated surgery chambers, cartridge scanning, and implant installation.
  - `AugmentSystem`: Manages passive stat modifications and active ability cooldowns from installed augments.
- **Components**:
  - `AugmentComponent`: Attached to augment item entities, detailing slot (Head, Torso, Arms, Legs) and bonuses.
  - `AutosurgeonComponent`: Attached to autosurgeon medical machinery.

## 4. Engineering and Science

| Directory | Purpose |
| --- | --- |
| `Supermatter` | Supermatter crystal engine and delamination |
| `Factory` | Automated factory production lines |
| `Enchanting` | Item enchantment system |
| `Xenobiology` | Xenobiology research and slime management |
| `Research` | Extended research system |
| `Power` | Power system extensions |
| `Atmos` | Atmospheric system extensions |
| `Wires` | Wire panel hacking extensions |
| `Electrocution` | Extended electrocution mechanics |

## 5. Medical and Biology

| Directory | Purpose |
| --- | --- |
| `Virology` | Virus creation, mutation, and transmission |
| `Disease` | Disease simulation and treatment |
| `Surgery` | Goob Station surgery extensions |
| `Medical` | Extended medical mechanics |
| `CheckInfection` | Infection checking system |
| `Body` | Body system extensions |

## 6. Social and Communication

| Directory | Purpose |
| --- | --- |
| `Emag` | Extended emag interactions with sparks/sound feedback |
| `Emoting` | Extended emote system |
| `Voice` / `Speech` | Voice and speech modifications |
| `Radio` / `StationRadio` | Extended radio systems |
| `Communications` | Extended communication systems |
| `Fax` | Fax machine extensions |
| `TapeRecorder` | Tape recorder playback/recording |
| `Polls` | In-game polling system |
| `GPS` | GPS tracking system |
| `Loudspeaker` | Loudspeaker broadcast system |

## 7. Miscellaneous Systems

| Directory | Purpose |
| --- | --- |
| `Teleportation` | Teleportation mechanics |
| `Fishing` | Fishing minigame |
| `Guardian` | Guardian spirit summoning |
| `Mimery` | Mime abilities (invisible walls, etc.) |
| `Voodoo` | Voodoo doll manipulation |
| `Religion` / `Exorcism` / `Bible` | Religious mechanics |
| `CloneProjector` | Clone projection hologram |
| `HoloCigar` | Holographic cigar |
| `SlotMachine` | Gambling slot machine |
| `Bingle` | Bingle card game |
| `Implants` | Extended implant system |
| `Traits` | Character traits system |
| `Keyring` | Key ring management |

## 8. UIKit Controls and RichText Tags

- **Namespace**: `Content.Goobstation.UIKit`
- **Custom Controls**:
  - `IconButton`: Texture-based button with interactive hover/pressed tinting.
  - `ShaderLabel`: Renders text through custom SWSL shader passes.
  - `StaticSpriteView`: Low-overhead sprite preview control for item listings.
  - `TooltipTextureRect`: Texture display with rich tooltip binding.
- **RichText Tags**:
  - `[icon prototype="..."]`: Embeds an entity prototype icon inline within chat or labels.
  - `[texture path="..."]`: Embeds a texture from the resource path.
  - `[entity uid="..."]`: Inline entity display tag.
  - `[button id="..."]`: Clickable inline action button inside UI text.
  - `[radio channel="..."]`: Channel badge icon for radio transmissions.

## 9. Goobstation Common and Maths

- **`Content.Goobstation.Common`**: Shared helper structures, data types, `Traits` namespace (used by `CarryingSystem` via `Content.Goobstation.Common.Traits`).
- **`Content.Goobstation.Maths`**: `FixedPoint` fixed-point arithmetic (heavily used by Shitmed for wound/damage values), trajectory calculations, curve solvers.
