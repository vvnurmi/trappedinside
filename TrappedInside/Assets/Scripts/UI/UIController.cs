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

    // Set once at startup.
    private UIControllerConfig config;

    // Modified throughout lifetime.
    private UIMode mode = UIMode.Title;

    public static UIController Instance => host.GetComponent<UIController>();

    public void RestartLevel()
    {
        Debug.Assert(mode == UIMode.Gameplay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Loads a new level and performs any necessary transition.
    /// </summary>
    public void LoadLevel(SceneReference level)
    {
        SceneManager.LoadScene(level.ScenePath);
    }

    #region MonoBehaviour overrides

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void OnBeforeSceneLoadRuntimeMethod()
    {
        Debug.Assert(host == null);
        host = new GameObject("UI Controller", typeof(UIController));
        DontDestroyOnLoad(host);
        SceneManager.sceneLoaded += host.GetComponent<UIController>().OnSceneLoaded;
    }

    private void Awake()
    {
        config = Resources.Load<UIControllerConfig>("UIControllerConfig");
        Debug.Assert(config != null);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
    {
        // A simple heuristic to determine if we loaded a gameplay or title scene.
        if (scene.name == "Title")
            mode = UIMode.Title;
        else if (scene.name.StartsWith("Level") || scene.path.Contains("Demo scenes"))
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
                    SceneManager.LoadScene(config.gameStartScene);
                break;
        }
    }

    #endregion
}
