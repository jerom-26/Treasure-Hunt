# Codex Project Instructions

## Authority order

1. `Docs/game-design-document.md`
2. `plan.md`
3. `Docs/technical-design.md`
4. Supporting documents in `Docs/`

When documents conflict, follow the highest document in this list and report the conflict.

## Current product direction

- Build a single-player competitive treasure hunt with AI rival hunters.
- Do not implement online multiplayer for the first commercial release.
- Preserve clean extension points for future online players.
- Bots and the human must use the same clue-completion, digging, interaction, scoring, and treasure-claim systems.
- Bots do not need to understand natural-language riddles. They use authored search metadata and controlled mistakes.
- A bot solving an intermediate clue advances the route for everyone.
- The first competitor to claim the final treasure wins.
- A session timer ends stalled matches.

## Working rules

- Follow `plan.md` milestone by milestone.
- Give one manageable development task at a time.
- Before editing, state the exact files to create or modify.
- Do not expand scope without explicit approval.
- Do not rewrite working systems unnecessarily.
- Do not edit Unity scenes, prefabs, imported assets, terrain, or package files unless explicitly approved.
- Prefer C# scripts plus precise manual Unity Inspector instructions.
- Preserve developer debug tools. Put spoilers such as clue distance, target IDs, route seed, and discovery radii behind an Editor/Development Build toggle; do not permanently delete them.
- Do not expose solution markers, clue distance, or hidden target locations in normal release builds.
- Do not begin networking.
- Avoid speculative architecture that is not needed for the current milestone.
- Explain code and Unity setup in beginner-friendly language.
- Run available checks and report compile/test limitations honestly.
- Show diffs before broad changes.
- Provide a suitable Git commit message after each completed feature.

## Release constraint

The target is a tightly scoped commercial first release in approximately eight weeks of focused development. Cut features rather than extending the schedule. The bot system is required for the intended Rival Hunt mode, but online multiplayer, a second map, progression, combat, built-in voice chat, and fully deformable terrain are outside version 1.0.
