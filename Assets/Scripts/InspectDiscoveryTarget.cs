using UnityEngine;

[RequireComponent(typeof(Collider))]
public sealed class InspectDiscoveryTarget : MonoBehaviour
{
    [Header("Optional Completion Presentation")]
    [Tooltip("A clue object to reveal only after this target is successfully inspected.")]
    [SerializeField] private GameObject discoveredClueVisual;

    [Header("References")]
    [SerializeField] private ClueManager clueManager;

    private bool isComplete;

    public Vector3 InspectionPosition => transform.position;
    public bool IsComplete => isComplete;

    private void Awake()
    {
        if (clueManager == null)
        {
            clueManager = FindFirstObjectByType<ClueManager>();
        }

        SetDiscoveredClueVisible(false);
    }

    public bool TryInspect()
    {
        if (isComplete)
        {
            return false;
        }

        if (clueManager == null)
        {
            clueManager = FindFirstObjectByType<ClueManager>();
        }

        if (clueManager == null || !clueManager.TryCompleteInspection(this))
        {
            return false;
        }

        isComplete = true;
        SetDiscoveredClueVisible(true);

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.Log($"Inspection clue completed at {name}.", this);
#endif

        return true;
    }

    public void ResetInspection()
    {
        isComplete = false;
        SetDiscoveredClueVisible(false);
    }

    private void SetDiscoveredClueVisible(bool isVisible)
    {
        if (discoveredClueVisual != null && discoveredClueVisual != gameObject)
        {
            discoveredClueVisual.SetActive(isVisible);
        }
    }
}
