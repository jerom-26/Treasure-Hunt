using UnityEngine;

public enum SearchMethod
{
    Dig,
    Inspect,
    Interact
}

[CreateAssetMenu(fileName = "ClueDefinition", menuName = "Treasure Hunt/Clue Definition")]
public sealed class ClueDefinition : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string locationId;
    [SerializeField] private SearchMethod searchMethod;
    [SerializeField] private string regionId;

    [Header("Route Rules")]
    [SerializeField, Range(1, 5)] private int difficulty = 1;
    [SerializeField] private bool canBeIntermediate = true;
    [SerializeField] private bool canBeFinal;

    [Header("Player-Facing Riddles")]
    [SerializeField, TextArea(2, 4)] private string[] riddleVariants;

    public string LocationId => locationId;
    public SearchMethod SearchMethod => searchMethod;
    public string RegionId => regionId;
    public int Difficulty => difficulty;
    public bool CanBeIntermediate => canBeIntermediate;
    public bool CanBeFinal => canBeFinal;
    public int RiddleVariantCount => riddleVariants?.Length ?? 0;

    public string GetRiddleVariant(int variantIndex)
    {
        if (riddleVariants == null || riddleVariants.Length == 0)
        {
            return string.Empty;
        }

        int safeIndex = Mathf.Clamp(variantIndex, 0, riddleVariants.Length - 1);
        return riddleVariants[safeIndex];
    }

    private void OnValidate()
    {
        locationId = locationId?.Trim();
        regionId = regionId?.Trim();
        difficulty = Mathf.Clamp(difficulty, 1, 5);
    }
}
