# Eight-Week Production Plan

## Product goal

Release a small commercial single-player Treasure Hunt game in which one human competes against AI rival hunters through a randomized shared clue route.

The release target is intentionally strict. Online multiplayer is postponed. The game must remain extensible, but no networking work is permitted before version 1.0.

---

## Production rules

- Preserve the working prototype through source control before redesign.
- Keep developer debug information; hide it behind a Development/Editor toggle.
- Build one complete loop before producing many clues.
- Human and bots must use the same completion and claim APIs.
- Bots use authored search metadata, not language understanding.
- Use one existing map.
- Do not add a feature unless it directly improves the core session.
- Cut content or polish before extending the eight-week target.
- Complete one reviewed task at a time with Codex.

---

## Milestone 0 — Safety and project cleanup

**Target:** 1–2 development sessions

- [x] Commit the Unity 6.3 upgrade, package state, and current documentation.
- [x] Create the annotated tag `legacy-singleplayer-prototype`.
- [x] Confirm the remote backup exists.
- [x] Record current scenes, scripts, prefabs, packages, and known conflicts.
- [x] Fix accidental `.gitignore` exclusions without committing generated folders.
- [ ] Remove runtime-only misuse of `UnityEditor` after backup.
- [ ] Verify Unity compiles.
- [ ] Add the playable scene to Build Settings or create a safe development copy.
- [ ] Confirm the legacy state can be restored.

**Exit criteria**

- A restorable backup exists locally and remotely.
- The project opens and compiles without new errors.
- The current prototype scene can be built or safely copied.

---

## Week 1 — Free search foundation

### Goal

Replace proximity collection with search actions while preserving the existing clue flow for testing.

- [ ] Create a configurable debug overlay/toggle.
- [ ] Keep clue distance, route IDs, and target visualization available only in Editor/Development mode.
- [ ] Add shovel input and ground raycast.
- [ ] Allow digging on all approved ground layers.
- [ ] Add generic feedback for every valid dig.
- [ ] Add pooled hole/decal visuals.
- [ ] Add one hidden `DigDiscoveryZone`.
- [ ] Reveal one buried intermediate clue after repeated correct digs.
- [ ] Keep old scripts intact until replacement works.

**Exit criteria**

A player can dig anywhere on valid terrain. Wrong digging looks normal. One buried clue can be discovered without a proximity collection prompt.

---

## Week 2 — Three solution types and complete solo route

- [ ] Define one shared clue-completion interface.
- [ ] Implement buried clue solution.
- [ ] Implement hidden-object inspection solution.
- [ ] Implement environmental-interaction solution.
- [ ] Implement final buried treasure discovery and claim.
- [ ] Make every solution report to one central match/clue manager.
- [ ] Create one fixed four-stage route for testing.
- [ ] Add clean stage transitions and current-riddle HUD.

**Exit criteria**

One human can finish:

1. Buried clue
2. Hidden environmental clue
3. Interaction clue
4. Final buried treasure

No release HUD reveals the answer.

---

## Week 3 — Data-driven clues and route generation

- [ ] Create stable location IDs.
- [ ] Create clue definition data assets.
- [ ] Create scene clue-location components.
- [ ] Store riddle variants and search method.
- [ ] Create seeded route generation.
- [ ] Select three intermediate clues and one final location.
- [ ] Prevent duplicates and excessive method repetition.
- [ ] Add route seed and force-route debugging.
- [ ] Create at least six intermediate and two final locations.

**Exit criteria**

Ten consecutive generated sessions produce valid, finishable routes with useful debug logs.

---

## Week 4 — Match, timer, results, and saving

- [ ] Create match-state flow: setup, countdown, active clue, transition, final, results.
- [ ] Add configurable session timer.
- [ ] Add clue credit and competitor standings.
- [ ] Add final winner logic.
- [ ] Define timer-expiry ranking.
- [ ] Add restart with a new seed.
- [ ] Save personal best time and best placement.
- [ ] Add title screen and basic settings.

**Exit criteria**

A complete timed human-only session starts, finishes, records results, and restarts without editor intervention.

---

## Week 5 — Rival bot vertical slice

- [ ] Create a shared `TreasureHunter` identity/action structure.
- [ ] Separate human input from shared gameplay actions.
- [ ] Add one bot controller.
- [ ] Add bot navigation to authored candidate landmarks.
- [ ] Add visible bot dig, inspect, and interact actions.
- [ ] Add clue-specific bot search profiles.
- [ ] Add controlled mistakes and reassessment.
- [ ] Allow the bot to solve intermediate clues through the shared completion API.
- [ ] Allow the bot to claim final treasure and win.
- [ ] Add bot debug state to the development overlay.

**Decision gate**

Continue with bots only if the rival creates understandable pressure and completes routes reliably. If navigation or behaviour remains unstable, reduce the first release to one bot before cutting core quality.

**Exit criteria**

One human and one bot can complete multiple randomized sessions. Either can solve any stage and win the final treasure.

---

## Week 6 — Content and bot difficulty

- [ ] Expand to 10–12 intermediate clue locations.
- [ ] Expand to 3–4 final locations.
- [ ] Add 2–3 riddle variants per location.
- [ ] Add Easy, Normal, and Hard bot profiles.
- [ ] Support one to three bots if stable.
- [ ] Balance route travel and timer values.
- [ ] Prevent bots from appearing omniscient.
- [ ] Add results statistics and rival names/colors.
- [ ] Run external playtests.

**Exit criteria**

Random routes feel different, clue text is fair, and at least two bot difficulties create distinct but believable pressure.

---

## Week 7 — Polish and commercial preparation

- [ ] Improve shovel and discovery feedback.
- [ ] Add clue-solved, timer, final reveal, and winner audio.
- [ ] Improve riddle HUD readability.
- [ ] Add tutorial/control screen.
- [ ] Add pause, volume, and sensitivity settings.
- [ ] Fix navigation, collision, reset, and save bugs.
- [ ] Produce stable standalone builds.
- [ ] Capture screenshots and trailer footage.
- [ ] Finalize store description and capsule-art requirements.

**Exit criteria**

External players can launch, understand, finish, and replay without developer help.

---

## Week 8 — Release candidate

- [ ] Freeze features.
- [ ] Run repeated fresh-install and save tests.
- [ ] Run at least 20 consecutive session resets.
- [ ] Test all route locations and all bot difficulties.
- [ ] Verify release build hides debug information.
- [ ] Profile performance on target hardware.
- [ ] Fix only release-blocking bugs.
- [ ] Prepare final commercial build and store submission.
- [ ] Create version tag and release notes.

**Release criteria**

- No known route blocker
- No repeatable soft lock
- Bots can finish every authored location
- Timer and rankings work
- Save data survives restart
- Debug spoilers are absent from release build
- Sessions reliably finish inside the intended range

---

## Post-release candidates

Only after version 1.0:

- Online multiplayer
- Additional map
- More search methods
- More rival personalities
- Relaxed exploration mode
- Cosmetics and progression
- Local puzzles and climbing clues

---

## Immediate Codex task sequence

1. Finish and verify Milestone 0 backup.
2. Add the development debug-toggle design without removing existing information.
3. Implement free digging in an isolated, reversible way.
4. Add one discovery zone and one buried clue.
5. Test manually in Unity before authorizing the next feature.
