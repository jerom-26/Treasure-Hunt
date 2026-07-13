# Technical Design

## 1. Technical goals

The architecture must support the first-release single-player Rival Hunt while keeping clear seams for future online multiplayer.

It should provide:

- One authoritative local match manager
- One route shared by the human and all bots
- Multiple solution types through a common completion contract
- Human and bot competitors using the same gameplay actions
- Seeded authored-route generation
- Session timer, results, and reset
- Bot navigation and believable search behaviour
- Development debug information separated from release UI

Do not add networking abstractions that create unnecessary complexity. Preserve stable IDs, event-driven state changes, and controller separation so networking can be added later.

---

## 2. Proposed runtime structure

```text
GameBootstrap
├── LocalSessionController
├── MatchManager
│   ├── MatchState
│   ├── RouteGenerator
│   ├── MatchTimer
│   └── ResultsTracker
├── WorldClueRegistry
├── CompetitorRegistry
├── DebugOverlay
├── UIManager
└── AudioManager

TreasureHunter
├── HunterIdentity
├── HunterActionController
├── HumanHunterBrain OR BotHunterBrain
├── CharacterMovement
└── Navigation/Animation presentation
```

### `MatchManager`

Owns:

- Current match state
- Route seed and selected route
- Active clue index
- First solver per stage
- Timer
- Final treasure state
- Rankings and winner
- Round reset

The first release uses a local authoritative implementation. Gameplay code should submit attempts to `MatchManager` rather than advancing stages directly.

### `WorldClueRegistry`

- Registers scene clue locations by stable ID
- Detects duplicate or missing IDs
- Resolves route IDs to scene objects
- Activates only the current solution
- Resets all location presentation between sessions

### `CompetitorRegistry`

- Registers the human and bots
- Provides stable competitor IDs
- Tracks clue credit, state, and placement
- Allows match systems to treat all hunters consistently

---

## 3. Core data model

```csharp
public enum SearchMethod
{
    Dig,
    Inspect,
    Interact
}
```

```csharp
[CreateAssetMenu(menuName = "Treasure Hunt/Clue Definition")]
public sealed class ClueDefinition : ScriptableObject
{
    public string locationId;
    public SearchMethod searchMethod;
    public string regionId;
    public int difficulty;
    public bool canBeIntermediate;
    public bool canBeFinal;
    [TextArea] public string[] riddleVariants;
    public BotSearchProfile botSearchProfile;
}
```

```csharp
public sealed class ClueLocation : MonoBehaviour
{
    [SerializeField] private string locationId;
    [SerializeField] private SearchMethod searchMethod;
    [SerializeField] private Transform discoveryPresentationRoot;
    [SerializeField] private MonoBehaviour solutionBehaviour;
}
```

```csharp
[System.Serializable]
public struct SelectedClue
{
    public string locationId;
    public int riddleVariantIndex;
}
```

```csharp
[System.Serializable]
public sealed class MatchRoute
{
    public int seed;
    public List<SelectedClue> intermediateClues;
    public SelectedClue finalTreasure;
}
```

---

## 4. Competitor architecture

The human and bots must share identity and gameplay actions.

```csharp
public sealed class TreasureHunter : MonoBehaviour
{
    public HunterIdentity Identity { get; }
    public HunterActionController Actions { get; }
    public IHunterBrain Brain { get; }
}
```

```csharp
public interface IHunterBrain
{
    void Initialize(TreasureHunter hunter);
    void SetActiveClue(ClueDefinition clue, ClueLocation location);
    void TickBrain();
    void Stop();
}
```

Implementations:

- `HumanHunterBrain`: converts player input into movement and search actions.
- `BotHunterBrain`: selects candidates, navigates, and requests the same search actions.

Shared actions include:

- Move/request movement target
- Dig
- Inspect
- Interact
- Submit clue completion
- Claim final treasure

No bot-only method should directly solve a clue without using the shared validation path.

---

## 5. Shared completion contract

```csharp
public interface IClueSolution
{
    string LocationId { get; }
    bool IsActive { get; }
    void ActivateForStage();
    void DeactivateAndReset();
}
```

```csharp
public interface IClueAttemptReceiver
{
    CompletionResult SubmitAttempt(
        string locationId,
        string competitorId,
        CompletionEvidence evidence);
}
```

`MatchManager` validates:

- The submitted location is active.
- The competitor is registered.
- The stage is still unresolved.
- The solution-specific condition is complete.
- Final treasure claim requirements are satisfied.

The first valid completion gets credit.

---

## 6. Digging system

### Human flow

1. Human presses shovel input.
2. Camera/controller raycasts to an approved ground layer.
3. Animation and generic effects play.
4. A dig attempt is sent to the active solution system.
5. A pooled hole/decal visual is created or updated.
6. If the point is inside the active discovery zone and cooldown rules pass, progress increases.
7. At the required progress, `MatchManager` receives a valid completion.

### Bot flow

1. Bot brain chooses an authored search point.
2. Navigation places the bot within action range.
3. Bot faces the target and calls the same dig action.
4. The same ground and discovery validation runs.

### `DigDiscoveryZone`

Contains:

- Location ID
- Shape/radius
- Required valid digs
- Minimum interval between credited digs
- Allowed surface/layer
- Reveal stages
- Reset method

Wrong digs receive normal presentation. Correctness is not revealed immediately.

Use pooled visuals; do not deform the entire terrain mesh.

---

## 7. Inspect and interact systems

### Inspect

- Uses range, view direction, and target validation.
- A generic prompt may appear only when directly looking at an inspectable object.
- The prompt must not identify that the object is the active clue before the player has located it.
- Bots navigate to authored inspect positions and call the same inspect action.

### Interact

- Uses an environmental object with a small state machine.
- Bots navigate to the authored interaction anchor.
- Multi-step puzzles are outside version 1.0.

---

## 8. Bot data and state machine

### `BotSearchProfile`

Each clue defines plausible search behaviour:

```csharp
[CreateAssetMenu(menuName = "Treasure Hunt/Bot Search Profile")]
public sealed class BotSearchProfile : ScriptableObject
{
    public BotCandidate[] candidates;
    public float readDelayMin;
    public float readDelayMax;
}
```

```csharp
[System.Serializable]
public sealed class BotCandidate
{
    public string candidateId;
    public TransformReference navigationAnchor;
    public SearchPointReference[] searchPoints;
    public bool isCorrectCandidate;
    public float plausibilityWeight;
}
```

Use stable IDs or scene-resolved references rather than storing unsafe scene Transform references directly in reusable assets.

### Bot states

```text
Inactive
→ Reading
→ ChoosingCandidate
→ Navigating
→ Searching
→ Reassessing
→ ReactingToCompetitor (optional)
→ StageSolvedTransition
→ FinalClaim
→ Finished
```

Difficulty parameters control:

- Candidate weighting
- Movement speed within fair limits
- Search delay
- Number of failed attempts before reassessment
- Chance of reacting to another hunter
- Reaction delay

The bot must remain deterministic enough to reproduce bugs using route seed plus bot seed.

---

## 9. Match state

```text
MainMenu
→ SessionSetup
→ Loading
→ Countdown
→ ActiveClue
→ ClueTransition
→ FinalTreasure
→ Results
→ Restart or MainMenu
```

Authoritative local variables:

- Match seed
- Bot seeds
- Selected route
- Active clue index
- Solvers and clue points
- Remaining time
- Final claim state
- Winner and rankings

---

## 10. Route generation

Inputs:

- Match seed
- Number of stages
- Enabled clue definitions
- Search-method constraints
- Region constraints
- Travel constraints
- Final-capable locations

Process:

1. Seed deterministic random generator.
2. Filter intermediate and final candidates.
3. Select an opening clue.
4. Select additional valid clues while preventing duplicates and excessive repetition.
5. Select a final treasure location.
6. Choose riddle variants.
7. Validate that every selected location has a bot search profile.
8. Log seed and IDs in development mode.
9. Retry a bounded number of times if invalid.

---

## 11. Debug tools

Required in Editor and Development Builds:

- Debug overlay toggle, suggested key `F3`
- Active route and seed
- Active location ID
- Distance to active solution
- Discovery-zone visualization
- Dig progress
- Bot state, candidate, destination, and seed
- Force clue/location
- Teleport development player
- Restart with same/new seed

Release builds must disable or omit these features.

Avoid per-frame console spam. Prefer an overlay and throttled logging.

---

## 12. Future online multiplayer seam

Do not implement networking now. Preserve these properties:

- Stable IDs for competitors and locations
- Match state changed only through `MatchManager`
- Search attempts submitted rather than directly changing stage state
- Human input separated from shared actions
- Bots separated through `IHunterBrain`
- Results driven by competitor IDs
- Route reproducible from seed

A future network authority can replace the local authority. Do not create network packages, RPCs, lobbies, or synchronization code in version 1.0.

---

## 13. Save data

Version 1.0 saves only:

- Settings
- Difficulty preference
- Best time
- Best placement
- Basic aggregate statistics

No inventory, currency, unlock tree, or cloud progression is required.

---

## 14. Testing

Automated/editor tests should cover:

- Route has no duplicate IDs.
- Route always contains a final-capable location.
- Every selected location has riddle and bot metadata.
- Inactive clue cannot complete.
- Only one competitor receives first-solver credit.
- Timer expiry ranks competitors consistently.
- Reset clears clue, bot, timer, and presentation state.
- Debug information is disabled for release configuration.

Manual tests should cover:

- Human can finish every location.
- Bot can finish every location.
- Bot can recover from a wrong candidate.
- Human and bot simultaneous completion.
- Bot wins final treasure.
- Player wins after losing earlier clues.
- Repeated sessions without editor restart.
