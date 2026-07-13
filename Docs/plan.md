# Production Plan

## Project goal

Build and validate a small competitive multiplayer treasure-hunt game in Unity 6.3 LTS.

The immediate goal is not a Steam release. It is a repeatable vertical slice where two or more players receive shared riddles, search the world using different methods, advance one global route, and compete for a randomized final treasure.

---

## Working principles

- Prove the gameplay before adding content volume.
- Keep one map until the first map is genuinely replayable.
- Use handcrafted clue locations selected randomly per match.
- Treat multiplayer authority as part of the design, not a late conversion.
- Remove debug assistance that destroys the search fantasy.
- Test with real players after every major milestone.
- Do not polish a mechanic that is not already fun.

---

## Milestone 0 — Project audit and safe backup

**Outcome:** The existing prototype is preserved and its useful systems are identified.

- [ ] Create a source-control repository.
- [ ] Tag or branch the current working prototype as `legacy-singleplayer-prototype`.
- [ ] Record the current Unity version and installed packages.
- [ ] List all current scenes, scripts, prefabs, and third-party assets.
- [ ] Identify which scripts control movement, clues, chests, minimap, and UI.
- [ ] Remove unused duplicate assets only after backup.
- [ ] Create a clean development scene named `VerticalSlice_Map01`.
- [ ] Create a project task board using the backlog document.

**Exit criteria**

- The current build can still be restored.
- The clean scene runs without missing references.
- Every reused system has a known owner script.

---

## Milestone 1 — Single-player search redesign

**Outcome:** The game no longer depends on walking into a clue trigger and pressing E.

### 1.1 Free digging

- [ ] Add shovel equip and use input.
- [ ] Raycast from the player to valid ground.
- [ ] Allow digging on every approved ground surface.
- [ ] Play shovel animation, sound, and dirt particles for every valid dig.
- [ ] Spawn or update a temporary hole/decal visual.
- [ ] Add hidden `DigDiscoveryZone` validation.
- [ ] Require multiple digs before a buried clue appears.
- [ ] Ensure wrong digging gives normal feedback without revealing failure.
- [ ] Limit or recycle old hole visuals for performance.

### 1.2 Remove solution spoilers

- [ ] Remove distance-to-clue debug text from player builds.
- [ ] Remove active clue marker from minimap.
- [ ] Remove proximity “Press E to collect clue” prompts.
- [ ] Hide unused clue targets.
- [ ] Ensure buried objects cannot be seen before discovery.

### 1.3 Add search variety

- [ ] Implement one hidden-object clue.
- [ ] Implement one environmental interaction clue.
- [ ] Keep one buried clue.
- [ ] Implement final buried treasure and claim action.
- [ ] Make all solution types report through one common completion interface.

**Exit criteria**

A single player can complete this route without debug information:

1. Buried clue
2. Hidden clue
3. Interaction clue
4. Final buried treasure

The route is understandable from riddles and environmental observation alone.

---

## Milestone 2 — Data-driven clue authoring

**Outcome:** New clues can be added without rewriting progression code.

- [ ] Create `ClueDefinition` data assets.
- [ ] Create `ClueLocation` scene components.
- [ ] Add search-method enum.
- [ ] Add region, difficulty, allowed-stage, and final-location metadata.
- [ ] Store multiple riddle variants per location.
- [ ] Add stable unique IDs for every location.
- [ ] Build validation warnings for missing targets or empty riddle text.
- [ ] Create editor gizmos visible only during development.
- [ ] Create a debug menu to force a route by location ID.

**Exit criteria**

A designer can create a new valid clue by:

1. Placing/configuring a location
2. Assigning a search method
3. Adding riddle variants
4. Registering it in the content database

No changes to the central match script are required.

---

## Milestone 3 — Random route generator

**Outcome:** Replaying the same map produces different valid hunts.

- [ ] Create seeded route generation.
- [ ] Select a configurable number of intermediate stages.
- [ ] Select one final treasure location.
- [ ] Prevent duplicate locations.
- [ ] Prevent excessive consecutive search-method repetition.
- [ ] Add minimum spacing between consecutive locations.
- [ ] Add region diversity rules.
- [ ] Calculate or estimate route travel cost.
- [ ] Reject invalid route combinations.
- [ ] Log selected route and seed for debugging.
- [ ] Add one-button restart with a new seed.

**Vertical-slice content target**

- [ ] 8 intermediate locations
- [ ] 2 final locations
- [ ] 3 search methods
- [ ] 2 riddle variants per location

**Exit criteria**

Ten consecutive generated matches complete without:

- Duplicate targets
- Broken targets
- Impossible routes
- Exact route repetition
- Unreasonable travel

---

## Milestone 4 — Multiplayer foundation

**Outcome:** Two players can enter one authoritative match and see each other reliably.

- [ ] Choose the networking stack after a small isolated test.
- [ ] Implement host and client connection flow.
- [ ] Spawn player objects correctly.
- [ ] Synchronize movement and facing.
- [ ] Allow only the owning client to control its player.
- [ ] Add player names and colors.
- [ ] Add ready state and match countdown.
- [ ] Add host-authoritative match state.
- [ ] Synchronize route seed and selected location IDs.
- [ ] Add clean session shutdown and return to menu.

**Exit criteria**

Two separate builds can:

- Connect
- Move independently
- See the same selected route
- Start and restart a round
- Disconnect without corrupting the project state

---

## Milestone 5 — Networked clue progression

**Outcome:** The complete treasure hunt works identically for every player.

- [ ] Server tracks the active clue index.
- [ ] Clients receive the same riddle text.
- [ ] Search attempts are validated by the server.
- [ ] First valid solver receives clue credit.
- [ ] The next clue is revealed globally.
- [ ] Unused targets remain inactive.
- [ ] Intermediate discovery presentation is synchronized.
- [ ] Final treasure discovery is synchronized.
- [ ] Final claim is server-authoritative.
- [ ] Winner state is synchronized.
- [ ] Add rematch vote or host restart.

**Exit criteria**

Two players can complete an entire randomized match. Both clients agree on:

- Active clue
- Solver
- Discovered object
- Final treasure state
- Winner

---

## Milestone 6 — First real playtest build

**Outcome:** The game can be tested without the developer explaining every action.

- [ ] Add simple title screen.
- [ ] Add host/join flow.
- [ ] Add control instructions.
- [ ] Add reopen-riddle input.
- [ ] Add stage indicator.
- [ ] Add clue-solved announcement.
- [ ] Add winner/results screen.
- [ ] Add rematch.
- [ ] Add basic settings for volume and sensitivity.
- [ ] Produce a standalone Windows build.
- [ ] Run tests with at least four external players across multiple sessions.
- [ ] Record observations using `docs/playtest-plan.md`.

**Exit criteria**

Players can launch, join, understand, play, finish, and replay without direct developer intervention.

---

## Milestone 7 — Gameplay validation gate

Do not add major content until the following questions have strong answers.

- [ ] Do players understand the goal within one minute?
- [ ] Do players spread out based on different interpretations?
- [ ] Do they watch or follow one another?
- [ ] Are wrong searches still enjoyable?
- [ ] Does solving a clue feel meaningful?
- [ ] Does the next-riddle transition create urgency?
- [ ] Does the final treasure create a scramble?
- [ ] Do players ask for another match?
- [ ] Are match lengths within the target range?
- [ ] Does route randomization prevent immediate memorization?

### Decision

- **Continue:** The social search loop is repeatedly fun.
- **Revise:** The loop works but stalls, feels unfair, or lacks interaction.
- **Stop or redesign:** Players follow markers, search alone without social tension, or do not request another round.

---

## Milestone 8 — Demo content and polish

Begin only after passing Milestone 7.

- [ ] Expand to 18–24 intermediate clue locations.
- [ ] Expand to 5–8 final treasure locations.
- [ ] Add 2–4 riddle variants per location.
- [ ] Add a fourth and fifth search method.
- [ ] Improve player character animation.
- [ ] Improve landmark readability.
- [ ] Add stronger audio.
- [ ] Add results statistics.
- [ ] Add key rebinding and accessibility settings.
- [ ] Improve lobby reliability.
- [ ] Test 4–8 players.
- [ ] Optimize hole visuals, particles, and network traffic.
- [ ] Create trailer-ready match footage only after gameplay is stable.

---

## Recommended development order for the next sessions

### Session 1

- [ ] Back up the current prototype.
- [ ] Remove clue distance and minimap solution marker.
- [ ] Create a clean test area.
- [ ] Add basic shovel input and ground raycast.

### Session 2

- [ ] Make every valid ground hit produce digging feedback.
- [ ] Add one invisible dig discovery zone.
- [ ] Reveal a small clue object after several correct digs.

### Session 3

- [ ] Add one hidden-object location.
- [ ] Route both dig and hidden-object completion into the same clue manager.

### Session 4

- [ ] Add one interaction location.
- [ ] Add final treasure claim.
- [ ] Test the complete four-stage single-player route.

Do not start networking before this small search loop works without debug assistance.

---

## Part-time schedule estimate

This is an estimate, not a deadline.

| Stage | Approximate effort |
|---|---:|
| Backup, audit, and cleanup | 1–3 sessions |
| Free digging and varied search vertical slice | 2–4 weeks |
| Data-driven clues and route generation | 2–4 weeks |
| Two-player networking foundation | 3–6 weeks |
| Full networked match | 3–6 weeks |
| External testing and major revisions | 4–8 weeks |
| Demo content and polish | 2–4 months |

A basic public demo is realistically a multi-month project for a solo developer working part-time. The plan should be shortened by cutting content, not by skipping testing or multiplayer reliability.
