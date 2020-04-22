using UnityEngine;
using UnityEngine.UI;

public class CardHandler : MonoBehaviour
{
    public Text numberOfCards;

    public void SetNumberOfCards(int value)
    {
        numberOfCards.text = value.ToString();
    }
}
