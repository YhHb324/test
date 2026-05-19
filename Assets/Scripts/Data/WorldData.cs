using UnityEngine;

[CreateAssetMenu(menuName = "Game/WorldData")]
public class WorldData : ScriptableObject
{
    public string worldName;

    public StageData[] stages;
}