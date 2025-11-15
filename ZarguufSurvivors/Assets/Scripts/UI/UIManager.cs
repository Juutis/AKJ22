using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager main;
    void Awake()
    {
        main = this;
    }

    public void ShowPoppingText(string value, Color color, Vector2 position)
    {
        UIPoppingText poppingText = UIPoppingTextPool.main.Get();
        poppingText.Show(value, color, position);
    }
}
