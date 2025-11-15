using UnityEngine;
using UnityEngine.UI;

public class UIProgressBar : MonoBehaviour
{
    [SerializeField]
    private Image imgBar;

    public void SetPercentage(float percentage)
    {
        imgBar.fillAmount = percentage;
    }
}
