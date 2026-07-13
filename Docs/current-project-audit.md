# Current Prototype Audit

## Current playable scene

- `Assets/Scenes/SampleScene.unity` is the current game prototype.
- Build Settings currently need verification/configuration for a standalone build.
- Numerous other scenes are third-party asset demonstrations and are not game scenes.

## Current game-owned scripts

- `ClueManager.cs` — hardcoded riddles, random clue-chest positions, proximity distance, E-based progression, and final reveal.
- `ClueUIManager.cs` — clue popup/HUD and digging prompt presentation.
- `FPSController.cs` — CharacterController movement and camera; class naming/jump implementation need review.
- `MinimapScript.cs` — follows the player.
- `ChestInteractionScript.cs` — proximity chest interaction that may conflict with central progression.
- `PlayerDigging.cs` — trigger-based F interaction, not free digging.
- `TreasureSpawner.cs`, `DigSpot.cs`, `TreasureDetection.cs`, and `Treasure.cs` — partial or disconnected treasure prototypes.

## Reusable foundations

- Existing terrain and landmarks
- First-person movement after small repairs
- Clue UI and reopen behaviour
- Minimap camera follow
- Final chest asset/presentation
- Terrain height and slope checks
- Existing debug distance information, moved behind a development toggle

## Known redesign needs

- Riddles are currently unrelated to random chest coordinates.
- Visible chest/proximity collection must be replaced by environmental discovery.
- Several overlapping interaction scripts can conflict.
- There are no stable clue IDs, authored clue data assets, route seed, bot metadata, timer, rankings, or shared completion interface.
- The final chest lacks a complete competitive claim/winner system.
- `ClueManager.cs` reportedly imports `UnityEditor` in runtime code and should be corrected after backup.

## Current source-control state

- Commit `e7abd13` is backed up on `origin/main`.
- The annotated tag `legacy-singleplayer-prototype` exists locally and remotely at the legacy checkpoint.
- Commit `38f0879`, which records previously excluded third-party tree-log assets, is preserved separately through the local branch `archive/third-party-log-assets` and has not been pushed.
- The active development branch is `rival-hunt`, based on `e7abd13`.
- Generated Unity directories are anchored to the repository root in `.gitignore`; no generated cache or build folders are part of the documentation cleanup.

Compilation, scene/build configuration, and runtime code cleanup have not yet been completed. This audit remains a starting point, and Codex must verify current files and Git state before acting because the repository may change.
