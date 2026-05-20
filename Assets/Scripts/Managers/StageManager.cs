using UnityEngine;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance;

    public WorldData[] worlds;

    public int currentWorldIndex = 0;

    public int currentStageIndex = 0;

    void Awake()
    {
        Instance = this;
    }

    public WorldData CurrentWorld()
    {
        return worlds[currentWorldIndex];
    }

    public StageData CurrentStage()
    {
        return CurrentWorld().stages[currentStageIndex];
    }

    public void OnPlayerWin()
    {
        currentStageIndex++;

        // Worldクリア
        if (
            currentStageIndex >=
            CurrentWorld().stages.Length
        )
        {
            // 最後のWorld
            if (
                currentWorldIndex >=
                worlds.Length - 1
            )
            {
                Debug.Log("GAME CLEAR");

                currentStageIndex =
                    CurrentWorld().stages.Length - 1;

                return;
            }

            // 次Worldへ
            currentWorldIndex++;

            currentStageIndex = 0;

            Debug.Log("NEXT WORLD");
        }


        DevelopmentManager.Instance.NextStage(1);
    }

    public void SpawnStageEnemy()
    {
        BoardManager.Instance
            .SpawnStageEnemies();
    }
}