using UnityEngine;
using UnityEngine.UI;

public class LifeHandler : MonoBehaviour
{
    private Image[] hearts;

    private void Start()
    {
        hearts = GetComponentsInChildren<Image>();
    }

    public void SetNumberOfHearts(int value)
    {
        Debug.Assert(hearts.Length <= value);
        DisableHearts();
        for (int i = 0; i < value; i++)
        {
            hearts[i].enabled = true;
        }
    }

    private void DisableHearts()
    {
        foreach (var heart in hearts)
        {
            heart.enabled = false;
        }
    }
}
