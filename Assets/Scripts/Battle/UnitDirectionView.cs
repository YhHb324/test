using UnityEngine;

public class UnitDirectionView : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Direction Sprites")]
    [SerializeField] private Sprite frontSprite;
    [SerializeField] private Sprite backSprite;
    [SerializeField] private Sprite sideSprite;

    private void Awake()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    public void SetDirectionFromVector(Vector2Int direction)
    {
        if (direction == Vector2Int.zero)
            return;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            SetDirection(direction.x > 0 ? Vector2Int.right : Vector2Int.left);
        }
        else
        {
            SetDirection(direction.y > 0 ? Vector2Int.up : Vector2Int.down);
        }
    }

    private void SetDirection(Vector2Int direction)
    {
        if (spriteRenderer == null)
        {
            Debug.LogWarning($"{name}: SpriteRenderer が設定されていません");
            return;
        }

        if (direction == Vector2Int.up)
        {
            if (backSprite != null)
                spriteRenderer.sprite = backSprite;

            spriteRenderer.flipX = false;
        }
        else if (direction == Vector2Int.down)
        {
            if (frontSprite != null)
                spriteRenderer.sprite = frontSprite;

            spriteRenderer.flipX = false;
        }
        else if (direction == Vector2Int.left)
        {
            if (sideSprite != null)
                spriteRenderer.sprite = sideSprite;

            spriteRenderer.flipX = false;
        }
        else if (direction == Vector2Int.right)
        {
            if (sideSprite != null)
                spriteRenderer.sprite = sideSprite;

            spriteRenderer.flipX = true;
        }
    }


    public void ResetDirection()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        if (spriteRenderer == null)
        {
            Debug.LogWarning($"{name}: SpriteRenderer が見つかりません");
            return;
        }

        if (backSprite != null)
        {
            spriteRenderer.sprite = backSprite;
        }

        spriteRenderer.flipX = false;
    }

    public void FaceFront()
    {
        Debug.Log($"{name}: FaceFront called");

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        if (spriteRenderer == null)
        {
            Debug.LogWarning($"{name}: SpriteRenderer が見つかりません");
            return;
        }

        if (frontSprite != null)
        {
            spriteRenderer.sprite = frontSprite;
        }

        spriteRenderer.flipX = false;
    }

    public void FaceBack()
    {
        Debug.Log($"{name}: FaceBack called");

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        if (spriteRenderer == null)
        {
            Debug.LogWarning($"{name}: SpriteRenderer が見つかりません");
            return;
        }

        if (backSprite != null)
        {
            spriteRenderer.sprite = backSprite;
        }

        spriteRenderer.flipX = false;
    }

}