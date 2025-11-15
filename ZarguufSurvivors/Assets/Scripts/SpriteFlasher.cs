using UnityEngine;

public class SpriteFlasher : MonoBehaviour
{
    private SpriteRenderer rend;

    [SerializeField]
    private float flashDuration;

    [SerializeField]
    private Color flashColor;
    private Color origColor;
    private float flashStarted = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        origColor = new Color(0,0,0,0);
    }

    // Update is called once per frame
    void Update()
    {
        var t = (flashStarted + flashDuration - Time.time) / flashDuration;
        if (t >= 0 && t <= 1.0f)
        {
            var color = Color.Lerp(origColor, flashColor, t);
            rend.material.SetColor("_Color", color);
        }
        else
        {
            rend.material.SetColor("_Color", origColor);
        }
    }

    public void Flash()
    {
        flashStarted = Time.time;
    }
}
