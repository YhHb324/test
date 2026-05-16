using TMPro;
using UnityEngine;

public class TotalTeamsUI : MonoBehaviour
{
    public TMP_Text totalText;

    void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        totalText.text =
            DevelopmentManager.Instance.totalTeams
            + "部隊";
    }
}