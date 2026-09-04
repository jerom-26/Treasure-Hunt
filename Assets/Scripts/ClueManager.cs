using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class ClueManager : MonoBehaviour
{
    public GameObject player;
    public GameObject treasurePrefab;
    public Text clueText;
    public Terrain terrain;
    public GameObject finalTreasurePrefab;

    [Header("Prototype Route")]
    [Tooltip("Ordered clue definitions used until seeded route generation is implemented.")]
    [SerializeField] private List<ClueDefinition> fixedRoute = new List<ClueDefinition>();

    private const float ClueCollectionDistance = 5f;
    private int currentClueIndex = 0;
    private readonly List<ClueDefinition> activeRoute = new List<ClueDefinition>();
    private readonly List<ClueLocation> resolvedClueLocations = new List<ClueLocation>();
    private readonly List<GameObject> spawnedChests = new List<GameObject>();

    [SerializeField] private GameObject clueUI;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
    [SerializeField] private bool enableDistanceDebugLogging = true;
    private const float DistanceDebugLogIntervalSeconds = 1f;
    private float nextDistanceDebugLogTime;
#endif
    float GetChestYOffset() => 0.5f;
    public GameObject currentChest;


    private readonly List<Vector3> clueLocations = new List<Vector3>();
    private Vector3 finalTreasureLocation;
    private ClueUIManager clueUIManager;


    void Start()
    {
        BuildFixedRoute();
        clueUIManager = FindFirstObjectByType<ClueUIManager>();

        if (activeRoute.Count == 0 || clueUIManager == null)
        {
            Debug.LogError("ClueManager requires at least one clue definition and a ClueUIManager.", this);
            enabled = false;
            return;
        }

        clueUIManager.RevealNewClue("First Clue: " + GetRiddleText(activeRoute[currentClueIndex]));
        LockAllChestsExceptCurrent();
    }

    void Update()
    {

        if (currentClueIndex < clueLocations.Count)
        {
            Vector3 cluePosition = clueLocations[currentClueIndex];
            float distance = Vector3.Distance(player.transform.position, cluePosition);

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (enableDistanceDebugLogging && Time.unscaledTime >= nextDistanceDebugLogTime)
            {
                Debug.Log($"Distance to clue {currentClueIndex}: {distance}");
                nextDistanceDebugLogTime = Time.unscaledTime + DistanceDebugLogIntervalSeconds;
            }
#endif

            GameObject activeClueChest = spawnedChests[currentClueIndex];
            if (activeClueChest != null
                && distance < ClueCollectionDistance
                && Input.GetKeyDown(KeyCode.E))
            {
                ShowNextClue();
            }

        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            clueText.enabled = !clueText.enabled;
            clueUI.SetActive(!clueUI.activeSelf);
        }

    }
    void BuildFixedRoute()
    {
        activeRoute.Clear();
        resolvedClueLocations.Clear();
        clueLocations.Clear();
        spawnedChests.Clear();

        Dictionary<string, ClueLocation> locationRegistry = BuildLocationRegistry();

        foreach (ClueDefinition clueDefinition in fixedRoute)
        {
            if (clueDefinition == null)
            {
                continue;
            }

            activeRoute.Add(clueDefinition);

            if (TryResolvePlayableLocation(clueDefinition, locationRegistry, out ClueLocation clueLocation))
            {
                resolvedClueLocations.Add(clueLocation);
                spawnedChests.Add(null);
                clueLocations.Add(clueLocation.DiscoveryPosition);
                continue;
            }

            resolvedClueLocations.Add(null);
            Vector3 randomValidPosition = GetRandomValidPosition();
            GameObject chest = Instantiate(treasurePrefab, randomValidPosition, Quaternion.identity);
            spawnedChests.Add(chest);
            clueLocations.Add(randomValidPosition);

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (enableDistanceDebugLogging)
            {
                Debug.Log($"Prototype fallback chest spawned at {randomValidPosition}.", chest);
            }
#endif
        }

        if (clueLocations.Count > 0)
        {
            finalTreasureLocation = clueLocations[clueLocations.Count - 1];
            currentChest = spawnedChests[0];
        }
    }

    public bool IsDigDiscoveryActive(DigDiscoveryZone discoveryZone)
    {
        ClueLocation activeLocation = GetActiveClueLocation();
        return discoveryZone != null
               && activeLocation != null
               && activeLocation.GetSolution<DigDiscoveryZone>() == discoveryZone;
    }

    public bool TryCompleteDigDiscovery(DigDiscoveryZone discoveryZone)
    {
        if (!IsDigDiscoveryActive(discoveryZone))
        {
            return false;
        }

        ShowNextClue();
        return true;
    }

    public bool IsInspectionActive(InspectDiscoveryTarget inspectionTarget)
    {
        ClueLocation activeLocation = GetActiveClueLocation();
        return inspectionTarget != null
               && activeLocation != null
               && activeLocation.GetSolution<InspectDiscoveryTarget>() == inspectionTarget;
    }

    public bool TryCompleteInspection(InspectDiscoveryTarget inspectionTarget)
    {
        if (!IsInspectionActive(inspectionTarget))
        {
            return false;
        }

        ShowNextClue();
        return true;
    }



    Vector3 GetValidClueLocation(Vector3 position)
    {
        float correctedY = terrain.SampleHeight(position) + GetChestYOffset();
        Vector3 correctedPosition = new Vector3(position.x, correctedY, position.z);

        if (!IsInsideTerrain(correctedPosition) || !IsFlatEnough(correctedPosition))
        {
            Debug.LogWarning($"Position {correctedPosition} is invalid or too steep. Replacing with random.");
            return GetRandomValidPosition();
        }

        return correctedPosition;
    }

    bool IsFlatEnough(Vector3 position, float maxSlope = 30f)
    {
        Vector3 terrainPos = position - terrain.GetPosition();
        Vector2 normalizedPos = new Vector2(
            terrainPos.x / terrain.terrainData.size.x,
            terrainPos.z / terrain.terrainData.size.z
        );

        float steepness = terrain.terrainData.GetSteepness(normalizedPos.x, normalizedPos.y);
        return steepness <= maxSlope;
    }


    bool IsInsideTerrain(Vector3 position)
    {
        Vector3 terrainPos = terrain.GetPosition();
        float terrainWidth = terrain.terrainData.size.x;
        float terrainLength = terrain.terrainData.size.z;

        return position.x >= terrainPos.x && position.x <= terrainPos.x + terrainWidth &&
               position.z >= terrainPos.z && position.z <= terrainPos.z + terrainLength;
    }

    Vector3 GetRandomValidPosition()
    {
        Vector3 terrainPos = terrain.GetPosition();
        float margin = 5f; // Prevent spawning too close to edges

        for (int attempt = 0; attempt < 20; attempt++) // Try 20 times
        {
            float x = Random.Range(terrainPos.x + margin, terrainPos.x + terrain.terrainData.size.x - margin);
            float z = Random.Range(terrainPos.z + margin, terrainPos.z + terrain.terrainData.size.z - margin);
            float y = terrain.SampleHeight(new Vector3(x, 0, z)) + GetChestYOffset();

            Vector3 pos = new Vector3(x, y, z);

            if (IsInsideTerrain(pos) && IsFlatEnough(pos))
            {
                return pos;
            }
        }

        Debug.LogError("Failed to find valid random position after 20 attempts.");
        return new Vector3(terrainPos.x + 50, terrain.SampleHeight(new Vector3(terrainPos.x + 50, 0, terrainPos.z + 50)) + GetChestYOffset(), terrainPos.z + 50); // fallback
    }



    void Shuffle(List<Vector3> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);
            Vector3 temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    void ShowNextClue()
    {
        currentClueIndex++;

        if (currentClueIndex >= activeRoute.Count || currentClueIndex >= clueLocations.Count || currentClueIndex >= spawnedChests.Count)
        {
            clueUIManager.RevealNewClue("Final Clue Solved! The treasure is revealed!");
            RevealTreasure();
            return;
        }

        // Get next clue data
        currentChest = spawnedChests[currentClueIndex];
        Vector3 nextCluePosition = clueLocations[currentClueIndex];
        string nextClueMessage = $"Next Clue: {GetRiddleText(activeRoute[currentClueIndex])}";

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        if (enableDistanceDebugLogging)
        {
            float distance = Vector3.Distance(player.transform.position, nextCluePosition);
            nextClueMessage += $"\n[Debug] Distance: {distance:F2}m";
        }
#endif

        clueUIManager.RevealNewClue(nextClueMessage);

        LockAllChestsExceptCurrent();
    }



    void RevealTreasure()
    {
        int finalIndex = clueLocations.Count - 1;
        GameObject finalStageChest = finalIndex >= 0 && finalIndex < spawnedChests.Count
            ? spawnedChests[finalIndex]
            : null;

        if (finalStageChest != null)
        {
            Destroy(finalStageChest);
        }

        Instantiate(finalTreasurePrefab, finalTreasureLocation, Quaternion.identity);
        clueUIManager.RevealNewClue("🎉 Final Treasure Revealed! Go grab it!");
    }

    private Dictionary<string, ClueLocation> BuildLocationRegistry()
    {
        var registry = new Dictionary<string, ClueLocation>(StringComparer.Ordinal);
        ClueLocation[] sceneLocations = FindObjectsByType<ClueLocation>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None);

        foreach (ClueLocation sceneLocation in sceneLocations)
        {
            if (sceneLocation == null || string.IsNullOrWhiteSpace(sceneLocation.LocationId))
            {
                continue;
            }

            if (!registry.TryAdd(sceneLocation.LocationId, sceneLocation))
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                Debug.LogWarning("Duplicate clue location ID found in the active scene.", sceneLocation);
#endif
            }
        }

        return registry;
    }

    private bool TryResolvePlayableLocation(
        ClueDefinition clueDefinition,
        Dictionary<string, ClueLocation> registry,
        out ClueLocation clueLocation)
    {
        clueLocation = null;

        if (string.IsNullOrWhiteSpace(clueDefinition.LocationId)
            || !registry.TryGetValue(clueDefinition.LocationId, out ClueLocation candidate)
            || !candidate.Matches(clueDefinition))
        {
            return false;
        }

        bool hasSupportedSolution = clueDefinition.SearchMethod switch
        {
            SearchMethod.Dig => candidate.GetSolution<DigDiscoveryZone>() != null,
            SearchMethod.Inspect => candidate.GetSolution<InspectDiscoveryTarget>() != null,
            _ => false
        };

        if (!hasSupportedSolution)
        {
            return false;
        }

        clueLocation = candidate;
        return true;
    }

    private ClueLocation GetActiveClueLocation()
    {
        return currentClueIndex >= 0 && currentClueIndex < resolvedClueLocations.Count
            ? resolvedClueLocations[currentClueIndex]
            : null;
    }

    private static string GetRiddleText(ClueDefinition clueDefinition)
    {
        string riddle = clueDefinition != null ? clueDefinition.GetRiddleVariant(0) : string.Empty;
        return string.IsNullOrWhiteSpace(riddle) ? "Clue text is missing." : riddle;
    }


    void LockAllChestsExceptCurrent()
    {
        for (int i = 0; i < spawnedChests.Count; i++)
        {
            if (spawnedChests[i] == null)
            {
                continue;
            }

            if (i != currentClueIndex)
            {
                spawnedChests[i].SetActive(false); // Hide other chests
            }
            else
            {
                spawnedChests[i].SetActive(true);  // Only show the current clue chest
            }
        }
    }



}
