using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoSwap : MonoBehaviour
{
    [SerializeField] List<Sprite> headSprites;
    [SerializeField] List<Sprite> bodySprites;
    private SpriteRenderer headSR;
    [SerializeField] SpriteRenderer bodySR;

    private void Start()
    {
        headSR = GetComponent<SpriteRenderer>();
        StartCoroutine(SwapParts());
    }
    IEnumerator SwapParts()
    {
        while (true)
        {
            Color tempColorOne;
            Color tempColorTwo;
            Color finishedColor;

            headSR.sprite = headSprites[Random.Range(0, headSprites.Count)];
            tempColorOne = CalculateAverageColor(headSR.sprite);

            bodySR.sprite = bodySprites[Random.Range(0, bodySprites.Count)];
            tempColorTwo = CalculateAverageColor(bodySR.sprite);

            finishedColor = (tempColorOne + tempColorTwo) / 2;
            headSR.color = finishedColor;
            bodySR.color = finishedColor;

            yield return new WaitForSeconds(.5f);
        }
    }

    private Color CalculateAverageColor(Sprite sprite)
    {
        Texture2D texture = sprite.texture;
        Color[] pixels = texture.GetPixels();

        Color averageColor = Color.black;
        float totalR = 0f;
        float totalG = 0f;
        float totalB = 0f;

        for (int i = 0; i < pixels.Length; i++)
        {
            totalR += pixels[i].r;
            totalG += pixels[i].g;
            totalB += pixels[i].b;
        }

        int pixelCount = pixels.Length;
        averageColor.r = totalR / pixelCount;
        averageColor.g = totalG / pixelCount;
        averageColor.b = totalB / pixelCount;

        return averageColor;
    }
}
