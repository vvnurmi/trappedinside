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
        else if (IsBusinessCard(collision))
        {
            characterState.collectedBusinessCards++;
            collision.gameObject.SetActive(false);
            UpdateStatusBar();
        }
        else if (IsArcadeToken(collision))
        {
            characterState.collectedArcadeTokens++;
            collision.gameObject.SetActive(false);
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

    private bool IsBusinessCard(Collider2D collision) =>
        collision.gameObject.tag == "BusinessCard";

    private bool IsArcadeToken(Collider2D collision) =>
        collision.gameObject.tag == "ArcadeToken";


}
