using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PhaseButtonUI : MonoBehaviour
{
    public TextMeshProUGUI text;
    public Button button;

    void Update()
    {
        var state = BattleManager.Instance.state;

        if (state == GameState.Setup1)
        {
            text.text = "開発準備完了";
        }
        else if (state == GameState.Setup2)
        {
            text.text = "戦闘開始";
        }
        else if (state == GameState.Battle)
        {
            text.text = "戦闘中";
        }
        else if (state == GameState.End)
        {
            text.text = "戦闘中2";
        }

        bool canStartBattle = true;

        if (state == GameState.Setup2)
        {
            BattleUnit[] units =
                FindObjectsByType<BattleUnit>(
                    FindObjectsSortMode.None);

            foreach (BattleUnit unit in units)
            {
                if (unit.isEnemy)
                    continue;

                if (!unit.isDamaged)
                    continue;

                if (unit.currentTile == null)
                    continue;

                if (unit.currentTile.tileType
                    == TileType.Board)
                {
                    canStartBattle = false;
                    break;
                }
            }
        }

        button.interactable =
            state != GameState.Battle
            && canStartBattle;
    }
}