using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class ClueManager : MonoBehaviour
{
    public GameObject player;
    public GameObject treasurePrefab;
    public Text clueText;
    public Terrain terrain;
    public Text cluePrompt;
    public GameObject finalTreasurePrefab;
    private int currentClueIndex = 0;
    private bool nearClue = false;
    private List<GameObject> spawnedChests = new List<GameObject>(); // Store spawned chests

    [SerializeField] private GameObject clueUI;
    float GetChestYOffset() => 0.5f;
    private bool clueCollected = false;
    public GameObject currentChest;


    private List<string> clueDescriptions = new List<string>
    {
        "Find the tallest tree and look beneath it.",
        "A rock stands alone near the shore. Check behind it.",
        "The wooden bridge holds a secret. Look underneath!"
    };

 /*   private List<Vector3> allPossibleClueLocations = new List<Vector3>
    {
        new Vector3(70, 1, 80), new Vector3(50, 0, 60),
        new Vector3(21, 2, 8), new Vector3(150, 1, 80),
        new Vector3(78, 0, 150), new Vector3(41, 1, 165)
    };*/

    private List<Vector3> clueLocations = new List<Vector3>();
    private Vector3 finalTreasureLocation;
    private ClueUIManager clueUIManager;


    void Start()
    {
        AssignRandomClueLocations();
        clueUIManager = FindObjectOfType<ClueUIManager>();
        clueUIManager.RevealNewClue("First Clue: " + clueDescriptions[currentClueIndex]);
        cluePrompt.gameObject.SetActive(false);
        LockAllChestsExceptCurrent(); // Lock all chests except the first one
    }

    void Update()
    {

        if (currentClueIndex < clueLocations.Count)
        {
            Vector3 cluePosition = spawnedChests[currentClueIndex].transform.position;
            float distance = Vector3.Distance(player.transform.position, cluePosition);
            Debug.Log($"Distance to clue {currentClueIndex}: {distance}");

            if (distance < 5f)
            {
                cluePrompt.gameObject.SetActive(true);
                cluePrompt.text = $"Press 'E' to collect clue ({distance:F}m)";
                nearClue = true;
            }
            else
            {
                cluePrompt.gameObject.SetActive(false);
                nearClue = false;
            }

            if (nearClue && Input.GetKeyDown(KeyCode.E) && !clueCollected)
            {
                clueCollected = true;
                ShowNextClue();
                cluePrompt.gameObject.SetActive(false);
                StartCoroutine(ResetClueCollectedAfterDelay());
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                clueUIManager.ShowDigPrompt();
            }
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            clueText.enabled = !clueText.enabled;
            clueUI.SetActive(!clueUI.activeSelf);
        }

    }
    private IEnumerator ResetClueCollectedAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        clueCollected = false;
    }


    void AssignRandomClueLocations()
    {
        clueLocations.Clear();
        spawnedChests.Clear();

        for (int i = 0; i < clueDescriptions.Count; i++)
        {
            Vector3 randomValidPosition = GetRandomValidPosition();

            GameObject chest = Instantiate(treasurePrefab, randomValidPosition, Quaternion.identity);
            spawnedChests.Add(chest);
            clueLocations.Add(randomValidPosition);

            Debug.Log($"Chest Spawned at: {randomValidPosition}");
        }

        finalTreasureLocation = clueLocations[clueLocations.Count - 1];
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

        if (currentClueIndex >= clueDescriptions.Count || currentClueIndex >= clueLocations.Count || currentClueIndex >= spawnedChests.Count)
        {
            clueUIManager.RevealNewClue("Final Clue Solved! The treasure is revealed!");
            RevealTreasure();
            return;
        }

        // Get next clue data
        currentChest = spawnedChests[currentClueIndex];
        Vector3 nextCluePosition = clueLocations[currentClueIndex];
        float distance = Vector3.Distance(player.transform.position, nextCluePosition);

        // Show clue UI
        clueUIManager.RevealNewClue($"Next Clue: {clueDescriptions[currentClueIndex]}\nDistance: {distance:F2}m");

        // Lock other chests
        LockAllChestsExceptCurrent();
    }



    void RevealTreasure()
    {
        // Use the last clue index instead of currentClueIndex (which is now out of bounds)
        int finalIndex = clueLocations.Count - 1;

        // Optional: destroy the last clue chest if it was different
        if (finalIndex < spawnedChests.Count && spawnedChests[finalIndex] != null)
        {
            Destroy(spawnedChests[finalIndex]);
        }

        Vector3 finalPosition = spawnedChests[spawnedChests.Count - 1].transform.position;
        Destroy(spawnedChests[spawnedChests.Count - 1]);
        Instantiate(finalTreasurePrefab, finalPosition, Quaternion.identity);
        clueUIManager.RevealNewClue("🎉 Final Treasure Revealed! Go grab it!");
    }


    void LockAllChestsExceptCurrent()
    {
        for (int i = 0; i < spawnedChests.Count; i++)
        {
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
