using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlotUI : MonoBehaviour
{
    public Image iconImage;
    public TMP_Text countText;

    public Color ownedColor = Color.white;

    public Color unownedColor =
        new Color(0.3f, 0.3f, 0.3f, 1f);

    public void SetItem(
        Sprite icon,
        int count
    )
    {
        iconImage.sprite = icon;

        // 未所持
        if (count <= 0)
        {
            iconImage.color = unownedColor;
            countText.gameObject.SetActive(false);
        }
        else
        {
            iconImage.color = ownedColor;

            // 2個以上なら表示
            if (count >= 2)
            {
                countText.gameObject.SetActive(true);
                countText.text = "×" + count;
            }
            else
            {
                countText.gameObject.SetActive(false);
            }
        }
    }
}
