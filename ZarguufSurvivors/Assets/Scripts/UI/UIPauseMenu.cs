using UnityEngine;
using UnityEngine.InputSystem;

public class UIPauseMenu : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    private bool isOpen = false;
    private bool gameIsPaused {get {return LevelManager.main.IsPaused;}}

    public void Show()
    {
        isOpen = true;
        animator.Play("uiPauseMenuShow");
        Time.timeScale = 0f;
        LevelManager.main.IsPaused = true;
    }

    public void AfterHide()
    {
        Time.timeScale = 1f;
        LevelManager.main.IsPaused = false;
        isOpen = false;
    }

    public void Hide()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameIsPaused && !isOpen && Keyboard.current != null && Keyboard.current.pKey.wasPressedThisFrame)
        {
            Show();
        }
        else if (isOpen && Keyboard.current != null && Keyboard.current.pKey.wasPressedThisFrame)
        {
            Hide();
        }
    }
}
