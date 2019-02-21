using UnityEngine;
using UnityEngine.SceneManagement;

public enum UIMode
{
    Title,
    Gameplay,
}

public class UIController : MonoBehaviour
{
    private UIMode mode = UIMode.Title;

    private void OnEnable()
    {
        if (Lifetime.KillForUniquePersistence<UIController>(gameObject))
            return;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
    {
        // A simple heuristic to determine if we loaded a gameplay or title scene.
        if (scene.name == "Title")
            mode = UIMode.Title;
        else if (scene.name.StartsWith("Level"))
            mode = UIMode.Gameplay;
        else
            Debug.Assert(false, "Scene name heuristic failed");
    }

    private void FixedUpdate()
    {
        if (mode == UIMode.Title)
        {
            bool promptPressed =
                Input.GetButtonDown("Fire1") ||
                Input.GetButtonDown("Jump");
            if (promptPressed)
                SceneManager.LoadScene("Level1");
        }
    }
}
