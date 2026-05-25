using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance;

    public WorldData[] worlds;

    public WorldData startWorld;

    public WorldData currentWorld;

    public int currentWorldIndex = 0;

    public int currentStageIndex = 0;

    void Awake()
    {
        // 既に存在するなら削除
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Scene切替でも残す
        DontDestroyOnLoad(gameObject);

        if (currentWorld == null)
        {
            currentWorld = startWorld;
        }

        Debug.Log("Awake currentWorld : " +(currentWorld == null? "NULL": currentWorld.worldName));
    }

    public WorldData CurrentWorld()
    {
        return currentWorld;
    }

    public StageData CurrentStage()
    {
        return CurrentWorld().stages[currentStageIndex];
    }

    public void OnPlayerWin()
    {
        currentStageIndex++;

        // Worldクリア
        if (currentStageIndex >= CurrentWorld().stages.Length)
        {
            // 次Worldなし
            if (CurrentWorld().nextWorlds.Length == 0)
            {
                Debug.Log("GAME CLEAR");

                currentStageIndex =
                    CurrentWorld().stages.Length - 1;

                return;
            }

            currentWorldIndex++;

            currentStageIndex = 0;

            BattleManager.Instance.CleanupOnly();

            Debug.Log("NEXT WORLD");

            SceneManager.LoadScene("RootChoiceScene");

            return;
        }


        DevelopmentManager.Instance.NextStage(1);
        FindFirstObjectByType<StageUI>().Refresh();
    }

    public void SpawnStageEnemy()
    {
        BoardManager.Instance
            .SpawnStageEnemies();
    }

    public void SelectWorld(WorldData world)
    {
        Debug.Log(
            "Select World : " +
            world.worldName);

        currentWorld = world;

        Debug.Log(
            "CurrentWorld After Select : " +
            currentWorld.worldName);

        currentStageIndex = 0;

        SceneManager.LoadScene("BattleScene");
    }
}