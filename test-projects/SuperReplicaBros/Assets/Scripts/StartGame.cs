using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    private void Start()
    {
        Invoke("LoadLevel", time: 2.0f);
    }

    private void LoadLevel()
    {
        SceneManager.LoadScene("Level 1");
    }
}
