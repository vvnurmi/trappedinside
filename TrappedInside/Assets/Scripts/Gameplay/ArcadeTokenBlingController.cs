using UnityEngine;

public class ArcadeTokenBlingController : MonoBehaviour
{
    void OnBlingFinished()
    {
        Destroy(gameObject);
    }
}
