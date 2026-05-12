using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;
    public GameState state = GameState.Setup;

    public int allyCount = 0;
    public int enemyCount = 0;

    void Awake()
    {
        Instance = this;
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

        if (playerWin)
        {
            BattleUnit[] units =
                FindObjectsByType<BattleUnit>(
                    FindObjectsSortMode.None);

            foreach (BattleUnit unit in units)
            {
                if (!unit.isEnemy)
                {
                    unit.ReturnToOriginalTile();
                }
            }
        }
    }
}