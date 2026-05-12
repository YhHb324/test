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

    public TMP_Text countText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RefreshAll();
    }


    public void ClickSlot(int index)
    {
        int value = index + 1;

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
            // ON
            if (i < current)
            {
                slots[i].color = onColor;
            }

            // 使用可能
            else if (i < available)
            {
                slots[i].color = offColor;
            }

            // 使用不可
            else
            {
                slots[i].color = disableColor;
            }
        }
    }

    void RefreshAll()
    {
        LiquidSectionUI liquid =
            FindFirstObjectByType<LiquidSectionUI>();

        liquid.Refresh();

        Refresh();

        countText.text = DevelopmentManager.Instance.resourceTeams + "/5";
    }
}
