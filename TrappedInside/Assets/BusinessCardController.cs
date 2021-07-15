using UnityEngine;

public class BusinessCardController : MonoBehaviour, ICollectible
{
    public void Collect(CharacterState characterState)
    {
        characterState.collectedBusinessCards++;
        gameObject.SetActive(false);
    }
}
