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
        string worldName =
            StageManager.Instance.CurrentWorld().worldName;

        int stage =
            StageManager.Instance.currentStageIndex + 1;

        stageText.text =
          worldName + "-" + stage;
    }
}