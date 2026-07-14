using UnityEngine;

public sealed class PlayerFreeDigging : MonoBehaviour
{
    [Header("Dig Settings")]
    [SerializeField] private Camera diggingCamera;
    [SerializeField, Min(0.1f)] private float diggingDistance = 3f;
    [SerializeField] private LayerMask validGroundLayers;
    [SerializeField, Min(0f)] private float diggingCooldown = 0.5f;

    [Header("Optional Feedback")]
    [SerializeField] private ParticleSystem digParticles;
    [SerializeField] private AudioSource digAudioSource;

    private float nextAllowedDigTime;

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
                validGroundLayers,
                QueryTriggerInteraction.Ignore))
        {
            return;
        }

        nextAllowedDigTime = Time.unscaledTime + diggingCooldown;

        if (digParticles != null && digParticles.gameObject.activeInHierarchy)
        {
            digParticles.Play();
        }

        if (digAudioSource != null && digAudioSource.isActiveAndEnabled && digAudioSource.clip != null)
        {
            digAudioSource.Play();
        }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.Log($"Successful dig at {hit.point} on {hit.collider.name}.", this);
#endif
    }
}
