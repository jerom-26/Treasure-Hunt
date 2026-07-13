# Decision Log

## D-001 — Original competitive multiplayer concept

The original design used 4–8 online players. Everyone received the same riddle. The first player to solve an intermediate clue advanced the route globally, and the first player to claim the final treasure won.

**Status:** Preserved as the future multiplayer direction, not the first release.

## D-002 — Environmental search instead of proximity collection

Players can dig anywhere on approved terrain. Clues may also be hidden in trees, under bridges, behind terrain features, or inside environmental interactions. No release HUD should identify the correct location through a proximity prompt.

**Status:** Active.

## D-003 — Authored random routes

Each session chooses a different route from handcrafted clue locations and riddle variants. Targets are not placed at arbitrary coordinates unrelated to the riddle.

**Status:** Active.

## D-004 — First commercial release pivots to single-player Rival Hunt

Because online multiplayer would exceed the desired first-release schedule, version 1.0 uses one human and AI rival treasure hunters.

The core shared-progression rule remains:

- Human or bot solving an intermediate clue advances the route for everyone.
- The first competitor to claim the final treasure wins.
- A timer prevents stalled sessions.

**Status:** Current design authority.

## D-005 — Bots use authored search data

Bots do not parse natural-language riddles. Each clue supplies plausible candidate landmarks, search points, weights, and mistake behaviour. Bots perform the same visible search actions and use the same completion rules as the human.

**Status:** Active.

## D-006 — Future multiplayer compatibility

Version 1.0 contains no networking. Human input and bot decision logic remain separate from shared treasure-hunter actions so a future network-controlled competitor can be added without replacing clue and match systems.

**Status:** Active, but must not cause unnecessary pre-optimization.

## D-007 — Debug tools are retained

Distance, route seed, active target ID, bot state, and discovery-zone information are necessary during development. They must be configurable and hidden in normal release builds rather than deleted.

**Status:** Active.

## D-008 — Eight-week scope

The intended first release uses one map, three intermediate clues plus final treasure, ten to twelve intermediate locations, three to four final locations, three solution methods, one to three bots, difficulty settings, timer, results, and personal records.

A second map, combat, progression, story, multiplayer, climbing, and complex puzzles are deferred.

**Status:** Active.
