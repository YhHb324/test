using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ToolTipUI : MonoBehaviour
{
    public static ToolTipUI Instance;

    [Header("基本表示")]
    public TMP_Text nameText;
    public Image unitImage;

    [Header("HP")]
    public GameObject hpBarRoot;
    public Image hpFillImage;
    public TMP_Text hpText;

    [Header("ステータス")]
    public TMP_Text statusText;

    [Header("アイテム")]
    public Image[] itemSlotImages; // 3個

    private BattleUnit currentUnit;

    void Awake()
    {
        Instance = this;
        Clear();
    }

    void Update()
    {
        if (currentUnit == null) return;

        if (BattleManager.Instance.state == GameState.Battle)
        {
            UpdateHP();
        }
    }

    public void Show(BattleUnit unit)
    {
        if (unit == null || unit.data == null) return;

        hpBarRoot.SetActive(true); //追加

        currentUnit = unit;

        nameText.text = unit.data.unitName;
        unitImage.sprite = unit.data.sprite;
        unitImage.enabled = unit.data.sprite != null;

        statusText.text =
            $"Atk : {unit.attack}\n" +
            $"Def : {unit.defense}\n" +
            $"AS : {unit.attackSpeed}\n" +
            $"MS : {unit.moveSpeed}\n" +
            $"Rng : {unit.range}";

        UpdateHP();
        UpdateItems(unit);
    }

    void UpdateHP()
    {
        int maxHp = currentUnit.maxHp;

        if (maxHp <= 0)
        {
            maxHp = currentUnit.data.hp;
        }

        hpText.text = $"{currentUnit.hp}/{maxHp}";

        float rate = Mathf.Clamp01((float)currentUnit.hp / maxHp);
        hpFillImage.fillAmount = rate;
    }

    void UpdateItems(BattleUnit unit)
    {
        for (int i = 0; i < itemSlotImages.Length; i++)
        {

            itemSlotImages[i].enabled = true; // ← 表示ON

            if (unit.isEnemy)
            {
                itemSlotImages[i].sprite = null;
                continue;
            }

            if (unit.items != null &&
                i < unit.items.Length &&
                unit.items[i] != null)
            {
                itemSlotImages[i].sprite = unit.items[i].icon;
            }
            else
            {
                itemSlotImages[i].sprite = null;
            }

            if (unit.isEnemy) continue;

            if (unit.items != null &&
                i < unit.items.Length &&
                unit.items[i] != null)
            {
                itemSlotImages[i].sprite = unit.items[i].icon;
                itemSlotImages[i].color = Color.white;
            }
        }
    }

    public void Clear()
    {
        currentUnit = null;

        nameText.text = "";
        unitImage.sprite = null;
        unitImage.enabled = false;

        hpText.text = "";
        hpFillImage.fillAmount = 0;

        hpBarRoot.SetActive(false); //追加

        statusText.text = "";

        foreach (Image slot in itemSlotImages)
        {
            slot.sprite = null;
            slot.color = Color.gray;
            slot.enabled = false;
        }
    }
}