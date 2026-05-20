using UnityEngine;

[CreateAssetMenu(menuName = "Game/WorldData")]
public class WorldData : ScriptableObject
{
    public string worldName;

    public int maxBoardUnits = 3;

    public StageData[] stages;
}