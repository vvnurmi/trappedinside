using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Updates the display of the player's hit points.
/// </summary>
public class StatusBarController : MonoBehaviour
{
    [Tooltip("HUD for gameplay.")]
    public GameObject gameplayHud;

    private TokenHandler tokenHandler;
    private LifeHandler lifeHandler;
    private CardHandler cardHandler;


    private void Awake()
    {
        var hud = Instantiate(gameplayHud);

        tokenHandler = hud.GetComponentInChildren<TokenHandler>();
        lifeHandler = hud.GetComponentInChildren<LifeHandler>();
        cardHandler = hud.GetComponentInChildren<CardHandler>();
    }

    public void SetNumberOfTokens(int numberOfTokens)
    {
        tokenHandler.SetNumberOfTokens(numberOfTokens);
    }

    public void SetNumberOfCards(int numberOfCards)
    {
        cardHandler.SetNumberOfCards(numberOfCards);
    }

    public void SetNumberOfHearts(int numberOfHearts, bool createParticleEffect)
    {
        lifeHandler.SetNumberOfHearts(numberOfHearts, createParticleEffect);
    }

}
