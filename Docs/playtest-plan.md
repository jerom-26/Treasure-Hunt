# Playtest Plan

## 1. Objective

Determine whether the game’s core loop is entertaining before investing in additional maps, cosmetics, progression, or large amounts of content.

The test is not primarily about whether players like the art. It is about whether shared riddles, environmental searching, player observation, and randomized routes produce repeatable social tension.

---

## 2. Test groups

Run multiple sessions with:

- Players unfamiliar with the project
- Players who know the map
- Friends using voice chat
- Players without voice chat
- Two-player tests
- Four-player tests
- Target-size tests when stable

Avoid relying only on people who helped develop the game.

---

## 3. Developer behavior

During the test:

- Do not explain a clue unless the session is blocked.
- Do not tell players where to search.
- Do not point out UI elements unless necessary to start.
- Observe silently.
- Record exact moments of confusion, laughter, following, deception, and frustration.
- Ask questions only after the match.

---

## 4. Core observations

Track:

- Time to understand the objective
- Time to solve each stage
- Number of wrong dig attempts
- Number of players reaching the correct region
- Whether players follow one another
- Whether players deliberately mislead others
- Number of players still engaged after losing a clue
- Final treasure claim duration
- Match length
- Rematch requests
- Disconnections or technical failures

---

## 5. Post-match questions

Use short neutral questions:

1. What did you think the goal was?
2. Which clue felt best?
3. Which clue felt unfair or confusing?
4. Did you ever follow another player? Why?
5. Did you ever pretend to know the answer?
6. Did digging feel useful or random?
7. Did you understand when another player solved a clue?
8. Did you feel you could recover after falling behind?
9. Was the final treasure satisfying?
10. Would you play another round immediately?
11. What would stop you from playing again?

Do not ask only “Was it fun?”

---

## 6. Success thresholds

The vertical slice passes when most sessions show:

- Objective understood within one minute
- No developer assistance required for basic controls
- At least one clue solved from environmental reasoning
- At least one moment of social tracking or deception
- Few or no players disengaging after another player solves a clue
- Match length within roughly 8–15 minutes
- Strong final-treasure convergence
- Majority requesting another round
- Different seeds producing meaningfully different behavior

---

## 7. Warning signs

Revise the design if:

- Everyone simply follows the fastest player.
- Players spread out but never pay attention to one another.
- Wrong digging feels like wasted time with no entertainment.
- Riddles are solved by guessing every landmark.
- Players cannot distinguish search methods.
- The minimap solves navigation too directly.
- The same knowledgeable player wins every route.
- Players stop trying after losing the first clue.
- The final treasure is found accidentally.
- Match restarts take too long.

---

## 8. Test log template

```text
Build:
Date:
Players:
Voice chat:
Route seed:
Selected route:
Match length:
Winner:

Technical issues:
- 

Clue 1:
- Solve time:
- Solver:
- Observations:

Clue 2:
- Solve time:
- Solver:
- Observations:

Clue 3:
- Solve time:
- Solver:
- Observations:

Final:
- Discovery time:
- Claim time:
- Observations:

Memorable social moments:
- 

Confusion or frustration:
- 

Players requesting rematch:
- 

Changes for next build:
- 
```

---

## 9. A/B tests

Useful later:

### Player visibility on minimap

- Version A: all players always visible
- Version B: players visible only nearby
- Measure following, deception, and frustration

### Solver reward

- Version A: point and announcement only
- Version B: short private preview of next clue
- Measure whether one player snowballs

### Dig feedback

- Version A: identical feedback until reveal
- Version B: subtle feedback after several correct digs
- Measure random scanning versus satisfying discovery

### Stage timer

- Version A: no forced hint
- Version B: regional hint after a long stall
- Measure fairness and pacing
