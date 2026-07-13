# Content and Level Design Guide

## 1. Purpose

This guide defines how to create fair clue locations that work for both the human player and rival bots.

The world is handcrafted. Replayability comes from route selection, riddle variants, and bot decisions.

---

## 2. Clue-location template

Every location must record:

| Field | Description |
|---|---|
| Location ID | Stable unique identifier |
| Region | Recognizable map area |
| Landmark | Object or formation described by the riddle |
| Search method | Dig, inspect, or interact |
| Difficulty | 1–5 |
| Stage use | Intermediate, final, or both |
| Exact human solution | Valid completion condition |
| Riddle variants | At least two |
| Intended search radius | Area the human should investigate |
| Similar landmarks | Potential ambiguity |
| Bot search profile | Correct/wrong candidates and search points |
| Navigation anchors | Reachable bot positions |
| Reset behaviour | State restored between sessions |
| Failure risks | Blocking, clipping, accidental trigger |

A location is incomplete until both the human and a bot can finish it.

---

## 3. Riddle rules

A good riddle identifies:

1. A region or landmark
2. A distinguishing feature
3. Sometimes the required search method

Example:

> Beneath the roots of the tree that watches the valley, the first secret sleeps.

- Landmark: prominent tree
- Feature: overlooks the valley
- Method: dig beneath roots

Avoid:

- External trivia
- Exact coordinates disguised as poetry
- A clue fitting many identical objects unintentionally
- Extremely long text
- Obscure cultural knowledge
- Wording that makes random brute force the best strategy

Most riddles should fit in one to three short HUD lines.

---

## 4. Search-method requirements

### Dig

- Ground visibly supports digging.
- Several players/bots could theoretically search the area in a future version.
- Discovery zone is broad enough to reward correct interpretation.
- Wrong nearby digs remain possible.
- Bot search points cover both correct and plausible incorrect positions.

Initial targets:

- Intermediate: 3–6 credited digs
- Final: 5–10 credited digs

### Inspect

- Hidden object is invisible from normal travel distance.
- It becomes visible through careful local observation.
- Interaction does not require one exact camera pixel.
- Bot has a reachable inspection anchor.

### Interact

- The object visibly suggests an action: bell, lever, statue, torch, panel.
- The riddle identifies why this object matters.
- No arbitrary interaction with ordinary decoration.
- Bot can stand at the same valid action anchor.

---

## 5. Bot candidate design

For every location, create:

- Correct candidate landmark
- One or more wrong candidates with a believable relationship to the wording
- Search points for each candidate
- Plausibility values

Wrong candidates should be plausible enough to make the bot look thoughtful, but not so similar that the human clue becomes unfair.

Do not create wrong candidates merely to waste time across the entire map. They should support readable behaviour.

---

## 6. Route composition

A standard session uses:

1. Easy opening clue
2. Medium clue with a different method or region
3. Harder clue
4. Final treasure clue

Rules:

- No duplicate location.
- Avoid more than two stages with the same method in a row.
- Avoid repeated travel between opposite map edges.
- Do not start with the hardest clue.
- Final location should create a visible race and satisfying claim.
- Every selected location must have validated bot metadata.

---

## 7. Initial location targets

Suggested version 1.0 content:

| ID | Region | Method | Concept |
|---|---|---|---|
| VALLEY_TALLEST_TREE_ROOTS | Green valley | Dig | Beneath roots of tallest tree |
| VALLEY_HOLLOW_TREE | Green valley | Inspect | Note inside hollow trunk |
| RIVER_BRIDGE_UNDERSIDE | River | Inspect | Fragment beneath bridge |
| RIVER_BROKEN_BELL | River ruins | Interact | Ring the silent bell |
| ROCK_LONE_STONE_BASE | Rocky basin | Dig | Buried beside isolated rock |
| RUINS_BACK_OF_STATUE | Ruins | Inspect | Token behind statue |
| RUINS_TURNING_STATUE | Ruins | Interact | Rotate statue toward landmark |
| CABIN_LOOSE_PANEL | Cabin slope | Interact | Open concealed wall panel |
| FOREST_FALLEN_LOG | Forest | Inspect | Clue within split log |
| SNOW_LONE_PINE | Snow ridge | Dig | Buried on sheltered side |
| FINAL_BRIDGE_SHADOW | River | Dig final | Treasure beneath bridge shadow |
| FINAL_CAVE_PANEL | Snow ridge | Interact final | Chest behind concealed panel |
| FINAL_RUINED_TOWER | Ruins | Dig final | Treasure at tower foundation |

These are design placeholders; final locations must match the actual terrain.

---

## 8. Fairness checklist

- [ ] Can the human identify the general region?
- [ ] Does the clue describe a visible fact?
- [ ] Is the intended ambiguity controlled?
- [ ] Is the target reachable?
- [ ] Can the bot reach every candidate and search point?
- [ ] Does the target reset correctly?
- [ ] Is the solution hidden from normal travel distance?
- [ ] Can the target be accidentally triggered?
- [ ] Does the clue remain solvable from every spawn orientation?
- [ ] Is the final action satisfying?
- [ ] Does the bot behaviour look connected to the riddle?

---

## 9. Content budget

Version 1.0 maximum:

- 10–12 intermediate locations
- 3–4 final locations
- 2–3 riddle variants per location
- 3 solution methods
- 3 difficulty profiles

Do not add a second map until these locations are polished and replayable.
