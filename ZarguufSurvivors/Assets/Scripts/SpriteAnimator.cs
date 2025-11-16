using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimator : MonoBehaviour
{
    [SerializeField]
    private List<Sprite> sprites;

    [SerializeField]
    private float minFramesPerSecond = 6;

    [SerializeField]
    private float maxFramesPerSecond = 8;
    private float frameDuration;

    private SpriteRenderer rend;
    private int spriteIndex;

    [SerializeField]
    private Transform sortRoot;

    [SerializeField]
    private bool loop = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        spriteIndex = 0;
        rend.sprite = sprites[0];
        frameDuration = Random.Range(1.0f / maxFramesPerSecond, 1.0f / minFramesPerSecond);
        CancelInvoke("NextFrame");
        Invoke("NextFrame", Random.Range(0.0f, frameDuration));
    }

    void OnEnable()
    {
        rend = GetComponent<SpriteRenderer>();
        spriteIndex = 0;
        rend.sprite = sprites[0];
        frameDuration = Random.Range(1.0f / maxFramesPerSecond, 1.0f / minFramesPerSecond);
        CancelInvoke("NextFrame");
        Invoke("NextFrame", Random.Range(0.0f, frameDuration));
    }

    public void Init(List<Sprite> sprites)
    {
        this.sprites = sprites;
    }

    // Update is called once per frame
    void Update()
    {
        if (sortRoot != null)
        {
            rend.sortingOrder = -(int)(sortRoot.position.y * 100);
        }
    }

    public void NextFrame()
    {
        spriteIndex++;
        if (spriteIndex >= sprites.Count)
        {
            if (loop)
            {
                spriteIndex = 0;
            }
            else
            {
                spriteIndex = sprites.Count - 1;
            }
        }
        rend.sprite = sprites[spriteIndex];
        Invoke("NextFrame", frameDuration);
    }
}
