using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class ResourceSectionUI : MonoBehaviour
{
    public List<Image> slots;

    public Color onColor;
    public Color offColor;
    public Color disableColor;
    public Color zeroColor;

    public TMP_Text countText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RefreshAll();
    }


    public void ClickSlot(int index)
    {
        int value = index;

        int total =
            DevelopmentManager.Instance.liquidTeams +
            value;

        if (total >
            DevelopmentManager.Instance.totalTeams)
        {
            return;
        }

        DevelopmentManager.Instance.resourceTeams =
            value;

        RefreshAll();
    }

    public void Refresh()
    {
        int current =
            DevelopmentManager.Instance.resourceTeams;

        int available =
            DevelopmentManager.Instance.totalTeams -
            DevelopmentManager.Instance.liquidTeams;

        for (int i = 0; i < slots.Count; i++)
        {
            // 0ボタンは常に固定色
            if (i == 0)
            {
                slots[i].color = zeroColor;
            }
            else if (i <= current)
            {
                slots[i].color = onColor;
            }
            else if (i <= available)
            {
                slots[i].color = offColor;
            }
            else
            {
                slots[i].color = disableColor;
            }
        }

        countText.text = DevelopmentManager.Instance.resourceTeams + "/5";
    }

    public void RefreshAll()
    {
        LiquidSectionUI liquid =
            FindFirstObjectByType<LiquidSectionUI>();

        liquid.Refresh();

        Refresh();

        countText.text = DevelopmentManager.Instance.resourceTeams + "/5";
    }
}
