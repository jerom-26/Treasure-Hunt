# Technical Design

## 1. Architecture goals

The codebase should support:

- One authoritative match state
- Multiple search methods using a shared completion contract
- Hand-authored clue locations
- Seeded route generation
- Different riddle variants per match
- Multiplayer synchronization without exposing hidden answers to clients unnecessarily
- Fast reset between rounds
- Easy debugging by route seed and location ID

The first version should use a host-authoritative listen-server model. Dedicated servers are outside the initial scope.

---

## 2. Proposed runtime structure

```text
GameBootstrap
├── SessionManager
├── NetworkAdapter
├── MatchManager
│   ├── MatchState
│   ├── RouteGenerator
│   ├── ClueDatabase
│   └── ResultsTracker
├── WorldClueRegistry
├── UIManager
└── AudioManager
```

### Main responsibilities

#### `SessionManager`

- Host/join/leave
- Lobby state
- Ready checks
- Return to menu
- Session cleanup

#### `MatchManager`

- Starts and ends rounds
- Owns the authoritative route
- Tracks active stage
- Validates clue completion
- Assigns solver credit
- Starts final claim state
- Publishes synchronized match events

#### `RouteGenerator`

- Receives a match seed
- Filters valid locations
- Applies route constraints
- Selects intermediate locations
- Selects final treasure location
- Selects riddle variants
- Returns stable location IDs, not direct scene references

#### `WorldClueRegistry`

- Registers all `ClueLocation` components in the loaded scene
- Resolves stable IDs to scene objects
- Enables only the active target
- Disables hidden presentation for inactive targets
- Reports invalid or duplicate IDs

---

## 3. Data model

### `SearchMethod`

```csharp
public enum SearchMethod
{
    Dig,
    Inspect,
    Climb,
    Interact,
    LocalPuzzle
}
```

### `ClueDefinition`

Recommended as a data asset.

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
}
```

### `ClueLocation`

Placed in the scene.

```csharp
public sealed class ClueLocation : MonoBehaviour
{
    [SerializeField] private string locationId;
    [SerializeField] private SearchMethod searchMethod;
    [SerializeField] private Transform discoveryPresentationRoot;
    [SerializeField] private MonoBehaviour solutionBehaviour;
}
```

The scene component and data asset must share the same stable `locationId`.

### Selected route record

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

## 4. Shared solution contract

Every search method should report through one interface.

```csharp
public interface IClueSolution
{
    string LocationId { get; }
    bool IsAvailable { get; }
    void ActivateForStage();
    void Deactivate();
}
```

A solution should never directly advance the game locally. It submits a completion attempt to `MatchManager`.

```csharp
public interface IClueCompletionReporter
{
    void SubmitCompletionAttempt(
        string locationId,
        ulong playerId,
        CompletionEvidence evidence);
}
```

The host/server validates that:

- The submitted location is the active route location.
- The player is allowed to act.
- The solution-specific condition is complete.
- The stage has not already been solved.

---

## 5. Digging system

## 5.1 Goals

- Ground appears diggable almost everywhere.
- Wrong digging looks legitimate.
- Correct digging requires intentional repeated action.
- The complete terrain mesh does not need real-time deformation.
- Hole visuals remain performant and synchronized only where necessary.

## 5.2 Client flow

1. Player presses shovel input.
2. Local controller performs a ground raycast.
3. Local animation starts immediately for responsiveness.
4. A dig attempt containing hit position, normal, surface ID, and timestamp is sent for validation.
5. Generic particles and sound play for every approved ground hit.
6. A local hole visual is spawned or updated.
7. If the position overlaps the active hidden discovery zone, the server increases progress.
8. Once threshold is reached, the server reveals the clue object globally.

## 5.3 Discovery zone

A `DigDiscoveryZone` contains:

- Stable location ID
- Shape or radius
- Required valid digs
- Minimum time between credited digs
- Allowed surface
- Optional depth stages
- Reveal presentation

Do not give special feedback on the first correct dig. A subtle progressive reveal can begin only after meaningful progress.

## 5.4 Hole visual pooling

Use pooled visuals rather than permanent GameObjects.

A hole record may contain:

- Position
- Rotation aligned to surface normal
- Size/depth stage
- Creation time
- Owner player ID
- Fade time

For the first prototype, hole visuals may be client-side presentation. Only clue discovery progress and final reveal must be authoritative.

---

## 6. Inspect and interaction systems

### Inspect solution

Use a view raycast and close-range validation.

Requirements:

- Target is visually integrated into the environment.
- No prompt appears from far away.
- A small generic “Inspect” prompt may appear only after the player is already looking directly at the intended object.
- The server validates distance, line of sight, and active location ID.

### Interaction solution

Environmental interactions should expose a small state machine:

```text
Inactive
→ ActiveForCurrentClue
→ PlayerInteracting
→ Solved
→ PresentationComplete
```

For multi-step puzzles, the server owns the puzzle state.

---

## 7. Match state

Suggested state machine:

```text
MainMenu
→ Lobby
→ Loading
→ Countdown
→ ActiveClue
→ ClueTransition
→ FinalTreasure
→ Results
→ RematchVote
→ Loading or MainMenu
```

### Authoritative variables

- Match state
- Match seed
- Selected route IDs
- Active clue index
- Solved stage count
- First solver for each stage
- Final treasure state
- Winner
- Round start/end timestamps

### Client presentation events

- Show riddle
- Hide riddle card
- Announce clue solver
- Play global sound
- Reveal intermediate object
- Reveal final chest
- Update results

---

## 8. Route-generation algorithm

### Inputs

- Match seed
- Number of intermediate stages
- All enabled clue definitions
- Region constraints
- Search-method constraints
- Minimum distance
- Maximum estimated travel cost

### Basic process

```text
1. Seed deterministic random generator.
2. Separate intermediate and final candidates.
3. Randomly choose a valid starting clue.
4. Repeatedly choose the next clue from candidates that:
   - are unused,
   - are not too close,
   - do not violate method repetition,
   - improve region diversity,
   - remain within travel budget.
5. Choose a valid final treasure location.
6. Select a riddle variant for every chosen location.
7. Validate the route.
8. Retry with a bounded number of attempts if invalid.
9. Log the final seed and route IDs.
```

### Debug requirements

- Enter a seed manually.
- Force a specific location as the next clue.
- Print the full selected route.
- Teleport development player to a location.
- Visualize all location IDs and regions in editor mode.
- Never include these tools in normal player UI.

---

## 9. Multiplayer message model

The exact networking package can be selected later. Keep gameplay code behind an adapter where practical.

### Client to server

- Ready state
- Movement/input data
- Dig attempt
- Inspect attempt
- Interaction/puzzle action
- Final treasure claim attempt
- Rematch vote

### Server to clients

- Player joined/left
- Match countdown
- Selected route summary needed by clients
- Active riddle
- Stage solved event
- Solver identity
- Discovery presentation
- Final treasure available
- Claim progress/state
- Winner/results
- Round reset

Hidden solution geometry should remain server-side or inactive where feasible. At minimum, clients must not receive an obvious active target marker.

---

## 10. Scene and prefab organization

```text
Assets/
├── Game/
│   ├── Art/
│   ├── Audio/
│   ├── Data/
│   │   ├── Clues/
│   │   └── Maps/
│   ├── Prefabs/
│   │   ├── Player/
│   │   ├── Clues/
│   │   ├── Digging/
│   │   └── UI/
│   ├── Scenes/
│   │   ├── Bootstrap.unity
│   │   ├── MainMenu.unity
│   │   └── Map01.unity
│   ├── Scripts/
│   │   ├── Core/
│   │   ├── Match/
│   │   ├── Networking/
│   │   ├── Clues/
│   │   ├── Digging/
│   │   ├── Player/
│   │     ├── UI/
│   │   └── Editor/
│   └── Tests/
└── ThirdParty/
```

Third-party assets should remain separate from game-owned code and content.

---

## 11. Save data

MVP save data should be minimal:

- Settings
- Player display name
- Input bindings
- Optional cosmetic selection
- Recent route seeds for debugging only

Do not build progression, inventory, currency, or cloud save before gameplay validation.

---

## 12. Performance targets

Initial PC targets:

- Stable 60 FPS on the developer’s current laptop at intended settings
- No unbounded hole GameObject growth
- Pooled particles and hole visuals
- Limited network messages for cosmetic digging
- No per-frame search through all clue locations
- No expensive route generation during active gameplay
- No physics-heavy fully deformable terrain

---

## 13. Testing strategy

### Automated or editor tests

- Route generator never selects duplicate IDs.
- Route generator always ends with a final-capable location.
- Riddle variant index remains valid.
- Duplicate scene IDs are detected.
- Inactive solution cannot complete a stage.
- Only one solver is credited per stage.
- Round reset clears all temporary state.

### Manual network tests

- Host and one client
- Host and multiple clients
- High-latency simulation
- Client disconnect during active clue
- Two simultaneous completion attempts
- Rematch after complete round
- Ten consecutive rounds without editor restart

---

## 14. Technical non-goals

Do not implement in the first version:

- Fully deformable terrain
- Dedicated servers
- Host migration
- Complex anti-cheat
- Cross-platform networking
- Large persistent worlds
- Procedural terrain generation
- User-generated clue scripting
- Physics-based player combat
