using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeUI : MonoBehaviour
{
    public static FadeUI Instance;

    [SerializeField]
    Image fadeImage;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartCoroutine(FadeIn(0.5f));
    }

    public IEnumerator FadeOut(float duration)
    {
        float t = 0;

        Color color = fadeImage.color;
        color.a = 0;
        fadeImage.color = color;

        while (t < duration)
        {
            t += Time.deltaTime;

            color.a = t / duration;
            fadeImage.color = color;

            yield return null;
        }

        color.a = 1;
        fadeImage.color = color;
    }

    public IEnumerator FadeIn(float duration)
    {
        float t = 0;

        Color color = fadeImage.color;
        color.a = 1;
        fadeImage.color = color;

        while (t < duration)
        {
            t += Time.deltaTime;

            color.a = 1f - (t / duration);
            fadeImage.color = color;

            yield return null;
        }

        color.a = 0;
        fadeImage.color = color;
    }
}