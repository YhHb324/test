using UnityEngine;
using System.Collections.Generic;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    public WorldData initialWorld;
    public bool hasRunSave = false;

    // ユニット保存
    public List<SavedUnitData> savedUnits =
    new List<SavedUnitData>();

    public List<SavedUnitData> battleResultUnits =
        new List<SavedUnitData>();

    public List<SavedUnitData> damagedUnits =
        new List<SavedUnitData>();

    // アイテム所持数保存
    public int[] itemCounts =
    {
        0,
        0,
        0,
        0,
        0,
        0
    };

    // ライフ保存
    public int playerLife = 3;

    // 部隊数保存
    public int savedTotalTeams = 5;

    //ワールド保存
    public WorldData savedWorld;
    public int savedStageIndex;


    // =========================
    // Singleton
    // =========================

    void Awake()
    {
        Debug.Log("SaveManager Awake : " + GetInstanceID());
        if (Instance != null)
        {
            Debug.Log("Duplicate SaveManager Destroyed");
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
        InitializeRunSave();
    }

    void InitializeRunSave()
    {
        savedWorld =
            StageManager.Instance.startWorld;

        savedStageIndex = 0;

        playerLife = 3;

        savedTotalTeams = 5;

        itemCounts =
            new int[6];

        savedUnits.Clear();
    }

    // =========================
    // Unit Save
    // =========================

    public void SaveUnits()
    {
        savedUnits.Clear();

        BattleUnit[] units =
            FindObjectsByType<BattleUnit>(
                FindObjectsSortMode.None);

        foreach (BattleUnit unit in units)
        {
            if (unit == null)
                continue;

            if (unit.isEnemy)
                continue;

            if (unit.isDead)
                continue;

            if (unit.currentTile == null)
                continue;

            SavedUnitData save =
                new SavedUnitData();

            save.unitData = unit.data;

            save.items =
                (ItemData[])unit.items.Clone();

            save.isOnBench =
                unit.currentTile.tileType
                == TileType.Bench;

            if (save.isOnBench)
            {
                save.benchIndex =
                    unit.currentTile.x;
            }
            else
            {
                save.x =
                    unit.currentTile.x;

                save.y =
                    unit.currentTile.y;
            }

            savedUnits.Add(save);
        }

        Debug.Log(
            "Saved Units : " +
            savedUnits.Count
        );
    }

    public void ProcessBattleResult()
    {
        battleResultUnits.Clear();

        BattleUnit[] units =
            FindObjectsByType<BattleUnit>(
                FindObjectsSortMode.None);

        foreach (BattleUnit unit in units)
        {
            if (unit.isEnemy)
                continue;

            SavedUnitData save =
                new SavedUnitData();

            save.unitData = unit.data;
            save.items =
                (ItemData[])unit.items.Clone();

            save.isOnBench =
                unit.isOnBench;

            if (unit.currentTile != null)
            {
                save.x = unit.currentTile.x;
                save.y = unit.currentTile.y;
                save.benchIndex =
                    unit.currentTile.x;
            }

            battleResultUnits.Add(save);
        }

        Debug.Log("damagedUnits=" + damagedUnits.Count);
        battleResultUnits.AddRange(damagedUnits);
        Debug.Log("battleResultUnits=" + battleResultUnits.Count);

        damagedUnits.Clear();
    }

    public void SaveDamagedUnit(BattleUnit unit)
    {
        SavedUnitData save =
            new SavedUnitData();

        save.unitData =
            unit.data;

        save.items =
            (ItemData[])unit.items.Clone();

        save.isDamaged = true;

        save.isOnBench =
            unit.isOnBench;

        if (unit.currentTile != null)
        {
            save.x =
                unit.currentTile.x;

            save.y =
                unit.currentTile.y;

            save.benchIndex =
                unit.currentTile.x;
        }

        damagedUnits.Add(save);
        Debug.Log("DamagedSaved : " + unit.name);
    }
    // =========================
    // Item Save
    // =========================

    public void SaveItems()
    {
        itemCounts =
            (int[])ItemManager.Instance
                .itemCounts
                .Clone();

        Debug.Log("Items Saved");
    }

    // =========================
    // Life Save
    // =========================

    public void SaveLife()
    {
        playerLife =
            SaveManager.Instance.playerLife;

        Debug.Log(
            "Life Saved : " +
            playerLife
        );
    }

    // =========================
    // Teams Save
    // =========================

    public void SaveTeams()
    {
        savedTotalTeams =
            DevelopmentManager.Instance.totalTeams;

        Debug.Log(
            "Teams Saved : " +
            savedTotalTeams
        );
    }

    // =========================
    // Restore
    // =========================

    public void RestoreItems()
    {
        ItemManager.Instance.itemCounts =
            (int[])itemCounts.Clone();
    }

    public void RestoreTeams()
    {
        DevelopmentManager.Instance.totalTeams =
            savedTotalTeams;
    }

    // =========================
    // Full Save
    // =========================

    public void SaveRun()
    {
        savedWorld = StageManager.Instance.currentWorld;
        savedStageIndex = StageManager.Instance.currentStageIndex;

        SaveUnits();
        SaveItems();
        SaveLife();
        SaveTeams();
        hasRunSave = true;

        Debug.Log("Run Saved");
    }

    // =========================
    // Clear
    // =========================

    public void ClearRun()
    {
        savedUnits.Clear();

        itemCounts =
            new int[6];

        playerLife = 3;

        savedTotalTeams = 5;

        Debug.Log("Run Cleared");
    }

    public void LoadRun()
    {
        Debug.Log("=== LoadRun Start ===");
        Debug.Log("LoadRun SaveManager ID : " + GetInstanceID());

        // World復元
        StageManager.Instance.currentWorld =
            savedWorld;

        // Stage復元
        StageManager.Instance.currentStageIndex =
            savedStageIndex;

        // Unit確認
        Debug.Log(
            "Saved Units Count : " +
            savedUnits.Count);

        foreach (SavedUnitData unit in savedUnits)
        {
            if (unit == null)
            {
                Debug.Log("Saved Unit : NULL");
                continue;
            }

            Debug.Log(
                "Saved Unit : " +
                unit.unitData.unitName
            );
        }

        // Item在庫復元
        RestoreItems();

        // 部隊数復元
        RestoreTeams();

        Debug.Log("=== LoadRun End ===");
    }
}