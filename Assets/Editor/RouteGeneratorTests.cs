using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

public sealed class RouteGeneratorTests
{
    private readonly List<ClueDefinition> createdDefinitions = new List<ClueDefinition>();

    [TearDown]
    public void TearDown()
    {
        foreach (ClueDefinition definition in createdDefinitions)
        {
            Object.DestroyImmediate(definition);
        }

        createdDefinitions.Clear();
    }

    [Test]
    public void SameSeedProducesSameRouteRegardlessOfInputOrder()
    {
        List<ClueDefinition> definitions = CreateValidPool();
        List<ClueDefinition> reversedDefinitions = definitions.AsEnumerable().Reverse().ToList();

        bool firstSucceeded = RouteGenerator.TryGenerate(
            24680,
            definitions,
            out MatchRoute firstRoute,
            out string firstFailure);
        bool secondSucceeded = RouteGenerator.TryGenerate(
            24680,
            reversedDefinitions,
            out MatchRoute secondRoute,
            out string secondFailure);

        Assert.That(firstSucceeded, Is.True, firstFailure);
        Assert.That(secondSucceeded, Is.True, secondFailure);
        CollectionAssert.AreEqual(GetRouteSignature(firstRoute), GetRouteSignature(secondRoute));
    }

    [Test]
    public void GeneratedRouteHasUniqueLocationsAndASeparateFinalClue()
    {
        List<ClueDefinition> definitions = CreateValidPool();

        bool succeeded = RouteGenerator.TryGenerate(
            13579,
            definitions,
            out MatchRoute route,
            out string failureReason);

        Assert.That(succeeded, Is.True, failureReason);
        Assert.That(route.IntermediateClues.Count, Is.EqualTo(3));

        var locationIds = new HashSet<string>(
            route.IntermediateClues.Select(clue => clue.LocationId),
            System.StringComparer.Ordinal);
        Assert.That(locationIds.Count, Is.EqualTo(route.IntermediateClues.Count));
        Assert.That(locationIds.Add(route.FinalTreasure.LocationId), Is.True);
    }

    [Test]
    public void GeneratedRouteNeverUsesThreeIdenticalSearchMethodsInARow()
    {
        List<ClueDefinition> definitions = CreateValidPool();
        Dictionary<string, SearchMethod> methodsByLocation = definitions.ToDictionary(
            definition => definition.LocationId,
            definition => definition.SearchMethod,
            System.StringComparer.Ordinal);

        for (int seed = 1; seed <= 50; seed++)
        {
            bool succeeded = RouteGenerator.TryGenerate(
                seed,
                definitions,
                out MatchRoute route,
                out string failureReason);

            Assert.That(succeeded, Is.True, $"Seed {seed}: {failureReason}");

            SearchMethod[] routeMethods = route.IntermediateClues
                .Select(clue => methodsByLocation[clue.LocationId])
                .Append(methodsByLocation[route.FinalTreasure.LocationId])
                .ToArray();

            for (int index = 2; index < routeMethods.Length; index++)
            {
                bool hasThreeIdenticalMethods =
                    routeMethods[index] == routeMethods[index - 1]
                    && routeMethods[index] == routeMethods[index - 2];
                Assert.That(hasThreeIdenticalMethods, Is.False, $"Seed {seed} repeats a method three times.");
            }
        }
    }

    [Test]
    public void GenerationFailsClearlyWhenNoFinalClueExists()
    {
        var definitions = new List<ClueDefinition>
        {
            CreateDefinition("DIG_A", SearchMethod.Dig, true, false),
            CreateDefinition("INSPECT_A", SearchMethod.Inspect, true, false),
            CreateDefinition("INTERACT_A", SearchMethod.Interact, true, false)
        };

        bool succeeded = RouteGenerator.TryGenerate(
            123,
            definitions,
            out MatchRoute route,
            out string failureReason);

        Assert.That(succeeded, Is.False);
        Assert.That(route, Is.Null);
        StringAssert.Contains("no final-capable definition", failureReason);
    }

    private List<ClueDefinition> CreateValidPool()
    {
        return new List<ClueDefinition>
        {
            CreateDefinition("DIG_A", SearchMethod.Dig, true, false),
            CreateDefinition("DIG_B", SearchMethod.Dig, true, false),
            CreateDefinition("INSPECT_A", SearchMethod.Inspect, true, false),
            CreateDefinition("INSPECT_B", SearchMethod.Inspect, true, true),
            CreateDefinition("INTERACT_A", SearchMethod.Interact, true, false),
            CreateDefinition("INTERACT_B", SearchMethod.Interact, true, false),
            CreateDefinition("FINAL_DIG", SearchMethod.Dig, false, true),
            CreateDefinition("FINAL_INTERACT", SearchMethod.Interact, false, true)
        };
    }

    private ClueDefinition CreateDefinition(
        string locationId,
        SearchMethod searchMethod,
        bool canBeIntermediate,
        bool canBeFinal)
    {
        ClueDefinition definition = ScriptableObject.CreateInstance<ClueDefinition>();
        definition.name = locationId;

        var serializedDefinition = new SerializedObject(definition);
        serializedDefinition.FindProperty("locationId").stringValue = locationId;
        serializedDefinition.FindProperty("searchMethod").enumValueIndex = (int)searchMethod;
        serializedDefinition.FindProperty("regionId").stringValue = "TEST_REGION";
        serializedDefinition.FindProperty("canBeIntermediate").boolValue = canBeIntermediate;
        serializedDefinition.FindProperty("canBeFinal").boolValue = canBeFinal;

        SerializedProperty riddles = serializedDefinition.FindProperty("riddleVariants");
        riddles.arraySize = 2;
        riddles.GetArrayElementAtIndex(0).stringValue = $"{locationId} riddle A";
        riddles.GetArrayElementAtIndex(1).stringValue = $"{locationId} riddle B";
        serializedDefinition.ApplyModifiedPropertiesWithoutUndo();

        createdDefinitions.Add(definition);
        return definition;
    }

    private static string[] GetRouteSignature(MatchRoute route)
    {
        return route.IntermediateClues
            .Select(clue => $"{clue.LocationId}:{clue.RiddleVariantIndex}")
            .Append($"FINAL={route.FinalTreasure.LocationId}:{route.FinalTreasure.RiddleVariantIndex}")
            .ToArray();
    }
}
