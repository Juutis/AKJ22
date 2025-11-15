using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimator : MonoBehaviour
{
    [SerializeField]
    private List<Sprite> sprites;

    [SerializeField]
    private float framesPerSecond = 6;

    private SpriteRenderer rend;
    private int spriteIndex;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        Invoke("NextFrame", 1.0f/framesPerSecond);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void NextFrame()
    {
        spriteIndex++;
        if (spriteIndex >= sprites.Count)
        {
            spriteIndex = 0;
        }
        rend.sprite = sprites[spriteIndex];
        Invoke("NextFrame", 1.0f/framesPerSecond);
    }
}
