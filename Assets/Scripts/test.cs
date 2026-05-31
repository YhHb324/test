using UnityEngine;

public class Test : MonoBehaviour
{
    void Start()
    {
        Debug.Log(
            GetComponent<SpriteRenderer>()
            .bounds.size.y);
    }
}