using System;
using UnityEngine;

[Serializable]
public struct SelectedClue
{
    [SerializeField] private string locationId;
    [SerializeField] private int riddleVariantIndex;

    public string LocationId => locationId;
    public int RiddleVariantIndex => riddleVariantIndex;

    public SelectedClue(string locationId, int riddleVariantIndex)
    {
        this.locationId = locationId;
        this.riddleVariantIndex = riddleVariantIndex;
    }
}
