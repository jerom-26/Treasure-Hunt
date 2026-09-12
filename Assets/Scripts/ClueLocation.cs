using System;
using UnityEngine;

public sealed class ClueLocation : MonoBehaviour
{
    [Header("Stable Identity")]
    [SerializeField] private string locationId;
    [SerializeField] private SearchMethod searchMethod;

    [Header("Scene References")]
    [SerializeField] private Transform discoveryPresentationRoot;
    [Tooltip("Assign the DigDiscoveryZone, InspectDiscoveryTarget, or future interaction component.")]
    [SerializeField] private MonoBehaviour solutionBehaviour;

    public string LocationId => locationId;
    public SearchMethod SearchMethod => searchMethod;
    public Vector3 DiscoveryPosition => transform.position;
    public Transform DiscoveryPresentationRoot => discoveryPresentationRoot;
    public MonoBehaviour SolutionBehaviour => solutionBehaviour;

    public bool Matches(ClueDefinition definition)
    {
        return definition != null
               && string.Equals(locationId, definition.LocationId, StringComparison.Ordinal)
               && searchMethod == definition.SearchMethod;
    }

    public T GetSolution<T>() where T : MonoBehaviour
    {
        return solutionBehaviour as T;
    }

    private void OnValidate()
    {
        locationId = locationId?.Trim();
    }
}
