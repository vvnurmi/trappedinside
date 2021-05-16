using UnityEngine;
using UnityEngine.UI;

public class TokenHandler : MonoBehaviour
{
    public Text numberOfTokens;

    public void SetNumberOfTokens(int value)
    {
        numberOfTokens.text = value.ToString();
    }
}
