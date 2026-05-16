using UnityEngine;

public class DevelopmentManager : MonoBehaviour
{
    public static DevelopmentManager Instance;

    // 総部隊数
    public int totalTeams = 20;

    // 液体管理
    public int liquidTeams = 0;

    // 資源調達
    public int resourceTeams = 0;

    void Awake()
    {
        Instance = this;
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
        // 資源調達は消える
        resourceTeams = 0;

        // 部隊追加
        totalTeams += addTeams;

        FindFirstObjectByType<TotalTeamsUI>()
        .Refresh();
    }
}
