using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    private void FixedUpdate()
    {
        bool promptPressed =
            Input.GetButtonDown("Fire1") ||
            Input.GetButtonDown("Jump");

        if (promptPressed)
            SceneManager.LoadScene("Level1");
    }
}
