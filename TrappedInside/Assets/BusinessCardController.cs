using UnityEngine;

public class BusinessCardController : MonoBehaviour, ICollectible
{
    public GameObject businessCardEndPrefab;

    public void Collect(CharacterState characterState)
    {
        characterState.collectedBusinessCards++;
        Instantiate(businessCardEndPrefab, transform.position, Quaternion.identity);
        gameObject.SetActive(false);
    }
}
