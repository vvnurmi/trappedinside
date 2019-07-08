using UnityEngine;
using UnityEngine.UI;

public class HitPointBarController : MonoBehaviour
{
    public Image[] hitPointImages;
    public Sprite fullHeart;
    public Sprite emptyHeart;
    public HitPoints hitPoints;
    
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
