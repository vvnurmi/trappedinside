using UnityEngine;

public class HitAreaCollisonHandler : MonoBehaviour
{
    private CharacterState characterState;
    private StatusBarController statusBarController;

    void Start()
    {
        characterState = GetComponentInParent<CharacterState>();
        statusBarController = GetComponentInParent<StatusBarController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsLadder(collision))
        {
            characterState.canClimb = true;
            Debug.Log("Ladder enter.");
        }

        var collectible = collision.gameObject.GetComponent<ICollectible>();
        if(collectible != null)
        {
            collectible.Collect(characterState);
            UpdateStatusBar();
        }
    }

    private void UpdateStatusBar()
    {
        if (statusBarController != null)
        {
            statusBarController.SetNumberOfCards(characterState.collectedBusinessCards);
            statusBarController.SetNumberOfTokens(characterState.collectedArcadeTokens);
        }
        else
        {
            Debug.LogWarning("Status bar controller not set.");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (IsLadder(collision))
        {
            characterState.canClimb = false;
            Debug.Log("Ladder exit.");
        }
    }

    private bool IsLadder(Collider2D collision) =>
        collision.gameObject.layer == LayerMask.NameToLayer("Ladder");


}
