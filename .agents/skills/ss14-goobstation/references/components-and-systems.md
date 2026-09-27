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

## 2. Cybernetics and Augmentations

- **Namespace**: `Content.Goobstation.Shared.Augments`, `Content.Goobstation.Shared.Autosurgeon`
- **Systems**:
  - `AutosurgeonSystem`: Coordinates automated surgery chambers, cartridge scanning, and implant installation.
  - `AugmentSystem`: Manages passive stat modifications and active ability cooldowns from installed augments.
- **Components**:
  - `AugmentComponent`: Attached to augment item entities, detailing slot (Head, Torso, Arms, Legs) and bonuses.
  - `AutosurgeonComponent`: Attached to autosurgeon medical machinery.

## 3. Combat, Weapons and Equipment

- **Namespace**: `Content.Goobstation.Shared.*`
- **Systems**:
  - `BerserkerImplantSystem`: Triggers rage state on low health or manual command; grants pain immunity and movement speed.
  - `BloodtrakSystem`: Scent tracking and bloodhound mechanics for tracking wounded prey across grid sectors.
  - `BoomerangSystem`: Solves curved trajectory physics and catching mechanics for returning throwing weapons.
  - `EmagSystem` (Goobstation overrides): Extended interactions and sparks/sound feedback for electronic warfare.

## 4. UIKit Controls and RichText Tags

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
