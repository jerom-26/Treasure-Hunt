using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public sealed class GameEndUI : MonoBehaviour
{
    [Header("Result Screen")]
    [SerializeField] private GameObject resultPanel;

    [Header("Win Behaviour")]
    [SerializeField] private bool pauseGameplayOnWin = true;
    [SerializeField] private bool unlockCursorOnWin = true;

    private bool isShowing;

    private void Awake()
    {
        if (resultPanel != null)
        {
            BindResultButtons();
            resultPanel.SetActive(false);
        }
    }

    public void ShowWin()
    {
        if (isShowing)
        {
            return;
        }

        if (resultPanel == null)
        {
            Debug.LogError(
                "GameEndUI has no Result Panel assigned. Assign the win panel in the Inspector.",
                this);
            return;
        }

        isShowing = true;
        resultPanel.SetActive(true);

        DisablePlayerGameplay();

        if (pauseGameplayOnWin)
        {
            Time.timeScale = 0f;
        }

        if (unlockCursorOnWin)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;

        Scene activeScene = SceneManager.GetActiveScene();
        if (activeScene.buildIndex < 0)
        {
            Debug.LogError(
                $"Cannot restart {activeScene.name} because it is not in Build Settings.",
                this);
            return;
        }

        SceneManager.LoadScene(activeScene.buildIndex);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.Log("Quit requested. Application.Quit only closes a built game.", this);
#endif
    }

    private void DisablePlayerGameplay()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning(
                "GameEndUI could not find an object tagged Player. "
                + "The win screen will open, but player scripts were not disabled.",
                this);
            return;
        }

        foreach (MonoBehaviour gameplayBehaviour in player.GetComponents<MonoBehaviour>())
        {
            gameplayBehaviour.enabled = false;
        }
    }

    private void BindResultButtons()
    {
        foreach (Button button in resultPanel.GetComponentsInChildren<Button>(true))
        {
            if (button.onClick.GetPersistentEventCount() > 0)
            {
                continue;
            }

            if (button.name == "RestartButton")
            {
                button.onClick.AddListener(RestartGame);
            }
            else if (button.name == "QuitButton")
            {
                button.onClick.AddListener(QuitGame);
            }
        }
    }
}
