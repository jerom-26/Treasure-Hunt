using UnityEngine;

[RequireComponent(typeof(Collider))]
public sealed class DigDiscoveryZone : MonoBehaviour
{
    [Header("Discovery Rules")]
    [SerializeField, Min(1)] private int requiredValidDigs = 3;
    [SerializeField, Min(0f)] private float minimumSecondsBetweenCreditedDigs = 0.35f;
    [SerializeField] private LayerMask allowedSurfaceLayers = ~0;

    [Header("Optional Completion Presentation")]
    [Tooltip("A buried clue object to reveal only after the required digs are complete.")]
    [SerializeField] private GameObject discoveredClueVisual;

    [Header("References")]
    [SerializeField] private ClueManager clueManager;

    private Collider discoveryCollider;
    private int validDigCount;
    private float nextCreditableDigTime;
    private bool isComplete;

    public Vector3 DiscoveryPosition => transform.position;
    public int ValidDigCount => validDigCount;
    public bool IsComplete => isComplete;

    private void Awake()
    {
        discoveryCollider = GetComponent<Collider>();

        if (clueManager == null)
        {
            clueManager = FindFirstObjectByType<ClueManager>();
        }

        SetDiscoveredClueVisible(false);

        if (discoveryCollider != null && !discoveryCollider.isTrigger)
        {
            Debug.LogWarning(
                "DigDiscoveryZone works best with Is Trigger enabled on its Collider.",
                this);
        }
    }

    private void Reset()
    {
        Collider zoneCollider = GetComponent<Collider>();
        if (zoneCollider != null)
        {
            zoneCollider.isTrigger = true;
        }
    }

    public static void ReportDig(Vector3 hitPoint, int surfaceLayer)
    {
        DigDiscoveryZone[] zones = FindObjectsByType<DigDiscoveryZone>(FindObjectsSortMode.None);

        foreach (DigDiscoveryZone zone in zones)
        {
            if (zone.TryCreditDig(hitPoint, surfaceLayer))
            {
                break;
            }
        }
    }

    public void ResetDiscovery()
    {
        validDigCount = 0;
        nextCreditableDigTime = 0f;
        isComplete = false;
        SetDiscoveredClueVisible(false);
    }

    private bool TryCreditDig(Vector3 hitPoint, int surfaceLayer)
    {
        if (isComplete
            || discoveryCollider == null
            || !discoveryCollider.enabled
            || Time.unscaledTime < nextCreditableDigTime
            || !IsLayerInMask(surfaceLayer, allowedSurfaceLayers))
        {
            return false;
        }

        if (clueManager == null)
        {
            clueManager = FindFirstObjectByType<ClueManager>();
        }

        if (clueManager == null || !clueManager.IsDigDiscoveryActive(this))
        {
            return false;
        }

        Vector3 closestPoint = discoveryCollider.ClosestPoint(hitPoint);
        if ((closestPoint - hitPoint).sqrMagnitude > 0.0001f)
        {
            return false;
        }

        nextCreditableDigTime = Time.unscaledTime + minimumSecondsBetweenCreditedDigs;
        validDigCount = Mathf.Min(validDigCount + 1, requiredValidDigs);

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.Log(
            $"Dig discovery progress: {validDigCount}/{requiredValidDigs} at {name}.",
            this);
#endif

        if (validDigCount >= requiredValidDigs
            && clueManager.TryCompleteDigDiscovery(this))
        {
            isComplete = true;
            SetDiscoveredClueVisible(true);
        }

        return true;
    }

    private void SetDiscoveredClueVisible(bool isVisible)
    {
        if (discoveredClueVisual != null && discoveredClueVisual != gameObject)
        {
            discoveredClueVisual.SetActive(isVisible);
        }
    }

    private static bool IsLayerInMask(int layer, LayerMask layerMask)
    {
        return (layerMask.value & (1 << layer)) != 0;
    }
}
