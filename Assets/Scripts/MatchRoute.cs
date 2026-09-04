using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public sealed class MatchRoute
{
    [SerializeField] private int seed;
    [SerializeField] private List<SelectedClue> intermediateClues;
    [SerializeField] private SelectedClue finalTreasure;

    public int Seed => seed;
    public IReadOnlyList<SelectedClue> IntermediateClues => intermediateClues;
    public SelectedClue FinalTreasure => finalTreasure;

    public MatchRoute(
        int seed,
        IEnumerable<SelectedClue> intermediateClues,
        SelectedClue finalTreasure)
    {
        this.seed = seed;
        this.intermediateClues = new List<SelectedClue>(intermediateClues);
        this.finalTreasure = finalTreasure;
    }
}
