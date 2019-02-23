using UnityEngine;
using UnityEngine.SceneManagement;

public enum UIMode
{
    Unknown,
    Title,
    Gameplay,
}

public class UIController : MonoBehaviour
{
    private static GameObject host;

    private UIMode mode = UIMode.Title;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void OnBeforeSceneLoadRuntimeMethod()
    {
        Debug.Assert(host == null);
        host = new GameObject("UI Controller", typeof(UIController));
        DontDestroyOnLoad(host);
        SceneManager.sceneLoaded += host.GetComponent<UIController>().OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
    {
        // A simple heuristic to determine if we loaded a gameplay or title scene.
        if (scene.name == "Title")
            mode = UIMode.Title;
        else if (scene.name.StartsWith("Level"))
            mode = UIMode.Gameplay;
        else
        {
            mode = UIMode.Unknown;
            Debug.Assert(false, $"Scene name heuristic couldn't identify scene '{scene.name}'");
        }
    }

    private void FixedUpdate()
    {
        switch (mode)
        {
            case UIMode.Title:
                bool promptPressed =
                    Input.GetButtonDown("Fire1") ||
                    Input.GetButtonDown("Jump");
                if (promptPressed)
                    SceneManager.LoadScene("Level1");
                break;
        }
    }
}
