using UnityEngine;
using System.Collections.Generic;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;

    public List<SavedUnitData> savedUnits =
        new List<SavedUnitData>();

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }
}