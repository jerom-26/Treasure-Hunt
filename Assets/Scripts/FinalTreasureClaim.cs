using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public sealed class FinalTreasureClaim : MonoBehaviour
{
    [SerializeField] private GameEndUI gameEndUI;
    [SerializeField] private PlayableDirector claimAnimation;
    [SerializeField] private KeyCode claimKey = KeyCode.E;
    [SerializeField, Min(0f)] private float claimAnimationStartTime = 12.45f;
    [SerializeField, Min(0f)] private float claimAnimationDuration = 0.85f;
    [SerializeField, Min(0f)] private float fallbackWinDelay = 1f;
    [SerializeField] private bool hideTreasureWhenClaimed = true;

    private bool isPlayerInRange;
    private bool isClaimed;

    private void Awake()
    {
        if (claimAnimation == null)
        {
            claimAnimation = GetComponent<PlayableDirector>();
        }

        if (claimAnimation != null)
        {
            claimAnimation.playOnAwake = false;
            claimAnimation.Stop();
            claimAnimation.time = 0d;
            claimAnimation.Evaluate();
        }
    }

    private void Update()
    {
        if (isPlayerInRange && !isClaimed && Input.GetKeyDown(claimKey))
        {
            StartCoroutine(ClaimTreasure());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }

    private IEnumerator ClaimTreasure()
    {
        if (gameEndUI == null)
        {
            gameEndUI = FindFirstObjectByType<GameEndUI>();
        }

        if (gameEndUI == null)
        {
            Debug.LogError(
                "The final treasure could not find a GameEndUI component. "
                + "Add GameEndUI to the active GameManager before testing the final claim.",
                this);
            yield break;
        }

        isClaimed = true;

        float winDelay = fallbackWinDelay;
        if (claimAnimation != null)
        {
            claimAnimation.time = claimAnimationStartTime;
            claimAnimation.Evaluate();
            claimAnimation.Play();
            winDelay = claimAnimationDuration;
        }

        if (winDelay > 0f)
        {
            yield return new WaitForSecondsRealtime(winDelay);
        }

        if (claimAnimation != null)
        {
            claimAnimation.Pause();
        }

        gameEndUI.ShowWin();

        if (hideTreasureWhenClaimed)
        {
            gameObject.SetActive(false);
        }
    }
}
