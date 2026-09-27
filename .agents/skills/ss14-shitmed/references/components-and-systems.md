# Shitmed: Components and Systems Register

Complete catalog of Shitmed systems, components, and anatomical structures.

## 1. Targeting Subsystem

- **Namespace**: `Content.Shared._Shitmed.Targeting`
- **Systems**:
  - `SharedTargetingSystem`: Coordinates active target zone selection between client UI and server combat resolution.
- **Components**:
  - `TargetingComponent`: Tracks the player's currently selected target zone.
- **Target Zones**:
  - `Head`, `Torso`, `Groin`, `LeftArm`, `RightArm`, `LeftHand`, `RightHand`, `LeftLeg`, `RightLeg`, `LeftFoot`, `RightFoot`.

## 2. Body Parts, Organs and Wounds

- **Namespace**: `Content.Shared._Shitmed.Body`, `Content.Shared._Shitmed.PartStatus`, `Content.Server._Shitmed.Body`
- **Systems**:
  - `PartStatusSystem`: Evaluates limb health thresholds, fractures, and amputations.
  - `OrganSystem`: Simulates internal organ health and failure consequences.
- **Part Statuses**:
  - `Intact`: Normal function.
  - `Fractured`: Severe pain, impaired movement speed or manipulation.
  - `ArterialBleed`: Critical blood loss; requires rapid tourniquet or clamping.
  - `Severed`: Limb detached from body; drops as a physical item entity.
- **Organs**:
  - Brain (consciousness, cognition), Heart (circulation), Lungs (breathing, gas exchange), Eyes (vision).

## 3. Tourniquets and Bleeding Control

- **Namespace**: `Content.Shared._Shitmed.Tourniquet`, `Content.Server._Shitmed.Tourniquet`
- **Systems**:
  - `TourniquetSystem`: Handles applying, tightening, and removing tourniquets on limbs.
- **Components**:
  - `TourniquetComponent`: Placed on tourniquet items or applied as a status on limbs to halt arterial bleeding.

## 4. Surgery and Autodoc

- **Namespace**: `Content.Shared._Shitmed.Surgery`, `Content.Shared._Shitmed.Autodoc`
- **Systems**:
  - `SharedSurgerySystem` / `SurgerySystem`: Orchestrates surgical step requirements, tool validation, and do-after events.
  - `AutodocSystem`: Automated surgery pod that diagnoses patient damage and performs operations autonomously.
- **Surgical Steps and Tool Qualities**:
  - `Incision`: Scalpel
  - `Retraction`: Retractor
  - `Clamping`: Hemostat
  - `Sawing`: Bone saw
  - `BoneSetting`: Bone gel
  - `Cauterization`: Cautery
  - `Suturing`: Surgical suture / needle

## 5. Cybernetics and Prosthetics

- **Namespace**: `Content.Shared._Shitmed.Cybernetics`
- **Components**:
  - `CyberneticsComponent`: Applied to cybernetic limbs or prosthetic organs.
  - Grants damage type immunities (e.g. no biological bleeding, immune to suffocation), but introduces vulnerability to EMP and electrical overload.
