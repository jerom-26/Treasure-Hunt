using System;
using System.Collections.Generic;

public static class RouteGenerator
{
    public const int DefaultIntermediateClueCount = 3;
    public const int MaximumConsecutiveSearchMethods = 2;

    public static bool TryGenerate(
        int seed,
        IReadOnlyList<ClueDefinition> definitions,
        int intermediateClueCount,
        out MatchRoute route,
        out string failureReason)
    {
        route = null;
        failureReason = string.Empty;

        if (definitions == null)
        {
            failureReason = "The clue definition collection is missing.";
            return false;
        }

        if (intermediateClueCount < 1)
        {
            failureReason = "A route must contain at least one intermediate clue.";
            return false;
        }

        if (!TryPrepareDefinitions(definitions, out List<ClueDefinition> prepared, out failureReason))
        {
            return false;
        }

        var finalCandidates = new List<ClueDefinition>();
        foreach (ClueDefinition definition in prepared)
        {
            if (definition.CanBeFinal)
            {
                finalCandidates.Add(definition);
            }
        }

        if (finalCandidates.Count == 0)
        {
            failureReason = "The clue pool has no final-capable definition.";
            return false;
        }

        var random = new StableRandom(seed);
        Shuffle(finalCandidates, ref random);

        foreach (ClueDefinition finalCandidate in finalCandidates)
        {
            var intermediateCandidates = new List<ClueDefinition>();
            foreach (ClueDefinition definition in prepared)
            {
                if (definition.CanBeIntermediate
                    && !string.Equals(
                        definition.LocationId,
                        finalCandidate.LocationId,
                        StringComparison.Ordinal))
                {
                    intermediateCandidates.Add(definition);
                }
            }

            if (intermediateCandidates.Count < intermediateClueCount)
            {
                continue;
            }

            Shuffle(intermediateCandidates, ref random);

            var selectedDefinitions = new List<ClueDefinition>(intermediateClueCount);
            var usedCandidates = new bool[intermediateCandidates.Count];
            if (!TrySelectIntermediateDefinitions(
                    intermediateCandidates,
                    finalCandidate,
                    intermediateClueCount,
                    selectedDefinitions,
                    usedCandidates))
            {
                continue;
            }

            var selectedClues = new List<SelectedClue>(intermediateClueCount);
            foreach (ClueDefinition selectedDefinition in selectedDefinitions)
            {
                selectedClues.Add(CreateSelection(selectedDefinition, ref random));
            }

            SelectedClue selectedFinal = CreateSelection(finalCandidate, ref random);
            route = new MatchRoute(seed, selectedClues, selectedFinal);
            return true;
        }

        failureReason =
            $"Could not build {intermediateClueCount} intermediate clues and one final clue "
            + $"without duplicate locations or more than {MaximumConsecutiveSearchMethods} "
            + "identical search methods in a row.";
        return false;
    }

    public static bool TryGenerate(
        int seed,
        IReadOnlyList<ClueDefinition> definitions,
        out MatchRoute route,
        out string failureReason)
    {
        return TryGenerate(
            seed,
            definitions,
            DefaultIntermediateClueCount,
            out route,
            out failureReason);
    }

    private static bool TryPrepareDefinitions(
        IReadOnlyList<ClueDefinition> definitions,
        out List<ClueDefinition> prepared,
        out string failureReason)
    {
        prepared = new List<ClueDefinition>();
        failureReason = string.Empty;
        var locationIds = new HashSet<string>(StringComparer.Ordinal);

        foreach (ClueDefinition definition in definitions)
        {
            if (definition == null)
            {
                continue;
            }

            if (!definition.CanBeIntermediate && !definition.CanBeFinal)
            {
                continue;
            }

            if (string.IsNullOrWhiteSpace(definition.LocationId))
            {
                failureReason = $"Clue definition '{definition.name}' has no location ID.";
                return false;
            }

            if (!locationIds.Add(definition.LocationId))
            {
                failureReason = $"Duplicate clue location ID '{definition.LocationId}' was found.";
                return false;
            }

            if (definition.RiddleVariantCount == 0)
            {
                failureReason =
                    $"Clue definition '{definition.LocationId}' has no riddle variants.";
                return false;
            }

            prepared.Add(definition);
        }

        prepared.Sort((left, right) => string.CompareOrdinal(left.LocationId, right.LocationId));
        return true;
    }

    private static bool TrySelectIntermediateDefinitions(
        IReadOnlyList<ClueDefinition> candidates,
        ClueDefinition finalCandidate,
        int requiredCount,
        List<ClueDefinition> selected,
        bool[] usedCandidates)
    {
        if (selected.Count == requiredCount)
        {
            return CanAppendSearchMethod(selected, finalCandidate.SearchMethod);
        }

        for (int index = 0; index < candidates.Count; index++)
        {
            if (usedCandidates[index])
            {
                continue;
            }

            ClueDefinition candidate = candidates[index];
            if (!CanAppendSearchMethod(selected, candidate.SearchMethod))
            {
                continue;
            }

            usedCandidates[index] = true;
            selected.Add(candidate);
            if (TrySelectIntermediateDefinitions(
                    candidates,
                    finalCandidate,
                    requiredCount,
                    selected,
                    usedCandidates))
            {
                return true;
            }

            selected.RemoveAt(selected.Count - 1);
            usedCandidates[index] = false;
        }

        return false;
    }

    private static bool CanAppendSearchMethod(
        IReadOnlyList<ClueDefinition> selected,
        SearchMethod nextMethod)
    {
        if (selected.Count < MaximumConsecutiveSearchMethods)
        {
            return true;
        }

        for (int offset = 1; offset <= MaximumConsecutiveSearchMethods; offset++)
        {
            if (selected[selected.Count - offset].SearchMethod != nextMethod)
            {
                return true;
            }
        }

        return false;
    }

    private static SelectedClue CreateSelection(
        ClueDefinition definition,
        ref StableRandom random)
    {
        int riddleVariantIndex = random.Next(definition.RiddleVariantCount);
        return new SelectedClue(definition.LocationId, riddleVariantIndex);
    }

    private static void Shuffle<T>(IList<T> items, ref StableRandom random)
    {
        for (int index = items.Count - 1; index > 0; index--)
        {
            int swapIndex = random.Next(index + 1);
            (items[index], items[swapIndex]) = (items[swapIndex], items[index]);
        }
    }

    private struct StableRandom
    {
        private uint state;

        public StableRandom(int seed)
        {
            state = unchecked((uint)seed);
            if (state == 0)
            {
                state = 0x6D2B79F5u;
            }
        }

        public int Next(int exclusiveMaximum)
        {
            if (exclusiveMaximum <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(exclusiveMaximum));
            }

            state ^= state << 13;
            state ^= state >> 17;
            state ^= state << 5;
            return (int)(state % (uint)exclusiveMaximum);
        }
    }
}
