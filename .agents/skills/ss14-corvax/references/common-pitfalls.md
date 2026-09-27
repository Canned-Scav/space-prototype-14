# Corvax: Common Pitfalls and Anti-Patterns

This document records architectural mistakes and common bugs when working with Corvax modules.

## 1. Direct Edits to Root `Corvax/` Interfaces
- **Anti-pattern**: Attempting to implement gameplay logic directly inside `Corvax/Content.Corvax.Interfaces.*`.
- **Consequence**: Breaks the architectural boundary between web/infrastructure integration and gameplay code.
- **Rule**: Interfaces in `Corvax/` are strictly contracts (stubs, Discord authentication, join queue); gameplay systems belong in `_ScavPrototype`.

## 2. Hardcoding Gendered Strings in Russian Localization
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

## 3. Unconstrained Announcer Flooding
- **Anti-pattern**: Calling `_announcer.SendAnnouncement(...)` from high-frequency event loops or weapon firing hooks.
- **Consequence**: Station audio queue overflows, deafening players and desyncing client audio channels.
- **Rule**: Gate announcer calls behind cooldowns, round stage transitions, or explicit admin actions.
