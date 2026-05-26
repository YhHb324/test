using System.Collections;
using UnityEngine;

public class BattleUnit : MonoBehaviour
{
    public bool isEnemy;

    public int baseHP;

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
        hp = data.hp;

        attack = data.attack;

        range = data.range;

        attackSpeed = data.attackSpeed;

        defense = data.defense;

        moveSpeed = data.moveSpeed;

        foreach (ItemData item in items)
        {
            if (item == null)
                continue;

            hp += item.hpBonus;

            attack += item.attackBonus;

            range += item.rangeBonus;

            attackSpeed += item.attackSpeedBonus;

            defense += item.defenseBonus;

            moveSpeed += item.moveSpeedBonus;
        }
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
                Tile nextTile =
                    GetNextTileTowards(target);

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

    Tile GetNextTileTowards(BattleUnit target)
    {
        int dx =
            target.currentTile.x - currentTile.x;

        int dy =
            target.currentTile.y - currentTile.y;

        Tile bestTile = null;

        // 横優先
        if (Mathf.Abs(dx) > Mathf.Abs(dy))
        {
            bestTile =
                TryGetTile(
                    currentTile.x + (dx > 0 ? 1 : -1),
                    currentTile.y);
        }
        else
        {
            bestTile =
                TryGetTile(
                    currentTile.x,
                    currentTile.y + (dy > 0 ? 1 : -1));
        }

        return bestTile;
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

    IEnumerator MoveTo(Tile tile)
    {
        Tile oldTile = currentTile;

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