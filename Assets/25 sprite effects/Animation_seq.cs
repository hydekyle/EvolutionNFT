using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Animation_seq: MonoBehaviour
{
    public float fps = 24.0f;
    public Texture2D[] frames;

    private int frameIndex;
    private Image rendererMy;

    void OnEnable()
    {
        rendererMy = GetComponent<Image>();
        NextFrame();
        InvokeRepeating("NextFrame", 1 / fps, 1 / fps);
    }

    void NextFrame()
    {
        rendererMy.sprite = Sprite.Create(frames[frameIndex], new Rect(0, 0, frames[frameIndex].width, frames[frameIndex].height), Vector2.zero);
        frameIndex = (frameIndex + 0001) % frames.Length;
        if (frameIndex == frames.Length) EndAnimation();
    }

    void EndAnimation()
    {
        gameObject.SetActive(false);
    }
}