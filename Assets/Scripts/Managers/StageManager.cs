using UnityEngine;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance;

    public WorldData currentWorld;

    int currentStageIndex = 0;

    void Awake()
    {
        Instance = this;
    }

    // 現在ステージ取得
    public StageData CurrentStage()
    {
        return currentWorld.stages[
            currentStageIndex];
    }

    // 勝利時
    public void OnPlayerWin()
    {
        // 次ステージへ
        currentStageIndex++;

        // 最終ステージ超え防止
        if (
            currentStageIndex >=
            currentWorld.stages.Length
        )
        {
            currentStageIndex =
                currentWorld.stages.Length - 1;

            Debug.Log("WORLD CLEAR");
        }

        // 開発部隊+1
        DevelopmentManager.Instance.totalTeams++;

        FindFirstObjectByType<TotalTeamsUI>()
            .Refresh();
    }

    // 敵生成
    public void SpawnStageEnemy()
    {
        BoardManager.Instance
            .SpawnStageEnemies();
    }
}