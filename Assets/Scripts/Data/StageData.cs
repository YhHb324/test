using UnityEngine;

[CreateAssetMenu(menuName = "Game/StageData")]
public class StageData : ScriptableObject
{
    public string stageName;

    public EnemySpawnData[] enemies;
}