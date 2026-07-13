# Playtest Plan

## 1. Objective

Determine whether a single human competing against AI rival hunters creates an understandable, fair, replayable treasure-hunt session.

The test is primarily about:

- Riddle clarity
- Environmental searching
- Bot pressure and fairness
- Timer pacing
- Route replayability
- Final treasure climax

---

## 2. Test stages

### Stage A — Human-only mechanics

Test free digging, inspect, interact, route generation, timer, and reset before bots.

### Stage B — One bot

Test whether one bot can complete all locations and create useful pressure.

### Stage C — Multiple bots

Only after one bot is reliable, test two and three rivals.

### Stage D — Release candidate

Test fresh installs, settings, save data, all difficulties, and repeated sessions.

---

## 3. Developer behaviour

- Do not explain clue answers during normal tests.
- Do not defend bot behaviour.
- Record exact moments when players follow, ignore, or mistrust a bot.
- Record when a bot appears to cheat or act randomly.
- Use route and bot seeds to reproduce issues.
- Ask neutral questions after the session.

---

## 4. Metrics

Track:

- Time to understand the goal
- Time to solve each clue
- Human and bot clue credits
- Wrong human digs
- Bot wrong candidates and failed searches
- Human reaction to bot movement
- Timer remaining at final claim
- Winner
- Session length
- Route seed and bot seed
- Restart/rematch request
- Technical failures

---

## 5. Post-session questions

1. What did you think the goal was?
2. Which clue felt best?
3. Which clue felt unfair?
4. Did the rival change where you searched?
5. Did the rival ever appear to cheat?
6. Did the rival ever look completely random?
7. Did losing an intermediate clue make you want to stop?
8. Did you still believe you could win before the final treasure?
9. Did the timer improve or damage the experience?
10. Was the final treasure satisfying?
11. Would you immediately play another randomized session?

---

## 6. Success criteria

The game passes when most external sessions show:

- Goal understood within one minute
- Basic controls understood without developer intervention
- Clues solved through environmental reasoning
- Rival movement changes player decisions
- Bots make visible but believable mistakes
- Player remains engaged after a bot solves a clue
- Final treasure creates urgency
- Session finishes in roughly 8–15 minutes
- Majority request another session
- Different seeds create meaningfully different decisions

---

## 7. Warning signs

Revise if:

- Bot walks directly to every exact answer.
- Bot frequently gets stuck.
- Human simply follows the bot every stage.
- Bot behaviour has no effect on the player.
- Wrong digging feels like empty waiting.
- Timer expires before fair clue progress.
- Riddle wording fits too many landmarks.
- A player cannot recover after losing the first clue.
- Final treasure is found accidentally.
- Reset does not fully restore world and bots.

---

## 8. Test log

```text
Build:
Date:
Tester:
Difficulty:
Number of bots:
Route seed:
Bot seeds:
Selected route:
Session length:
Winner:

Technical issues:
-

Clue 1:
- Solver:
- Solve time:
- Human interpretation:
- Bot behaviour:

Clue 2:
- Solver:
- Solve time:
- Human interpretation:
- Bot behaviour:

Clue 3:
- Solver:
- Solve time:
- Human interpretation:
- Bot behaviour:

Final:
- Winner:
- Discovery time:
- Claim time:
- Timer remaining:

Did the bot appear fair?
-

Memorable pressure or social-reading moments:
-

Frustration:
-

Would replay immediately:
-

Changes for next build:
-
```
