using UnityEngine;

public class ArcadeTokenController : MonoBehaviour, ICollectible
{
    public GameObject arcadeTokenBlingPrefab;

    public void Collect(CharacterState characterState)
    {
        characterState.collectedArcadeTokens++;
        Instantiate(arcadeTokenBlingPrefab, transform.position, Quaternion.identity);
        gameObject.SetActive(false);
    }
}
