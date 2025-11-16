using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UIPauseMenu : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    private bool isOpen = false;
    private bool gameIsPaused {get {return LevelManager.main.IsPaused;}}

    [SerializeField]
    private Sprite deathImage;
    [SerializeField]
    private Sprite pauseImage;

    [SerializeField]
    private Image imgFlair;

    [SerializeField]
    private TextMeshProUGUI txtTitle;

    [SerializeField]
    private TextMeshProUGUI txtMessage;
    [SerializeField]
    private TextMeshProUGUI txtAction;

    private bool isDeathScreen = false;

    public void Show()
    {
        isOpen = true;
        animator.Play("uiPauseMenuShow");
        Time.timeScale = 0f;
        txtTitle.text = "Game paused";
        txtMessage.text = "You have paused the game";
        txtAction.text = "Continue";
        imgFlair.sprite = pauseImage;
        LevelManager.main.Pause();
    }

    public void ShowDeathScreen()
    {
        isDeathScreen = true;
        isOpen = true;
        animator.Play("uiPauseMenuShow");
        txtTitle.text = "Game over";
        txtMessage.text = "You have died!";
        txtAction.text = "New game";
        Time.timeScale = 0f;
        imgFlair.sprite = deathImage;
        LevelManager.main.Pause();
    }

    public void AfterHide()
    {
        Time.timeScale = 1f;
        LevelManager.main.Unpause();
        isOpen = false;
        if (isDeathScreen)
        {
            SceneManager.LoadScene("juhoScene");
        }
        isDeathScreen = false;
    }

    public void Hide()
    {
        animator.Play("uiPauseMenuHide");
    }

    private void OnEnable()
    {
        MessageBus.Subscribe<PlayerDiedEvent>(OnPlayerDiedEvent);
    }

    private void OnDisable()
    {
        MessageBus.Unsubscribe<PlayerDiedEvent>(OnPlayerDiedEvent);
    }

    private void OnPlayerDiedEvent(PlayerDiedEvent e)
    {
        ShowDeathScreen();
    }

    // Update is called once per frame
    void Update()
    {
        if (
            isDeathScreen && isOpen && Keyboard.current != null &&
            (
                Keyboard.current.spaceKey.wasPressedThisFrame ||
                Keyboard.current.enterKey.wasPressedThisFrame
            )
        )
        {
            SoundManager.main.PlaySound(GameSoundType.Select);
            Hide();
        }
        else if (!gameIsPaused && !isOpen && Keyboard.current != null && Keyboard.current.pKey.wasPressedThisFrame)
        {
            Show();
        }
        else if (
            isOpen && Keyboard.current != null &&
            (
                Keyboard.current.spaceKey.wasPressedThisFrame ||
                Keyboard.current.enterKey.wasPressedThisFrame ||
                Keyboard.current.pKey.wasPressedThisFrame
            )
        )
        {
            SoundManager.main.PlaySound(GameSoundType.Select);
            Hide();
        }
    }
}
