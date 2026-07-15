using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClueUIManager : MonoBehaviour
{
    [Header("Existing Main HUD")]
    public GameObject clueTextHUD;        // Your existing ClueText GameObject
    public Text clueHUDText;          // The TextMeshPro on that HUD

    [Header("Popup Reveal UI")]
    public GameObject cluePopupUI;        // The new panel you created
    public Text cluePopupText;        // The TextMeshPro under CluePopupUI
    public float popupDuration = 4f;      // Seconds until popup hides
   
    void Start()
    {
        // Ensure initial states
        cluePopupUI.SetActive(false);
        clueTextHUD.SetActive(true);
    }

    // Call this when a chest is opened
    public void RevealNewClue(string clue)
    {
        StopAllCoroutines();

        // Hide main HUD
        clueTextHUD.SetActive(false);

        // Show popup
        cluePopupText.text = clue;
        cluePopupUI.SetActive(true);

        // After delay, switch back
        StartCoroutine(SwitchBackToHUD(clue));
    }

    IEnumerator SwitchBackToHUD(string clue)
    {
        yield return new WaitForSeconds(popupDuration);

        cluePopupUI.SetActive(false);

        // Update and show main HUD
        clueHUDText.text = clue;
        clueTextHUD.SetActive(true);
    }

}
