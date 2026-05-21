using TMPro;
using UnityEngine;

public class StageUI : MonoBehaviour
{
    public TMP_Text stageText;

    void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        int world =
            StageManager.Instance.currentWorldIndex + 1;

        int stage =
            StageManager.Instance.currentStageIndex + 1;

        stageText.text =
          "Stage : " +  world + "-" + stage;
    }
}