using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UIMainMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Mouse.current != null &&
            (
                Mouse.current.leftButton.wasPressedThisFrame ||
                Mouse.current.rightButton.wasPressedThisFrame
            )
        )
        {
            SceneManager.LoadScene("juhoScene");
        }
        else if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
        {
            SceneManager.LoadScene("juhoScene");
        }
    }
}
