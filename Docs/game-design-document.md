# Game Design Document

## 1. Document status

**Project:** Treasure Hunt  
**Release mode:** Single-player Rival Hunt  
**Genre:** Competitive environmental treasure hunt / time-attack puzzle race  
**Engine:** Unity 6.3 LTS  
**Target platform:** PC first  
**Players:** One human plus one to three AI rivals  
**Camera:** First-person  
**Session target:** 8–15 minutes  
**Commercial target:** Small premium first release  
**Document version:** 0.2 — single-player bot pivot

![Current prototype clue screen](images/current-prototype/prototype-01.png)

---

## 2. High concept

The player enters a handcrafted island with AI rival treasure hunters. Every competitor is pursuing the same active treasure route.

At the beginning of a session, the game chooses a valid sequence from many authored clue locations. Everyone receives the first riddle. Competitors interpret the environment, travel to likely landmarks, dig on valid terrain, inspect hiding places, and operate environmental objects.

The first competitor to solve an intermediate clue receives credit and advances the hunt globally. The next riddle then becomes active for the player and all bots. After several stages, the final riddle leads to the treasure. The first competitor to uncover and claim the final treasure wins. If the session timer expires first, the match ends and rankings are calculated from progress and clue credit.

### One-sentence pitch

> Race AI treasure hunters across a changing island, solve environmental riddles, and claim the treasure before your rivals or the timer.

### Player fantasy

- Interpret a mysterious riddle.
- Recognize a landmark before the rivals do.
- Search more intelligently than competing hunters.
- Notice where rivals are moving and decide whether to follow them.
- Mislead rivals by checking false locations.
- Recover after losing an intermediate clue.
- Win the final scramble for the treasure.

---

## 3. Design pillars

### 3.1 Read the world, not the release HUD

The environment and riddle must guide normal play. The release build must not show:

- Exact distance to the active solution
- A waypoint over the target
- A minimap marker for a hidden clue
- Hidden discovery-zone boundaries
- A proximity prompt that identifies the answer before discovery

Developer builds may show these through a debug toggle.

### 3.2 Shared progress, individual victory

When the human or a bot solves an intermediate clue, the route advances for everyone. Losing one clue never removes the player from the match. The final treasure determines the winner.

### 3.3 Different hunt every session

The terrain remains handcrafted, but each session may change:

- Selected clue locations
- Route order
- Riddle wording
- Search methods
- Final treasure location
- Bot search decisions and mistakes

Targets are selected from authored locations, not placed at meaningless random coordinates.

### 3.4 Varied environmental searching

Version 1.0 uses three solution types:

- **Dig:** Use the shovel anywhere on valid terrain and discover a buried target through correct positioning and repeated digging.
- **Inspect:** Search a tree hollow, bridge underside, ruined wall, barrel, ledge, or concealed environmental space.
- **Interact:** Ring, rotate, pull, light, or operate the correct environmental object.

Climbing and multi-step local puzzles are post-release candidates.

### 3.5 Pressure without unfair cheating

Bots create urgency, but they must appear fallible. They should visit plausible wrong landmarks, search incorrect positions, and take time to reassess. Difficulty changes decision quality and speed; it must not simply reveal the exact answer immediately.

### 3.6 Future multiplayer compatibility without present multiplayer scope

The first release contains no online networking. Human and bot competitors should nevertheless use shared gameplay actions and result systems so that a future network-controlled hunter can replace a bot controller without redesigning clue logic.

---

## 4. Session structure

1. Main menu
2. Select difficulty and number of rivals
3. Load island
4. Generate route and timer
5. Spawn human and bots
6. Display first riddle to all competitors
7. Search and solve three intermediate clues
8. Reveal final riddle
9. Race to uncover and claim final treasure
10. Show winner, rankings, clue credits, and completion statistics
11. Restart with a new route or return to menu

### Emotional arc

1. **Orientation:** Read the opening riddle.
2. **Separation:** Competitors choose different interpretations.
3. **Suspicion:** Observe rival movement and search behaviour.
4. **Convergence:** Several hunters reach a likely location.
5. **Breakthrough:** One competitor solves the clue.
6. **Reset:** Everyone receives the next riddle.
7. **Escalation:** Timer pressure and rival progress increase urgency.
8. **Final scramble:** All remaining information points toward the treasure.
9. **Payoff:** One competitor claims it.

---

## 5. Core rules

### Intermediate clue

- Exactly one route stage is active.
- Any competitor may attempt the active solution.
- The first valid completion receives clue credit.
- The discovered object is revealed.
- After a short transition, the next riddle becomes active for everyone.
- The session continues regardless of who solved the clue.

### Final treasure

- The final location requires discovery plus a short claim action.
- The first competitor to complete the claim wins.
- If a bot claims the treasure, the main competitive session ends.
- A future relaxed mode may allow the player to continue, but it is not required for version 1.0.

### Timer

Initial balancing targets:

- Easy: 18 minutes
- Normal: 12 minutes
- Hard: 8 minutes

If time expires without a final claim:

1. Rank by furthest active-stage progress.
2. Break ties by number of intermediate clues solved.
3. Break remaining ties by proximity or validated progress at the active solution.

These values must be tested rather than treated as permanent.

### Scoring and results

The final treasure winner always ranks first. The results screen records:

- Final position
- Treasure winner
- Clues solved by each competitor
- Session time
- Wrong dig attempts
- Distance travelled
- Personal best result

Persistent currency and progression are outside version 1.0.

---

## 6. Route and clue system

### Clue location

Each authored location contains:

- Stable location ID
- Region
- Landmark description
- Search method
- Exact human completion condition
- Riddle variants
- Difficulty rating
- Allowed stage usage
- Bot search profile
- Reset behaviour

### Initial route format

- Three intermediate clues
- One final treasure
- No duplicate location
- No excessive repetition of the same search method
- Reasonable travel distance
- Different regions where possible

### Content targets

#### Development vertical slice

- 6 intermediate locations
- 2 final locations
- 3 search methods
- 1 human and 1 bot
- 2 riddle variants per location

#### Version 1.0

- 10–12 intermediate locations
- 3–4 final locations
- 3 search methods
- 1–3 bots
- 2–3 riddle variants per location

---

## 7. Digging

The player can dig on every approved ground surface. The game should not display “Press E to dig near the clue.”

Every valid dig provides:

- Shovel animation
- Dirt particles
- Impact sound
- Ground mark or shallow hole visual

Wrong digs must look legitimate. Correct digs contribute to hidden discovery progress. The first correct hit should not reveal correctness through a unique sound or UI signal.

Version 1.0 uses simulated holes, decals, or pooled meshes rather than fully deformable terrain.

Bots use the same logical dig action and discovery rules. Their search controller selects authored search points and performs visible digging there.

---

## 8. Bots

Bots are rival treasure hunters, not enemies and not followers.

They do not parse the riddle text. Each active clue supplies hidden search metadata containing plausible candidate landmarks and search points. A bot:

1. Waits briefly as though reading.
2. Chooses a candidate interpretation.
3. Navigates to the candidate.
4. Searches using the required action.
5. Reassesses after enough failed attempts.
6. May react to visible player confidence.
7. Submits completion through the same system used by the human.

### Difficulty

**Easy**

- Slower movement and search pace
- Higher chance of wrong candidate
- Longer reassessment delays
- Rarely reacts to the player

**Normal**

- Balanced candidate selection
- Moderate search speed
- Occasional reaction to other hunters

**Hard**

- Better candidate weighting
- Faster, but still human-readable, search behaviour
- More likely to investigate where another hunter appears confident
- Still makes controlled mistakes

Bots must not teleport, instantly complete hidden targets, or receive invisible speed boosts that feel impossible.

See [Rival Bot Design](bot-design.md) for implementation detail.

---

## 9. Map and UI

### Map

Use the existing low-poly island and landmarks. The map should be compact enough for repeated 8–15 minute sessions.

The minimap may show:

- Terrain outline
- Major permanent landmarks
- Human player
- Rival positions, subject to playtesting

It must not reveal active solutions.

### Required HUD

- Current riddle
- Current stage
- Session timer
- Rival status or compact standings
- Clue-solved event feed
- Final claim progress

### Development HUD

Behind a debug toggle:

- Active location ID
- Distance to active target
- Route seed and route IDs
- Discovery-zone radius
- Valid dig count
- Bot state and selected candidate

Debug information is required for development but disabled in normal release builds.

---

## 10. Scope

### Must have for version 1.0

- One map
- One human player
- One to three bots
- Three intermediate clues and one final treasure per session
- Free digging on valid terrain
- Inspect and interact solution types
- Authored clue locations and riddle variants
- Randomized valid routes
- Session timer
- Easy, Normal, and Hard rival settings
- Winner, rankings, restart, and saved personal records
- Settings for volume and mouse sensitivity
- Debug tools separated from release UI

### Explicitly outside version 1.0

- Online multiplayer
- Local split-screen multiplayer
- Second map
- Combat
- Survival meters
- Crafting or inventory progression
- Story campaign
- Built-in voice chat
- Ranked mode
- Fully deformable terrain
- AI language-model riddle solving
- Procedural terrain
- User-generated clues
- Dedicated servers

---

## 11. Commercial and validation goals

The game is worth releasing when external tests show:

- New players understand the objective within one minute.
- Players can solve clues without developer explanation.
- Rival movement changes player decisions.
- Players feel pressure without believing bots are cheating.
- Losing an intermediate clue still leaves hope of winning.
- Random routes produce meaningfully different sessions.
- The final treasure creates a clear climax.
- Most testers request another session.
- A complete session reliably finishes inside the target time.

The project must cut features rather than expand beyond the first-release schedule.
