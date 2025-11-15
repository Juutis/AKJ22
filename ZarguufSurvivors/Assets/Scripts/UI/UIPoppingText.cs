using TMPro;
using UnityEngine;

public class UIPoppingText : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private TextMeshProUGUI txtValue;

    public void Show(string value, Color color, Vector2 position)
    {
        /*transform.position = Camera.main.WorldToScreenPoint(
            new Vector3(position.x, position.y, -10f)
        );*/
        transform.position = position;
        txtValue.text = $"{value}";
        txtValue.color = color;
        animator.Play("uiPoppingTextShow");
    }

    public void AfterHide()
    {
        animator.Play("uiPoppingTextIdle");
        UIPoppingTextPool.main.Kill(this);
    }
}
