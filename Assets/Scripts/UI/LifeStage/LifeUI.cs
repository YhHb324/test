using UnityEngine;
using UnityEngine.UI;

public class LifeUI : MonoBehaviour
{
    public Image[] lamps;

    public Color onColor;

    public Color offColor;

    void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        int life =
            BattleManager.Instance.playerLife;

        for (int i = 0; i < lamps.Length; i++)
        {
            if (i < life)
            {
                lamps[i].color = onColor;
            }
            else
            {
                lamps[i].color = offColor;
            }
        }
    }
}