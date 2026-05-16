using UnityEngine;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance;

    public int currentStage = 0;

    void Awake()
    {
        Instance = this;
    }

    public void OnPlayerWin()
    {
        currentStage++;

        DevelopmentManager.Instance.totalTeams++;

        FindFirstObjectByType<TotalTeamsUI>()
        .Refresh();
    }

    public void SpawnStageEnemy()
    {
        BoardManager.Instance.SpawnStageEnemy(
            currentStage);
    }
}