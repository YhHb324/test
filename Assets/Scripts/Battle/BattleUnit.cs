using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUnit : MonoBehaviour
{
    public bool isEnemy;

    public int baseHP;

    public int maxHp;

    public UnitData data;

    public int hp;

    public int attack;

    public int range;

    public float attackSpeed;

    public int defense;

    public float moveSpeed;

    public ItemData[] items = new ItemData[3];

    public Tile currentTile;

    Tile originalTile;

    public bool isOnBench = true;
    public bool isDead = false;

    void Start()
    {
        RefreshStats();
    }

    public void StartAI()
    {
        StartCoroutine(AILoop());
    }

    public void StopAI()
    {
        StopAllCoroutines();
    }

    public void RefreshStats()
    {
        maxHp = data.hp;

        attack = data.attack;

        range = data.range;

        attackSpeed = data.attackSpeed;

        defense = data.defense;

        moveSpeed = data.moveSpeed;

        foreach (ItemData item in items)
        {
            if (item == null)
                continue;

            maxHp += item.hpBonus;

            attack += item.attackBonus;

            range += item.rangeBonus;

            attackSpeed += item.attackSpeedBonus;

            defense += item.defenseBonus;

            moveSpeed += item.moveSpeedBonus;
        }

        hp = maxHp;

    }

    public void SetBattleStartTile()
    {
        originalTile = currentTile;
    }

    IEnumerator AILoop()
    {
        yield return new WaitUntil(() =>
        BattleManager.Instance.state == GameState.Battle);

        if (currentTile != null &&
       currentTile.tileType == TileType.Bench)
        {
            yield break;
        }

        while (!isDead)
        {
            BattleUnit target =
                FindNearestEnemy();

            if (target == null)
                yield break;

            int distance =
                GetDistance(currentTile, target.currentTile);

            // 射程内
            if (distance <= range)
            {
                Attack(target);

                yield return
                    new WaitForSeconds(1f/ attackSpeed);
            }
            else
            {
                Tile targetTile = GetTargetTileFromEnemies();

                if (targetTile == null)
                {
                    yield return null;
                    continue;
                }

                Tile nextTile =
                    GetNextTileTowards(targetTile);

                if (nextTile != null)
                {
                    yield return
                        MoveTo(nextTile);
                }
                else
                {
                    yield return null;
                }
            }
        }
    }

    BattleUnit FindNearestEnemy()
    {
        BattleUnit[] units =
            FindObjectsByType<BattleUnit>(FindObjectsSortMode.None);

        BattleUnit nearest = null;
        int bestDistance = 999;

        foreach (BattleUnit unit in units)
        {
            if (unit == this)
                continue;

            if (unit.isDead)
                continue;

            if (unit.currentTile == null)
                continue;

            if (unit.currentTile.tileType != TileType.Board)
                continue;

            if (unit.isEnemy == isEnemy)
                continue;

            int dist =
                GetDistance(currentTile, unit.currentTile);

            if (dist < bestDistance)
            {
                bestDistance = dist;
                nearest = unit;
            }
        }

        return nearest;
    }

    List<Tile> GetAttackableTiles(BattleUnit target)
    {
        List<Tile> tiles = new List<Tile>();

        int tx = target.currentTile.x;
        int ty = target.currentTile.y;

        for (int dx = -range; dx <= range; dx++)
        {
            for (int dy = -range; dy <= range; dy++)
            {
                int dist = Mathf.Abs(dx) + Mathf.Abs(dy);

                if (dist > range) continue;
                if (dx == 0 && dy == 0) continue;

                Tile tile = BoardManager.Instance.GetBoardTile(tx + dx, ty + dy);
                if (tile == null) continue;

                tiles.Add(tile);
            }
        }

        return tiles;
    }

    Tile GetBestAttackTile(List<Tile> tiles)
    {
        Tile best = null;
        int bestDist = 999;

        foreach (Tile tile in tiles)
        {
            if (tile.currentUnit != null) continue;
            if (tile.reservedUnit != null) continue;

            int dist = GetDistance(currentTile, tile);

            if (dist < bestDist)
            {
                bestDist = dist;
                best = tile;
            }
        }

        return best;
    }

    Tile GetTargetTileFromEnemies()
    {
        BattleUnit[] units =
            FindObjectsByType<BattleUnit>(FindObjectsSortMode.None);

        List<BattleUnit> enemies = new List<BattleUnit>();

        foreach (BattleUnit unit in units)
        {
            if (unit == null) continue;
            if (unit.isDead) continue;
            if (unit.currentTile == null) continue;
            if (unit.currentTile.tileType != TileType.Board) continue;
            if (unit.isEnemy == isEnemy) continue;

            enemies.Add(unit);
        }

        // 距離順にソート
        enemies.Sort((a, b) =>
            GetDistance(currentTile, a.currentTile)
            .CompareTo(GetDistance(currentTile, b.currentTile))
        );

        // 近い順に試す
        foreach (BattleUnit enemy in enemies)
        {
            List<Tile> tiles = GetAttackableTiles(enemy);

            Tile best = GetBestAttackTile(tiles);

            if (best != null)
                return best;
        }

        return null;
    }

    int GetDistance(Tile a, Tile b)
    {
        int dx = Mathf.Abs(a.x - b.x);
        int dy = Mathf.Abs(a.y - b.y);

        return dx + dy;
    }

    void Attack(BattleUnit target)
    {
        target.TakeDamage(attack);
    }

    public void TakeDamage(int damage)
    {
        hp -= damage;

        if (hp <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        currentTile.currentUnit = null;

        BattleManager.Instance.CheckBattleEnd();

        Destroy(gameObject);

    }

    Tile GetNextTileTowards(Tile targetTile)
    {
        int dx = targetTile.x - currentTile.x;
        int dy = targetTile.y - currentTile.y;

        int stepX = dx == 0 ? 0 : (dx > 0 ? 1 : -1);
        int stepY = dy == 0 ? 0 : (dy > 0 ? 1 : -1);

        // 優先方向決定（C）
        bool prioritizeX = Mathf.Abs(dx) > Mathf.Abs(dy);

        Tile forward = null;
        Tile side1 = null;
        Tile side2 = null;
        Tile back = null;

        if (prioritizeX)
        {
            forward = TryGetTileWithReserve(currentTile.x + stepX, currentTile.y);

            side1 = TryGetTileWithReserve(currentTile.x, currentTile.y + 1);
            side2 = TryGetTileWithReserve(currentTile.x, currentTile.y - 1);
        }
        else
        {
            forward = TryGetTileWithReserve(currentTile.x, currentTile.y + stepY);

            side1 = TryGetTileWithReserve(currentTile.x + 1, currentTile.y);
            side2 = TryGetTileWithReserve(currentTile.x - 1, currentTile.y);
        }

        back = TryGetTileWithReserve(
            currentTile.x - stepX,
            currentTile.y - stepY
        );

        // 優先順
        if (forward != null) return forward;
        if (side1 != null) return side1;
        if (side2 != null) return side2;
        if (back != null) return back;

        return null;
    }

    Tile TryGetTile(int x, int y)
    {
        Tile tile =
            BoardManager.Instance.GetBoardTile(x, y);

        if (tile == null)
            return null;

        if (tile.currentUnit != null)
            return null;

        return tile;
    }

    Tile TryGetTileWithReserve(int x, int y)
    {
        Tile tile = BoardManager.Instance.GetBoardTile(x, y);

        if (tile == null) return null;

        if (tile.currentUnit != null) return null;

        if (tile.reservedUnit != null) return null;

        return tile;
    }

    IEnumerator MoveTo(Tile tile)
    {
        Tile oldTile = currentTile;

        tile.reservedUnit = this;

        if (oldTile.currentUnit == this)
        {
            oldTile.currentUnit = null;
        }

        currentTile = tile;

        currentTile.currentUnit = this;

        Vector3 start =
            transform.position;

        Vector3 end =
            tile.transform.position;

        float time = 0f;

        while (time < 1f/moveSpeed)
        {
            time += Time.deltaTime;

            transform.position =
                Vector3.Lerp(
                    start,
                    end,
                    time / (1f/moveSpeed));

            yield return null;
        }

        transform.position = end;

        currentTile.reservedUnit = null;
    }

    public void ReturnToOriginalTile()
    {
        if (originalTile == null)
        {
            Debug.Log($"{name}: originalTile is null");
            return;
        }

        if (currentTile != null)
        {
            if (currentTile.currentUnit == this)
            {
                currentTile.currentUnit = null;
            }
        }

        currentTile = originalTile;
        originalTile.currentUnit = this;

        transform.position = originalTile.transform.position;

        Physics2D.SyncTransforms();

        RefreshStats();
    }
}