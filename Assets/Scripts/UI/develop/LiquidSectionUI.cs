using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class LiquidSectionUI : MonoBehaviour
{
    public List<Image> slots;

    public Image tenButtonImage;

    public Sprite[] tenButtonSprites;

    public Color onColor;
    public Color offColor;
    public Color disableColor;

    public TMP_Text countText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RefreshAll();
    }

    // 1〜9クリック
    public void ClickSlot(int index)
    {
        int ones = index + 1;

        int tens =
            DevelopmentManager.Instance.liquidTeams / 10;

        int newValue =
            tens * 10 + ones;

        int total =
            newValue +
            DevelopmentManager.Instance.resourceTeams;

        if (total >
            DevelopmentManager.Instance.totalTeams)
        {
            return;
        }

        DevelopmentManager.Instance.liquidTeams =
            newValue;

        RefreshAll();
    }

    // +10
    public void AddTen()
    {
        int newValue =
            DevelopmentManager.Instance.liquidTeams + 10;

        int total =
            newValue +
            DevelopmentManager.Instance.resourceTeams;

        if (total >
            DevelopmentManager.Instance.totalTeams)
        {
            return;
        }

        DevelopmentManager.Instance.liquidTeams =
            newValue;

        RefreshAll();
    }

    // -10
    public void RemoveTen()
    {
        int newValue =
            DevelopmentManager.Instance.liquidTeams - 10;

        if (newValue < 0)
        {
            return;
        }

        DevelopmentManager.Instance.liquidTeams =
            newValue;

        RefreshAll();
    }

    public void Refresh()
    {
        int current =
            DevelopmentManager.Instance.liquidTeams;

        int ones = current % 10;

        int available =
            DevelopmentManager.Instance.totalTeams -
            DevelopmentManager.Instance.resourceTeams;

        // 1〜9
        for (int i = 0; i < slots.Count; i++)
        {
            int value = i + 1;

            // ON
            if (value <= ones)
            {
                slots[i].color = onColor;
            }

            // 使用可能
            else if (current - ones + value <= available)
            {
                slots[i].color = offColor;
            }

            // 使用不可
            else
            {
                slots[i].color = disableColor;
            }
        }

        // 10単位画像更新
        int tens = current / 10;

        if (
            tenButtonSprites != null &&
            tens < tenButtonSprites.Length
        )
        {
            tenButtonImage.sprite =
                tenButtonSprites[tens];
        }

        // ★追加
        countText.text =
            DevelopmentManager.Instance.liquidTeams
            + "/"
            + DevelopmentManager.Instance.totalTeams;
    }

    void RefreshAll()
    {
        ResourceSectionUI resource =
            FindFirstObjectByType<ResourceSectionUI>();

        resource.Refresh();

        Refresh();

        countText.text = DevelopmentManager.Instance.liquidTeams + "/" + DevelopmentManager.Instance.totalTeams;
    }
}
