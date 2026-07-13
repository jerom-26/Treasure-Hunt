# Rival Bot Design

## 1. Purpose

Bots create the pressure originally intended to come from multiplayer opponents. They must look like rival hunters who interpret, travel, search, make mistakes, and recover.

They do not need natural-language understanding. The riddle is for the human; authored bot metadata provides plausible decisions.

---

## 2. Required player experience

The player should think:

- “That bot may know where it is going.”
- “Should I follow it or trust my own answer?”
- “It is searching the wrong side; I still have time.”
- “It found this clue, but I can still win the treasure.”

The player should not think:

- “The bot knows the exact hidden coordinate.”
- “The bot teleported.”
- “The difficulty just makes it cheat.”
- “The bot is wandering randomly with no relationship to the clue.”

---

## 3. Search profile

Each clue location needs:

- One correct candidate landmark
- One to three plausible wrong candidates where possible
- Navigation anchors for each candidate
- Search points for the clue’s action type
- Candidate plausibility weights
- Minimum and maximum search durations
- Conditions for abandoning a candidate
- Optional reaction positions when observing another competitor

Example:

```text
Location: VALLEY_TALLEST_TREE_ROOTS
Method: Dig

Candidates:
1. Tall valley tree — correct — high plausibility
   Search points: front roots, rear roots, left roots, right roots
2. Dead river tree — wrong — medium plausibility
   Search points: trunk base, river side
3. Cabin tree — wrong — low plausibility
   Search points: front roots, rear roots
```

---

## 4. Bot loop

1. **Reading:** Pause for a difficulty-adjusted delay.
2. **Choosing:** Select a candidate using weights, difficulty, prior failures, and optional observed rival behaviour.
3. **Navigating:** Travel using normal world movement.
4. **Searching:** Perform visible digs, inspections, or interactions.
5. **Reassessing:** If attempts fail, lower that candidate’s confidence and choose again.
6. **Reacting:** Optionally investigate a rival who appears close to solving.
7. **Completing:** Use the same completion and claim APIs as the human.
8. **Transitioning:** Stop current actions when another competitor solves the stage, then process the next clue.

---

## 5. Difficulty design

Difficulty is a parameter profile, not separate bot code.

| Parameter | Easy | Normal | Hard |
|---|---:|---:|---:|
| Correct-candidate bias | Low | Medium | High |
| Reading delay | Long | Medium | Short |
| Search delay | Long | Medium | Short |
| Wrong attempts | Many | Some | Few |
| Rival reaction chance | Low | Medium | High |
| Rival reaction delay | Long | Medium | Short |
| Movement speed | Slightly slow | Player-like | Player-like or slightly fast |

Hard bots must still have a non-zero chance of making a plausible mistake.

---

## 6. Fairness rules

Bots may know authored candidate data, but must not:

- Read the exact hidden world coordinate and go directly there every time
- Ignore terrain and collision
- Complete actions outside valid range
- Skip required dig count or claim duration
- Continue searching an old stage after global progression
- React instantly to hidden player actions
- Receive information the human could not plausibly observe unless clearly defined as difficulty behaviour

---

## 7. Player observation

A bot may react when the player:

- Repeatedly digs in one local area
- Remains near one landmark for a threshold time
- Begins final claim
- Reveals an intermediate clue

Reaction should include delay and uncertainty. The bot may investigate nearby rather than selecting the exact player position.

This feature is optional until basic search behaviour works.

---

## 8. Navigation requirements

- Every candidate needs a reachable anchor.
- Every search point must be reachable and allow the correct animation/action.
- Bots must recover from blocked paths.
- Bots must not permanently body-block the player or clue.
- A development command must validate all route locations for bot reachability.

Start with one bot. Add additional bots only after one bot reliably completes all authored locations.

---

## 9. Debugging

Development overlay should show:

- Bot name and seed
- Current state
- Active clue ID
- Selected candidate ID
- Candidate confidence
- Destination
- Search point index
- Failed attempts
- Reassessment timer

Support restarting with the same route and bot seed to reproduce failures.

---

## 10. Acceptance criteria

A bot is release-ready when it can:

- Complete every clue type
- Visit plausible wrong locations
- Recover from a failed candidate
- Stop immediately when another competitor solves the stage
- Claim the final treasure
- Lose without becoming stuck
- Run 20 consecutive sessions without a route blocker
- Create visible pressure without appearing omniscient
