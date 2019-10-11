using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Updates the display of the player's hit points.
/// </summary>
public class HitPointBarController : MonoBehaviour
{
    [Tooltip("HUD for gameplay.")]
    public GameObject gameplayHud;

    public Sprite fullHeart;
    public Sprite emptyHeart;

    private Image[] hitPointImages;
    private HitPoints hitPoints;

    private void Awake()
    {
        var hud = Instantiate(gameplayHud);
        hitPointImages = hud.transform.GetComponentsInChildren<Image>();

        var playerTag = "Player";
        var player = GameObject.FindWithTag(playerTag);
        Debug.Assert(player != null, $"{nameof(HitPointBarController)} couldn't find anything tagged {playerTag}");
        hitPoints = player.GetComponent<HitPoints>();
        Debug.Assert(hitPoints != null, $"{nameof(HitPointBarController)} couldn't find {nameof(HitPoints)} on the player");
    }

    void Update()
    {
        for(int i = 0; i < hitPointImages.Length; i++)
        {
            hitPointImages[i].sprite = 
                hitPoints.CurrentHitPoints > i 
                ? fullHeart
                : emptyHeart;
        }
    }
}
