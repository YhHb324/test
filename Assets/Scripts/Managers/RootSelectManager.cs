using UnityEngine;

public class RouteSelectManager : MonoBehaviour
{
    public Transform choiceRoot;

    public GameObject choicePrefab;

    void Start()
    {
        WorldData current =
            StageManager.Instance.CurrentWorld();

        foreach (WorldData nextWorld
            in current.nextWorlds)
        {
            GameObject obj =
                Instantiate(
                    choicePrefab,
                    choiceRoot);

            ChoiceUI ui =
                obj.GetComponent<ChoiceUI>();

            ui.Setup(nextWorld);
        }
    }
}