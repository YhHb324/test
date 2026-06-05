using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance;

    public int[] itemCounts =
    {
        0,
        0,
        0,
        0,
        0,
        0
    };

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

    void Start()
    {
        if (DevelopmentManager.Instance.debugMode)
        {
            for (int i = 0; i < itemCounts.Length; i++)
            {
                itemCounts[i] = 10;
            }
        }
    }

}