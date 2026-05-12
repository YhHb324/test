using System.Collections;
using UnityEngine;

public class BattleUnit : MonoBehaviour
{
    public bool isEnemy;

    public int hp = 100;
    public int attack = 20;

    public int range = 1;

    public float attackInterval = 1f;
    public float moveDuration = 0.5f;

    public Tile currentTile;

    Tile originalTile;

    public bool isOnBench = true;
    public bool isDead = false;

    public void StartAI()
    {
        StartCoroutine(AILoop());
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
                    new WaitForSeconds(attackInterval);
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
        currentTile.currentUnit = null;

        tile.currentUnit = this;

        currentTile = tile;

        Vector3 start =
            transform.position;

        Vector3 end =
            tile.transform.position;

        float time = 0f;

        while (time < moveDuration)
        {
            time += Time.deltaTime;

            transform.position =
                Vector3.Lerp(
                    start,
                    end,
                    time / moveDuration);

            yield return null;
        }

        transform.position = end;
    }

    public void ReturnToOriginalTile()
    {
        currentTile.currentUnit = null;

        currentTile = originalTile;

        originalTile.currentUnit = this;

        transform.position =
            originalTile.transform.position;
    }
}