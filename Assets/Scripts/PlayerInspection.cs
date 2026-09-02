using UnityEngine;

public sealed class PlayerInspection : MonoBehaviour
{
    [Header("Inspection")]
    [SerializeField] private Camera inspectionCamera;
    [SerializeField, Min(0.1f)] private float inspectionDistance = 3f;
    [SerializeField] private LayerMask inspectionLayers = ~0;
    [SerializeField] private KeyCode inspectionKey = KeyCode.E;

    private void Awake()
    {
        if (inspectionCamera == null)
        {
            inspectionCamera = GetComponentInChildren<Camera>();
        }

        if (inspectionCamera == null)
        {
            inspectionCamera = Camera.main;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(inspectionKey))
        {
            TryInspect();
        }
    }

    public bool TryInspect()
    {
        if (inspectionCamera == null)
        {
            return false;
        }

        Ray cameraCentreRay = inspectionCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (!Physics.Raycast(
                cameraCentreRay,
                out RaycastHit hit,
                inspectionDistance,
                inspectionLayers,
                QueryTriggerInteraction.Collide))
        {
            return false;
        }

        InspectDiscoveryTarget target = hit.collider.GetComponentInParent<InspectDiscoveryTarget>();
        return target != null && target.TryInspect();
    }
}
