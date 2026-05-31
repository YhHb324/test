using System.Collections;
using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    public static BackgroundManager Instance;

    [Header("Far")]
    public Transform[] farLayers;

    [Header("Middle")]
    public Transform[] middleLayers;

    [Header("Near")]
    public Transform[] nearLayers;

    [Header("Layer Heights")]
    public float farHeight = 10f;
    public float middleHeight = 10f;
    public float nearHeight = 10f;

    [Header("Scroll Speeds")]
    public float farSpeed = 0.1f;
    public float middleSpeed = 0.2f;
    public float nearSpeed = 0.3f;

    [Header("Scroll Multiplier")]
    public float multi;

    bool scrolling = true;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (!scrolling)
            return;

        ScrollLayer(farLayers, farSpeed, farHeight);
        ScrollLayer(middleLayers, middleSpeed, middleHeight);
        ScrollLayer(nearLayers, nearSpeed, nearHeight);
    }

    void ScrollLayer(
    Transform[] layers,
    float speed,
    float height)
    {
        foreach (Transform layer in layers)
        {
            layer.position +=
                Vector3.down * speed * Time.deltaTime;

            // 一番下へ抜けたら一番上へ
            if (layer.position.y <= -height * 2f)
            {
                layer.position +=
                    Vector3.up * height * 3f;
            }

            // 敗北演出用
            if (layer.position.y >= height * 2f)
            {
                layer.position +=
                    Vector3.down * height * 3f;
            }
        }
    }

    public void PlayWinScroll()
    {
        StartCoroutine(
            ScrollBurst(
                1f,
                multi
            ));
    }

    public void PlayLoseScroll()
    {
        StartCoroutine(
            ScrollBurst(
                -1f,
                multi
            ));
    }

    IEnumerator ScrollBurst(
        float direction,
        float multiplier)
    {
        scrolling = false;

        float timer = 0f;

        while (timer < 1f)
        {
            timer += Time.deltaTime;

            ScrollLayer(
                farLayers,
                farSpeed * multiplier * direction,
                farHeight);

            ScrollLayer(
                middleLayers,
                middleSpeed * multiplier * direction,
                middleHeight);

            ScrollLayer(
                nearLayers,
                nearSpeed * multiplier * direction,
                nearHeight);

            yield return null;
        }

        scrolling = true;
    }
}