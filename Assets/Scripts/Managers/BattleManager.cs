using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;
    public GameState state = GameState.Setup1;

    public int allyCount = 0;
    public int enemyCount = 0;

    bool playerWin;

    bool gameOver;

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
        gameOver = false;
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

    void EndBattle(bool win)
    {
        playerWin = win;

        Debug.Log(playerWin ? "PLAYER WIN" : "PLAYER LOSE");

        if (!playerWin)
        {
            SaveManager.Instance.playerLife--;

            FindFirstObjectByType<LifeUI>()
                .Refresh();

            if (SaveManager.Instance.playerLife <= 0)
            {
                gameOver = true;
            }
        }

        state = GameState.End;

        StartCoroutine(EndProcess());
    }

    IEnumerator EndProcess()
    {
        if (playerWin)
        {
            BackgroundManager.Instance.PlayWinScroll();
        }
        else
        {
            BackgroundManager.Instance.PlayLoseScroll();
        }

        yield return new WaitForSeconds(1f);
        CleanupBattle();
        yield return new WaitForSeconds(0.5f);

        if (playerWin)
        {

            if (StageManager.Instance.OnPlayerWin())
            {
                StageManager.Instance.SpawnStageEnemy();

                state = GameState.Setup1;
            }
        }
        else
        {
            if (gameOver)
            {
                SceneManager.LoadScene("TitleScene");
            }
            else
            {
                SaveManager.Instance.LoadRun();

                if (SaveManager.Instance.hasRunSave)
                {
                    SceneManager.LoadScene("RootChoiceScene");
                }
                else
                {
                    SceneManager.LoadScene("BattleScene");
                }
            }
        }

        RefreshDevelopmentUI();
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
    }

    void RefreshDevelopmentUI()
    {
        DevelopmentManager.Instance.liquidTeams = 0;
        DevelopmentManager.Instance.resourceTeams = 0;

        FindFirstObjectByType<LiquidSectionUI>()
            ?.Refresh();

        FindFirstObjectByType<ResourceSectionUI>()
            ?.Refresh();

        FindFirstObjectByType<TotalTeamsUI>()
            ?.Refresh();
    }

    public void CleanupOnly()
    {
        CleanupBattle();
    }
}