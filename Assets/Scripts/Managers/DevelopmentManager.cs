using UnityEngine;

public class DevelopmentManager : MonoBehaviour
{
    public static DevelopmentManager Instance;

    // 総部隊数
    public int totalTeams = 5;

    // 液体管理
    public int liquidTeams = 0;

    // 資源調達
    public int resourceTeams = 0;

    [SerializeField]
    public bool debugMode;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (debugMode)
        {
            totalTeams = 50;
        }
    }

    // 使用済み
    public int UsedTeams()
    {
        return liquidTeams + resourceTeams;
    }

    // 残り
    public int RemainingTeams()
    {
        return totalTeams - UsedTeams();
    }

    // 次ステージ
    public void NextStage(int addTeams)
    {
        totalTeams -= resourceTeams;

        totalTeams += addTeams;

        FindFirstObjectByType<TotalTeamsUI>()
            .Refresh();

        FindFirstObjectByType<LiquidSectionUI>()
            .Refresh();

        FindFirstObjectByType<ResourceSectionUI>()
            .Refresh();
    }
}
