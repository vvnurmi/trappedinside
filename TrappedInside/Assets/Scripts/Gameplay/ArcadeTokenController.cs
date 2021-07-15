using UnityEngine;

public class ArcadeTokenController : MonoBehaviour
{
    public GameObject arcadeTokenBlingPrefab;

    void OnDisable()
    {
        Instantiate(arcadeTokenBlingPrefab, transform.position, Quaternion.identity);
    }
}
