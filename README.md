# Treasure Hunt — Rival Mode Design Pack

This is the current design authority for the first commercial version of **Treasure Hunt**.

## Current direction

The first release is a **single-player competitive treasure hunt with AI rival hunters**. The player and bots receive the same active riddle, search the same randomized route, and compete to claim the final treasure before another hunter or the session timer.

Online multiplayer is a future expansion. It must not be implemented for the first release, but the core gameplay code should avoid decisions that make multiplayer conversion unnecessarily difficult.

## Core pitch

> Race rival treasure hunters across a changing island, solve environmental riddles, dig wherever you believe the answer lies, and claim the final treasure before your opponents or the timer.

## Documents

- [Codex instructions](AGENTS.md)
- [Game Design Document](Docs/game-design-document.md)
- [Eight-week Production Plan](plan.md)
- [Technical Design](Docs/technical-design.md)
- [Content and Level Design Guide](Docs/content-design-guide.md)
- [Bot Design](Docs/bot-design.md)
- [Playtest Plan](Docs/playtest-plan.md)
- [Product Backlog](Docs/product-backlog.md)
- [Decision Log](Docs/decision-log.md)
- [Current Prototype Audit](Docs/current-project-audit.md)

## First-release scope

- One existing map
- One human player
- One to three AI rivals
- Three intermediate clues and one final treasure per session
- Dig, inspect, and environmental-interaction clue types
- Ten to twelve intermediate clue locations
- Three to four final treasure locations
- Randomized route and riddle variants
- Session timer
- Results screen, difficulty settings, and saved personal records
- No online multiplayer in version 1.0

## Scope rule

The first release is not an open-world adventure, survival game, story campaign, combat game, or procedural terrain generator. It is one polished map, short repeatable sessions, reliable bots, and a strong environmental-search loop.
