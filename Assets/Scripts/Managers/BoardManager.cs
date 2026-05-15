using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance;

    public GameObject tilePrefab;
    public GameObject unitPrefab;
    public GameObject enemyUnitPrefab;

    public int boardWidth = 7;
    public int boardHeight = 8;

    Tile[,] boardTiles;
    Tile[] benchTiles;

    BattleUnit draggingUnit;

    Vector3 dragOffset;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        CreateBoard();
        CreateBench();
        SpawnEnemyUnit();
    }

    void Update()
    {
        HandleMouseInput();
    }

    // =========================
    // マウス入力
    // =========================

    void HandleMouseInput()
    {
        if (BattleManager.Instance.state != GameState.Setup1 && BattleManager.Instance.state != GameState.Setup2)
            return;

        Vector3 mouseWorld =
        Camera.main.ScreenToWorldPoint(Input.mousePosition);

        mouseWorld.z = 0;

        // 押した瞬間
        if (Input.GetMouseButtonDown(0))
        {
            Collider2D hit =
                Physics2D.OverlapPoint(mouseWorld);

            if (hit != null)
            {
                BattleUnit unit = hit.GetComponent<BattleUnit>();

                if (unit != null)
                {
                    draggingUnit = unit;

                    dragOffset =
                        unit.transform.position
                        - mouseWorld;
                }
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

        // 埋まってる
        if (
            targetTile.currentUnit != null &&
            targetTile.currentUnit != draggingUnit
        )
        {
            ReturnUnit();
            return;
        }

        // 元タイル空に
        draggingUnit.currentTile.currentUnit =
            null;

        // 新タイル
        draggingUnit.currentTile =
            targetTile;

        targetTile.currentUnit =
            draggingUnit;

        draggingUnit.transform.position =
            targetTile.transform.position;

        draggingUnit.isOnBench =
        (targetTile.tileType == TileType.Bench);
        Debug.Log("Called DropUnit");

        draggingUnit = null;
    }

    void ReturnUnit()
    {
        draggingUnit.transform.position =
            draggingUnit.currentTile.transform.position;

        draggingUnit = null;
    }

    // =========================
    // Board生成
    // =========================

    void CreateBoard()
    {
        float offsetY = -3.5f;

        boardTiles = new Tile[boardWidth, boardHeight];

        for (int y = 0; y < boardHeight; y++)
        {
            for (int x = 0; x < boardWidth; x++)
            {
                Vector3 pos =
                    new Vector3(
                        x,
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

                tile.UpdateColor();

                boardTiles[x, y] = tile;
            }
        }
    }

    // =========================
    // Bench生成
    // =========================

    void CreateBench()
    {
        float offsetY = -2f;
        benchTiles = new Tile[5];

        for (int y = 0; y < 5; y++)
        {
            Vector3 pos =
                new Vector3(
                    -2,
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

            tile.tileType = TileType.Bench;
            tile.UpdateColor();

            benchTiles[y] = tile;
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

    public void SpawnPlayerUnitsToBench()
    {
        int spawnCount =
            DevelopmentManager.Instance.liquidTeams;

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
                return;
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
    }

    void SpawnEnemyUnit()
    {
        Tile tile = boardTiles[3, 6];
        GameObject obj = Instantiate(enemyUnitPrefab, tile.transform.position, Quaternion.identity);

        BattleUnit unit = obj.GetComponent<BattleUnit>();
        unit.currentTile = tile;
        unit.isEnemy = true;
        tile.currentUnit = unit;
    }

}