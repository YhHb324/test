using UnityEngine;

public class BoardManager : MonoBehaviour
{

    public GameObject tilePrefab;

    int width = 7;
    int height = 8;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3 pos = new Vector3 (x, y, 0);

                Instantiate(tilePrefab, pos, Quaternion.identity);
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
