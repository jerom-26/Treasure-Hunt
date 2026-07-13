# Content and Level Design Guide

## 1. Purpose

This document defines how to create fair, readable, replayable clue locations and routes for Treasure Hunt.

The map is handcrafted. Replayability comes from selecting different authored clue locations and riddle variants each match.

---

## 2. Clue-location template

Every location should record:

| Field | Description |
|---|---|
| Location ID | Stable unique identifier |
| Region | Recognizable map area |
| Landmark | Object or formation described by the clue |
| Search method | Dig, inspect, climb, interact, or local puzzle |
| Difficulty | 1–5 |
| Stage use | Intermediate, final, or both |
| Exact solution | Precise completion condition |
| Riddle variants | At least two |
| Intended search radius | Area players are expected to investigate |
| Similar landmarks | Potential sources of ambiguity |
| Visibility notes | Where the landmark can be seen from |
| Failure risks | Blocking, clipping, accidental triggering |
| Reset behavior | How the location returns to unused state |

---

## 3. Riddle-writing rules

### A good riddle identifies

1. A region or landmark
2. A distinguishing feature
3. Sometimes the required search method

### Good example structure

> Beneath the roots of the tree that watches the valley, the first secret sleeps.

- Landmark: prominent tree
- Distinguishing feature: overlooks the valley
- Search method: dig beneath roots

### Avoid

- Pure trivia unrelated to the map
- Words requiring obscure cultural knowledge
- A clue that fits five identical objects
- Exact coordinates disguised as poetry
- Riddles so vague that following other players is the only strategy
- Riddles that directly state the full answer
- Long paragraphs players cannot reread while moving

### Length target

Most riddles should fit in one to three short lines when shown in the compact HUD.

---

## 4. Search-method design

## 4.1 Dig locations

A dig clue needs:

- A clearly interpretable landmark
- A reasonable search radius
- Ground that visually supports digging
- Enough space for several players
- No terrain edge or collision problem
- A hidden zone large enough to reward correct interpretation

Wrong digging should remain possible nearby.

### Recommended discovery profile

- 3–6 credited digs for an intermediate clue
- 5–10 credited digs for a final treasure
- Small delay between credited digs
- Progressive but not immediate reveal

## 4.2 Hidden-object locations

The hidden object should be:

- Invisible from normal travel distance
- Visible after careful local inspection
- Reachable by multiple players
- Not dependent on one exact camera pixel
- Not marked by unique lighting unless the riddle implies it

## 4.3 Climb locations

A climb clue should:

- Use broad, forgiving traversal
- Avoid precision jumping
- Provide at least one obvious possible route after the landmark is found
- Prevent players from getting trapped
- Avoid fall damage in the initial design

## 4.4 Interaction locations

The interaction should be understandable from context:

- Bell can be rung
- Lever can be pulled
- Statue can rotate
- Torch can be lit

Avoid arbitrary interaction with ordinary props that do not appear interactive.

## 4.5 Local puzzles

A local puzzle should take roughly 10–45 seconds after discovery.

The riddle should provide the solution logic. The puzzle should not become a separate five-minute minigame.

---

## 5. Landmark design

A landmark is successful when players can identify and discuss it without developer terminology.

Useful qualities:

- Strong silhouette
- Unique height, shape, or material
- Visible from multiple approaches
- Located within a memorable region
- Not visually confused with another landmark unless intentional

Examples:

- Tallest tree overlooking the valley
- Lone black rock in green grass
- Bridge missing its center planks
- Three pillars, one broken
- Cabin below the snow line
- Statue facing away from the river

---

## 6. Route composition

A standard four-stage match might use:

1. Easy, highly readable opening clue
2. Medium clue using a different search method
3. Harder clue in a different region
4. Final treasure clue with a satisfying search area

### Route rules

- Do not start with the hardest clue.
- Do not use more than two dig stages in a row.
- Do not send players repeatedly between opposite map edges.
- Do not place consecutive clues in the same immediate landmark cluster.
- Use the final clue to create convergence and competition.
- Avoid a final location that can be watched from the previous clue.

### Example route

1. Inspect a hollow in the tallest valley tree
2. Dig beside the lone rock near the river
3. Rotate the broken statue behind the ruins
4. Dig for the final treasure beneath the bridge’s longest shadow

---

## 7. Difficulty rating

### Difficulty 1

- Landmark visible and unique
- Riddle names obvious features
- Broad search area
- Simple action

### Difficulty 2

- Landmark requires light exploration
- Riddle uses one metaphor
- Moderate search radius

### Difficulty 3

- Several possible interpretations
- Requires observing landmark orientation
- Smaller exact solution area

### Difficulty 4

- Requires combining two environmental facts
- Less visible landmark
- Multi-step interaction

### Difficulty 5

Use rarely. Appropriate for special modes, not the first public demo.

---

## 8. Fairness checklist

Before approving a clue:

- [ ] Can a new player locate the general region?
- [ ] Does the riddle describe something actually visible?
- [ ] Are there accidental duplicate answers?
- [ ] Can several players search simultaneously?
- [ ] Can the target be blocked?
- [ ] Can the object be triggered accidentally from far away?
- [ ] Is the solution readable at common graphics settings?
- [ ] Does the clue remain solvable from every spawn orientation?
- [ ] Does the target reset correctly?
- [ ] Is the final action satisfying?

---

## 9. Initial content list

Suggested vertical-slice locations:

| ID | Region | Method | Concept |
|---|---|---|---|
| VALLEY_TALLEST_TREE_ROOTS | Green valley | Dig | Beneath roots of tallest tree |
| VALLEY_HOLLOW_TREE | Green valley | Inspect | Note inside hollow trunk |
| RIVER_BRIDGE_UNDERSIDE | River | Inspect | Map fragment beneath bridge |
| RIVER_BROKEN_BELL | River ruins | Interact | Ring silent/broken bell |
| ROCK_LONE_STONE_BASE | Rocky basin | Dig | Buried beside isolated rock |
| RUINS_BACK_OF_STATUE | Ruins | Inspect | Token hidden behind statue |
| RUINS_THREE_PILLARS | Ruins | Local puzzle | Activate correct pillar order |
| CABIN_ROOF_BEAM | Cabin slope | Climb | Clue tucked above door beam |
| FINAL_BRIDGE_SHADOW | River | Dig final | Treasure under longest shadow |
| FINAL_SNOWLINE_CAVE | Snow ridge | Inspect final | Chest behind concealed cave panel |

---

## 10. Content production targets

### Vertical slice

- 8 intermediate locations
- 2 final locations
- 16+ riddle texts
- 3 search methods
- 1 complete route presentation set

### Demo

- 18–24 intermediate locations
- 5–8 final locations
- 50+ riddle variants
- 4–5 search methods
- Multiple difficulty-balanced route profiles
