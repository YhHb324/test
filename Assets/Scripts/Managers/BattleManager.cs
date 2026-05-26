using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;
    public GameState state = GameState.Setup1;

    public int allyCount = 0;
    public int enemyCount = 0;

    public int playerLife = 3;

    void Awake()
    {
        Instance = this;
    }

    public void NextPhase()
    {
        if (state == GameState.Setup1)
        {
            state = GameState.Setup2;
            Debug.Log("→ Setup2");

            EnterSetup2();
            return;
        }

        if (state == GameState.Setup2)
        {
            StartBattle();
            return;
        }

        if (state == GameState.Battle)
        {
            // 戦闘中はボタン無効でもOK
            return;
        }

        if (state == GameState.End)
        {
            state = GameState.Setup1;
            Debug.Log("→ Setup1");
            return;
        }
    }

    void EnterSetup2()
    {
        BoardManager.Instance.SpawnPlayerUnitsToBench();

        FindFirstObjectByType<OwnedItemsUI>()
            .GenerateItems();
    }

    public void StartBattle()
    {
        state = GameState.Battle;

        BattleUnit[] units =
            FindObjectsByType<BattleUnit>(FindObjectsSortMode.None);

        bool allyExistsOnBoard = false;
        bool enemyExistsOnBoard = false;

        foreach (BattleUnit unit in units)
        {
            if (unit == null || unit.isDead)
                continue;

            if (unit.currentTile == null)
                continue;

            if (unit.currentTile.tileType != TileType.Board)
                continue;

            if (unit.isEnemy)
                enemyExistsOnBoard = true;
            else
                allyExistsOnBoard = true;
        }

        if (!allyExistsOnBoard || !enemyExistsOnBoard)
        {
            Debug.Log("NO UNITS ON BOARD → IMMEDIATE END");

            EndBattle(allyExistsOnBoard);
            return;
        }

        // 通常開始
        foreach (BattleUnit unit in units)
        {
            unit.SetBattleStartTile();

            if (unit.currentTile != null &&
                unit.currentTile.tileType == TileType.Bench)
            {
                continue;
            }

            unit.StartAI();
        }

        Debug.Log("Battle Start");
    }

    public void OnUnitDead(BattleUnit unit)
    {
        if (unit.isEnemy)
            enemyCount--;
        else
            allyCount--;

        CheckBattleEnd();
    }

    public void CheckBattleEnd()
    {
        if (state != GameState.Battle)
            return;

        BattleUnit[] units =
            FindObjectsByType<BattleUnit>(FindObjectsSortMode.None);

        bool allyAlive = false;
        bool enemyAlive = false;

        foreach (BattleUnit unit in units)
        {
            if (unit == null || unit.isDead)
                continue;

            if (unit.currentTile == null)
                continue;

            // ★盤面だけが戦闘参加
            if (unit.currentTile.tileType != TileType.Board)
                continue;

            if (unit.isEnemy)
                enemyAlive = true;
            else
                allyAlive = true;
        }

        if (!allyAlive || !enemyAlive)
        {
            EndBattle(allyAlive);
        }
    }

    void EndBattle(bool playerWin)
    {
        Debug.Log(
            playerWin
            ? "PLAYER WIN"
            : "PLAYER LOSE");

        // 勝利時だけステージ進行
        if (playerWin)
        {
            StageManager.Instance.OnPlayerWin();
        }
        else
        {
            playerLife--;

            FindFirstObjectByType<LifeUI>()
                .Refresh();

            if (playerLife <= 0)
            {
                Debug.Log("GAME OVER");
            }
        }

        state = GameState.End;

        StartCoroutine(EndProcess());
    }

    IEnumerator EndProcess()
    {
        
        yield return new WaitForSeconds(1f);

        CleanupBattle();
        SaveUnits();

        yield return new WaitForSeconds(0.5f);

        yield return null;

        // 次ステージ敵生成
        StageManager.Instance.SpawnStageEnemy();

        // 自動でSetup1へ戻す
        state = GameState.Setup1;

        Debug.Log("→ Setup1");
    }

    void CleanupBattle()
    {
        BattleUnit[] units =
            FindObjectsByType<BattleUnit>(
                FindObjectsSortMode.None);

        foreach (BattleUnit unit in units)
        {
            unit.StopAI();
        }

        foreach (BattleUnit unit in units)
        {
            if (!unit.isEnemy)
            {
                unit.ReturnToOriginalTile();
            }
        }

        List<BattleUnit> enemies =
            new List<BattleUnit>();

        foreach (BattleUnit unit in units)
        {
            if (unit.isEnemy)
            {
                enemies.Add(unit);
            }
        }

        foreach (BattleUnit enemy in enemies)
        {
            if (enemy.currentTile != null)
            {
                enemy.currentTile.currentUnit = null;
            }

            Destroy(enemy.gameObject);
        }

        DevelopmentManager.Instance.liquidTeams = 0;
        DevelopmentManager.Instance.resourceTeams = 0;

        FindFirstObjectByType<LiquidSectionUI>()
            .Refresh();

        FindFirstObjectByType<ResourceSectionUI>()
            .Refresh();

        FindFirstObjectByType<TotalTeamsUI>()
            .Refresh();
    }

    public void CleanupOnly()
    {
        CleanupBattle();
    }

    public void SaveUnits()
    {
        UnitManager.Instance.savedUnits.Clear();

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
                save.benchIndex = unit.currentTile.y;
            }
            else
            {
                save.x = unit.currentTile.x;
                save.y = unit.currentTile.y;
            }

            UnitManager.Instance.savedUnits
                .Add(save);
        }

        Debug.Log("Saved Units : " + UnitManager.Instance.savedUnits.Count
        );
    }
}