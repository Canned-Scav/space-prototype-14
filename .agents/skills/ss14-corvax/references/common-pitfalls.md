# Corvax: Common Pitfalls and Anti-Patterns

This document records architectural mistakes and common bugs when working with Corvax modules.

## 1. Confusing `AnnouncerSystem` with a Broadcast API

- **Anti-pattern**: Calling `AnnouncerSystem` methods expecting to send station-wide text/audio messages.
- **Consequence**: The system only selects which `AnnouncerPrototype` voice pack is active for the round. It does **not** broadcast messages.
- **Rule**: Use vanilla `ChatSystem.DispatchStationAnnouncement(...)` for station-wide broadcasts. Use `AnnouncerSystem.ForceSetAnnouncer(...)` only to override which voice pack plays during announcements.

## 2. Direct Edits to Root `Corvax/` Interfaces

- **Anti-pattern**: Attempting to implement gameplay logic directly inside `Corvax/Content.Corvax.Interfaces.*`.
- **Consequence**: Breaks the architectural boundary between web/infrastructure integration and gameplay code.
- **Rule**: Interfaces in `Corvax/` are strictly contracts (stubs, Discord authentication, join queue); gameplay systems belong in `_ScavPrototype`.

## 3. Hardcoding Gendered Strings in Russian Localization

- **Anti-pattern**: Writing hardcoded Russian strings like `он взял` or `она пошла` without gender selectors.
- **Consequence**: Broken grammatical immersion when characters of different genders trigger the interaction.
- **Rule**: Use Corvax grammatical parameters in `.ftl` strings:
  ```ftl
  scav-action-take = {$user} {GENDER($user) ->
      [male] взял
      [female] взяла
      *[epicene] взяли
  } предмет.
  ```

## 4. TTS Rate Limit Ignoring

- **Anti-pattern**: Triggering TTS playback from high-frequency event loops or rapid action handlers without rate limiting.
- **Consequence**: `TTSSystem.RateLimit.cs` will throttle or drop messages, causing silent failures and confusing audio gaps.
- **Rule**: Gate TTS-triggering logic behind cooldowns or significant state transitions. The TTS system has built-in rate limiting, but excessive calls still waste server resources.

## 5. Misusing CorvaxGoob Criminal Records

- **Anti-pattern**: Attempting to query criminal record status via `CriminalRecordExamineSystem` in CorvaxGoob.
- **Consequence**: `CriminalRecordExamineSystem` only adds text to entity examine output. It is not a data API.
- **Rule**: Use vanilla `CriminalRecordsSystem` from `Content.Server.CriminalRecords.Systems` for querying and modifying criminal records programmatically.

## 6. Confusing Nuclear Reactor Partial Classes

- **Anti-pattern**: Looking for reactor station logic in `NuclearReactorSystem.cs` and not finding it.
- **Consequence**: Missing the `NuclearReactorSystemStation.cs` partial class that handles station-level reactor integration.
- **Rule**: `NuclearReactorSystem` is a multi-partial class. Check all `.cs` files in the `FissionGenerator` directory.

## 7. Breaking Multi-Partial System Pattern

- **Anti-pattern**: Adding logic directly to `OfferItemSystem` or `TTSSystem` main `.cs` file instead of checking for existing partial files.
- **Consequence**: Duplicating event subscriptions or overriding partial class initialization that already exists in `.Interactions.cs`, `.Verbs.cs`, `.Sanitize.cs`, etc.
- **Rule**: Before modifying any CorvaxGoob system, list all files in its directory. Many systems are multi-partial (`SharedOfferItemSystem.cs`, `SharedOfferItemSystem.Interactions.cs`, `SharedOfferItemSystem.Verbs.cs`).
