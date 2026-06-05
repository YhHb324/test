using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance;

    public GameObject tilePrefab;
    public GameObject unitPrefab;
    public GameObject enemyUnitPrefab;

    public int boardWidth = 7;
    public int boardHeight = 8;

    public ItemData draggingItem;
    public Image dragItemIcon;

    public ItemData triggeritem;

    Tile[,] boardTiles;
    Tile[] benchTiles;
    Tile[] recoveryTiles;

    BattleUnit draggingUnit;

    Vector3 dragOffset;

    void Awake()
    {
        Instance = this;
    }

    IEnumerator Start()
    {
        CreateBoard();
        CreateBench();
        CreateRecoveryBench();
        RestoreUnits();

        StageManager.Instance.SpawnStageEnemy();

        yield return null;
    }

    void Update()
    {
        HandleMouseInput();

        UpdateDragItemIcon();
    }

    // =========================
    // マウス入力
    // =========================

    void HandleMouseInput()
    {
        if (BattleManager.Instance.state != GameState.Setup1 && BattleManager.Instance.state != GameState.Setup2)
        {
            return;
        }

        Vector3 mouseWorld =
            Camera.main.ScreenToWorldPoint(Input.mousePosition);

        mouseWorld.z = 0;

        // =========================
        // Itemドラッグ中
        // =========================

        if (draggingItem != null)
        {
            if (Input.GetMouseButtonUp(0))
            {
                DropItem(mouseWorld);
            }

            return;
        }

        // 押した瞬間
        if (Input.GetMouseButtonDown(0))
        {
            Collider2D[] hits =
                Physics2D.OverlapPointAll(mouseWorld);

            foreach (Collider2D hit in hits)
            {
                BattleUnit unit =
                    hit.GetComponent<BattleUnit>();

                if (unit == null)
                    continue;

                Debug.Log(unit.name);

                if (unit != null)
                {
                    // 追加：ツールチップ表示
                    ToolTipUI.Instance.Show(unit);

                    if (unit.isEnemy)
                        return;

                    draggingUnit = unit;
                    dragOffset =
                        unit.transform.position - mouseWorld;
                    break;
                }

                if (unit.isEnemy)
                    return;

                draggingUnit = unit;

                dragOffset =
                    unit.transform.position
                    - mouseWorld;

                break;
            }
        }

        // ドラッグ中
        if (
            draggingUnit != null &&
            Input.GetMouseButton(0)
        )
        {
            draggingUnit.transform.position =
                mouseWorld + dragOffset;
        }

        // 離した
        if (
            draggingUnit != null &&
            Input.GetMouseButtonUp(0)
        )
        {
            DropUnit(mouseWorld);
        }
    }

    // =========================
    // Unitを置く
    // =========================

    void DropUnit(Vector3 mouseWorld)
    {

        Collider2D[] hits =
            Physics2D.OverlapPointAll(mouseWorld);

        Tile targetTile = null;

        foreach (Collider2D hit in hits)
        {
            Tile tile =
                hit.GetComponent<Tile>();

            if (tile != null)
            {
                targetTile = tile;
                break;
            }
        }

        // タイルなし
        if (targetTile == null)
        {
            ReturnUnit();
            return;
        }

        //敵陣
        if (targetTile.tileArea == TileArea.Enemy)
        {
            ReturnUnit();
            return;
        }

        BattleUnit otherUnit = null;

        if (draggingUnit.isDamaged && targetTile.tileType == TileType.Board)
        {
            ReturnUnit();
            return;
        }

        // 既にユニットがいる
        if (
            targetTile.currentUnit != null &&
            targetTile.currentUnit != draggingUnit
        )
        {
            otherUnit = targetTile.currentUnit;

            // 敵は交換不可
            if (otherUnit.isEnemy)
            {
                ReturnUnit();
                return;
            }
        }

        Tile oldTile =
             draggingUnit.currentTile;

        // 入れ替え時
        if (otherUnit != null)
        {
            oldTile.currentUnit = otherUnit;

            otherUnit.currentTile = oldTile;

            otherUnit.transform.position =
                oldTile.transform.position;

            otherUnit.isOnBench =
                (oldTile.tileType == TileType.Bench);

            otherUnit.SetDirectionForTile();
        }
        else
        {
            oldTile.currentUnit = null;
        }

        bool movingToBoard = targetTile.tileType
            == TileType.Board;

        bool cameFromBench = draggingUnit.currentTile.tileType
            == TileType.Bench;

        if (
            movingToBoard &&
            cameFromBench &&
            CurrentBoardUnitCount() >= StageManager.Instance.CurrentWorld().maxBoardUnits
        )
        {
            Debug.Log("Board Full");

            ReturnUnit();

            return;
        }

        // 新タイル
        draggingUnit.currentTile = targetTile;

        targetTile.currentUnit = draggingUnit;

        draggingUnit.transform.position =
            targetTile.transform.position;

        draggingUnit.isOnBench =
        (targetTile.tileType == TileType.Bench);

        draggingUnit.SetDirectionForTile();;

        draggingUnit = null;
    }

    void ReturnUnit()
    {
        draggingUnit.transform.position =
            draggingUnit.currentTile.transform.position;

        draggingUnit = null;
    }

    void DropItem(Vector3 mouseWorld)
    {
        Collider2D[] hits =
            Physics2D.OverlapPointAll(mouseWorld);

        BattleUnit unit = null;

        foreach (Collider2D hit in hits)
        {
            unit = hit.GetComponent<BattleUnit>();

            if (unit != null)
            {
                break;
            }
        }

        if (unit == null)
        {
            dragItemIcon.enabled = false;
            draggingItem = null;
            return;
        }

        Debug.Log(unit.name);

        if (unit.isEnemy)
        {
            dragItemIcon.enabled = false;
            draggingItem = null;
            return;
        }

        // 空きスロット探索
        for (int i = 0; i < unit.items.Length; i++)
        {
            if (unit.items[i] == null)
            {
                unit.items[i] = draggingItem;

                EvolutionData evo = unit.GetEvolution();

                if (evo != null)
                {
                    BattleUnit newUnit =
                        unit.Evolve(evo.resultUnit);

                    newUnit.evolutionKeyItem =
                        evo.triggerItem;

                    RankUpManager.Instance.CheckRankUp();
                }
                else
                {
                    unit.RefreshStats();
                }

                ToolTipUI.Instance.Show(unit);

                OwnedItemsUI itemsUI =
                    FindFirstObjectByType<OwnedItemsUI>();

                int index =
                    System.Array.IndexOf(
                        itemsUI.itemDatas,
                        draggingItem);

                if (index >= 0)
                {
                    ItemManager.Instance.itemCounts[index]--;

                    itemsUI.Refresh();
                }

                dragItemIcon.enabled = false;
                draggingItem = null;

                return;
            }
        }

        Debug.Log("Item Full");

        dragItemIcon.enabled = false;
        draggingItem = null;
    }

    void UpdateDragItemIcon()
    {

        if (dragItemIcon == null)
        {
            return;
        }

        if (draggingItem == null)
        {
            return;
        }

        RectTransform rect = dragItemIcon.rectTransform;

        Canvas canvas =
            dragItemIcon.canvas;

        Vector2 localPos;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            Input.mousePosition,
            canvas.worldCamera,
            out localPos
        );

        rect.localPosition = localPos;
    }

    // =========================
    // Board生成
    // =========================

    void CreateBoard()
    {
        float offsetY = -3f;

        boardTiles = new Tile[boardWidth, boardHeight];

        for (int y = 0; y < boardHeight; y++)
        {
            for (int x = 0; x < boardWidth; x++)
            {
                Vector3 pos =
                    new Vector3(
                        x - 3f,
                        y + offsetY,
                        0
                    );

                GameObject obj =
                    Instantiate(
                        tilePrefab,
                        pos,
                        Quaternion.identity
                    );

                Tile tile =
                    obj.GetComponent<Tile>();

                tile.x = x;
                tile.y = y;
                tile.tileType = TileType.Board;

                // 下4列 = Player
                if (y < 4)
                {
                    tile.tileArea =
                        TileArea.Player;
                }

                // 上4列 = Enemy
                else
                {
                    tile.tileArea =
                        TileArea.Enemy;
                }

                if (tile.tileArea == TileArea.Enemy)
                {
                    Sprite enemySprite =
                        StageManager.Instance
                        .CurrentWorld()
                        .enemyTileSprite;

                    if (enemySprite != null)
                    {
                        tile.SetSprite(enemySprite);
                    }
                }

                boardTiles[x, y] = tile;

                boardTiles[x, y] = tile;
            }
        }
    }

    int CurrentBoardUnitCount()
    {
        int count = 0;

        BattleUnit[] units =
            FindObjectsByType<BattleUnit>(
                FindObjectsSortMode.None);

        foreach (BattleUnit unit in units)
        {
            if (unit == null)
                continue;

            if (unit.isEnemy)
                continue;

            if (unit.currentTile == null)
                continue;

            if (unit.currentTile.tileType
                != TileType.Board)
                continue;

            count++;
        }

        return count;
    }

    // =========================
    // Bench生成
    // =========================

    void CreateBench()
    {
        float boardOffsetY = -3.5f;
        float benchY = boardOffsetY -1f; //盤面の1マス下

        int benchCount = 7;
        benchTiles = new Tile[benchCount];

        for (int x = 0; x < benchCount; x++)
        {
            Vector3 pos = 
                new Vector3(
                    x - 3f,  //盤面中央寄せ。0開始にしたければ x
                    benchY,
                    0
                );

            GameObject obj =
                Instantiate(
                    tilePrefab,
                    pos,
                    Quaternion.identity
                );

            Tile tile =
                obj.GetComponent<Tile>();

            tile.x = x;
            tile.y = -1;
            tile.tileType = TileType.Bench;
            tile.tileArea = TileArea.Player;

            benchTiles[x] = tile;
        }
    }

    // =========================
    // Recovery生成
    // =========================

    void CreateRecoveryBench()
    {
        recoveryTiles = new Tile[6];

        float startX = -7f;
        float startY = -3.5f;

        int index = 0;

        for (int y = 0; y < 2; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                GameObject obj =
                    Instantiate(
                        tilePrefab,
                        new Vector3(
                            startX + x,
                            startY - y,
                            0),
                        Quaternion.identity);

                Tile tile =
                    obj.GetComponent<Tile>();

                tile.tileType =
                    TileType.Recovery;

                recoveryTiles[index] = tile;
                index++;
            }
        }
    }


    public Tile GetBoardTile(int x, int y)
    {
        if (x < 0 || x >= boardWidth)
            return null;

        if (y < 0 || y >= boardHeight)
            return null;

        return boardTiles[x, y];
    }

    public void SpawnPlayerUnitsToBench(int spawnCount)
    {

        for (int i = 0; i < spawnCount; i++)
        {
            Tile emptyTile = null;

            // 空きベンチ探索
            foreach (Tile tile in benchTiles)
            {
                if (tile.currentUnit == null)
                {
                    emptyTile = tile;
                    break;
                }
            }

            // 空きなし
            if (emptyTile == null)
            {
                Debug.Log("Bench Full");
                break;
            }

            SpawnOneUnit(emptyTile);
        }
    }

    void SpawnOneUnit(Tile tile)
    {

        GameObject obj =
            Instantiate(unitPrefab,
                tile.transform.position,
                Quaternion.identity);

        BattleUnit unit = obj.GetComponent<BattleUnit>();

        unit.currentTile = tile;
        tile.currentUnit = unit;

        unit.SetDirectionForTile();
    }

    public void SpawnStageEnemies()
    {
        StageData stage =
            StageManager.Instance.CurrentStage();

        foreach (EnemySpawnData enemy
            in stage.enemies)
        {

            if (enemy == null)
            {
                Debug.Log("Enemy Null");
                continue;
            }

            if (enemy.unitData == null)
            {
                Debug.Log("UnitData Null");
                continue;
            }

            Tile tile =
                GetBoardTile(enemy.x, enemy.y);

            if (tile == null)
                continue;

            if (tile.currentUnit != null)
            {
                Debug.Log("Tile Occupied");
                continue;
            }

            GameObject obj =
                Instantiate(
                    enemyUnitPrefab,
                    tile.transform.position,
                    Quaternion.identity);

            BattleUnit unit =
                obj.GetComponent<BattleUnit>();

            unit.data = enemy.unitData;

            unit.currentTile = tile;

            unit.isEnemy = true;

            tile.currentUnit = unit;

            
        }
    }

    public void RestoreUnits()
    {
        foreach (
            SavedUnitData save
            in SaveManager.Instance.savedUnits
        )
        {
            Tile tile;

            if (save.isOnBench)
            {
                tile =
                    benchTiles[save.benchIndex];
            }
            else
            {
                tile = GetBoardTile(save.x, save.y);
            }

            if (tile == null)
            {
                Debug.Log("Tile Null");
                continue;
            }

            GameObject obj =
                Instantiate(
                    unitPrefab,
                    tile.transform.position,
                    Quaternion.identity);

            BattleUnit unit = obj.GetComponent<BattleUnit>();

            unit.data = save.unitData;

            unit.items = (ItemData[])save.items.Clone();

            unit.currentTile = tile;

            tile.currentUnit = unit;

            unit.RefreshStats();

            unit.SetDirectionForTile();
        }
    }

    public void RestoreBattleResultUnits()
    {
        Debug.Log("Restore count = "+ SaveManager.Instance.battleResultUnits.Count);

        foreach (
            SavedUnitData save
            in SaveManager.Instance.battleResultUnits
        )
        {
            Debug.Log(save.unitData.unitName + " damaged="+ save.isDamaged);

            Tile tile;

            if (save.isOnBench)
            {
                tile = benchTiles[save.benchIndex];
            }
            else
            {
                tile = GetBoardTile(save.x, save.y);
            }

            if (tile == null)
                continue;

            if (tile.currentUnit != null)
            {
                Debug.Log(
                    "Occupied : "
                    + save.unitData.unitName
                );
                continue;
            }

            GameObject obj =
                Instantiate(
                    unitPrefab,
                    tile.transform.position,
                    Quaternion.identity);

            BattleUnit unit =
                obj.GetComponent<BattleUnit>();

            unit.data = save.unitData;
            unit.items =
                (ItemData[])save.items.Clone();

            unit.isDamaged =
                save.isDamaged;
            unit.RefreshDamageVisual();

            unit.currentTile = tile;
            tile.currentUnit = unit;

            unit.RefreshStats();
            unit.SetDirectionForTile();
        }
    }

    int GetRepairCost(BattleUnit unit)
    {
        if (unit.data.rank == 2)
            return 1;

        if (unit.data.rank == 3)
            return 3;

        return 0;
    }

    public void RecoverDamagedUnits(int availableTeams)
    {
        int remainTeams = availableTeams;

        BattleUnit[] units =
            FindObjectsByType<BattleUnit>(
                FindObjectsSortMode.None);

        foreach (BattleUnit unit in units)
        {
            if (!unit.isDamaged)
                continue;

            if (unit.currentTile.tileType
                != TileType.Recovery)
            {
                continue;
            }

            int cost =
                GetRepairCost(unit);

            if (remainTeams < cost)
                continue;

            remainTeams -= cost;

            unit.isDamaged = false;
            unit.RefreshDamageVisual();

            MoveToBench(unit);
        }

        SpawnPlayerUnitsToBench(remainTeams);
    }

    void MoveToBench(BattleUnit unit)
    {
        Tile emptyTile = null;

        foreach (Tile tile in benchTiles)
        {
            if (tile.currentUnit == null)
            {
                emptyTile = tile;
                break;
            }
        }

        if (emptyTile == null)
        {
            Debug.Log("Bench Full");
            return;
        }

        if (unit.currentTile != null)
        {
            unit.currentTile.currentUnit = null;
        }

        unit.currentTile = emptyTile;
        emptyTile.currentUnit = unit;

        unit.transform.position =
            emptyTile.transform.position;

        unit.isOnBench = true;
    }
}