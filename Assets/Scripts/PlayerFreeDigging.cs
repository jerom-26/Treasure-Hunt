using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class PlayerFreeDigging : MonoBehaviour
{
    [Header("Dig Settings")]
    [SerializeField] private Camera diggingCamera;
    [SerializeField, Min(0.1f)] private float diggingDistance = 3f;
    [SerializeField] private LayerMask validGroundLayers;
    [Tooltip("Set this to the full shovel animation duration.")]
    [SerializeField, Min(0f)] private float diggingCooldown = 0.5f;

    [Header("Shovel Animation")]
    [SerializeField] private Animator shovelAnimator;
    [SerializeField] private string shovelDigTrigger = "Dig";
    [Tooltip("Seconds from the animation start until the shovel touches the ground.")]
    [SerializeField, Min(0f)] private float digImpactDelay = 0.25f;

    [Header("Optional Feedback")]
    [SerializeField] private ParticleSystem digParticles;
    [SerializeField] private AudioSource digAudioSource;
    [SerializeField] private GameObject groundMarkPrefab;
    [SerializeField, Min(1)] private int groundMarkPoolSize = 16;
    [SerializeField, Min(0f)] private float groundMarkSurfaceOffset = 0.02f;

    private float nextAllowedDigTime;
    private int shovelDigTriggerHash;
    private readonly Queue<GameObject> groundMarkPool = new Queue<GameObject>();

    private void Awake()
    {
        shovelDigTriggerHash = Animator.StringToHash(shovelDigTrigger);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            TryDig();
        }
    }

    private void TryDig()
    {
        if (diggingCamera == null || Time.unscaledTime < nextAllowedDigTime)
        {
            return;
        }

        Ray cameraCentreRay = diggingCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (!Physics.Raycast(
                cameraCentreRay,
                out RaycastHit hit,
                diggingDistance,
                Physics.DefaultRaycastLayers,
                QueryTriggerInteraction.Ignore)
            || !IsLayerInMask(hit.collider.gameObject.layer, validGroundLayers))
        {
            return;
        }

        nextAllowedDigTime = Time.unscaledTime + Mathf.Max(diggingCooldown, digImpactDelay);

        if (shovelAnimator != null
            && shovelAnimator.isActiveAndEnabled
            && !string.IsNullOrWhiteSpace(shovelDigTrigger))
        {
            shovelAnimator.SetTrigger(shovelDigTriggerHash);
        }

        StartCoroutine(PlayDigImpactFeedback(hit.point, hit.normal, hit.collider.name));
    }

    private IEnumerator PlayDigImpactFeedback(Vector3 hitPoint, Vector3 hitNormal, string hitColliderName)
    {
        if (digImpactDelay > 0f)
        {
            yield return new WaitForSecondsRealtime(digImpactDelay);
        }

        Quaternion surfaceRotation = Quaternion.FromToRotation(Vector3.up, hitNormal);
        Vector3 feedbackPosition = hitPoint + hitNormal * groundMarkSurfaceOffset;

        if (digParticles != null && digParticles.gameObject.activeInHierarchy)
        {
            digParticles.transform.SetPositionAndRotation(feedbackPosition, surfaceRotation);
            digParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            digParticles.Play();
        }

        if (digAudioSource != null && digAudioSource.isActiveAndEnabled && digAudioSource.clip != null)
        {
            digAudioSource.Play();
        }

        PlaceGroundMark(feedbackPosition, surfaceRotation);

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.Log($"Successful dig at {hitPoint} on {hitColliderName}.", this);
#endif
    }

    private void PlaceGroundMark(Vector3 position, Quaternion rotation)
    {
        if (groundMarkPrefab == null)
        {
            return;
        }

        GameObject groundMark;

        if (groundMarkPool.Count < groundMarkPoolSize)
        {
            groundMark = Instantiate(groundMarkPrefab);
        }
        else
        {
            groundMark = groundMarkPool.Dequeue();
        }

        groundMark.transform.SetPositionAndRotation(position, rotation);
        groundMark.SetActive(true);
        groundMarkPool.Enqueue(groundMark);
    }

    private static bool IsLayerInMask(int layer, LayerMask layerMask)
    {
        return (layerMask.value & (1 << layer)) != 0;
    }
}
