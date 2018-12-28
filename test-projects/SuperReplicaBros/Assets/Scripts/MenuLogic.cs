using UnityEngine;
using UnityEngine.SceneManagement;

public enum MenuMode
{
    Title,
    Gameplay,
}

public class MenuLogic : MonoBehaviour
{
    public float levelTitleDelay = 3.0f;
    public float deathDelay = 4.0f;
    public float levelCompletionDelay = 5.0f;

    private MenuMode mode = MenuMode.Title;

    private void OnEnable()
    {
        if (DieForUniquePersistence())
            return;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        SceneManager.LoadScene("Game Title", LoadSceneMode.Additive);

    }

    private void Update()
    {
        var isStartPressed = Input.GetButtonDown("Jump");
        if (mode == MenuMode.Title && isStartPressed)
            BeginGameplay();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
    {
        // Look for the player and hook its death event.
        // Note that it doesn't exist in title screens, etc.
        var player = TryFindPlayer();
        if (player == null) return;

        var playerController = player.GetComponent<PlayerController>();
        Debug.Assert(playerController != null);
        playerController.Death += OnPlayerDeath;

        if (mode == MenuMode.Title)
            HidePlayer();
    }

    private void OnPlayerDeath()
    {
        SceneManager.LoadScene("Game Over", LoadSceneMode.Additive);
        Invoke(nameof(ReturnToGameTitle), time: levelTitleDelay);
    }

    private void ReturnToGameTitle()
    {
        mode = MenuMode.Title;
        SceneManager.LoadScene("Level 1");
        SceneManager.LoadScene("Game Title", LoadSceneMode.Additive);
    }

    private void HidePlayer()
    {
        FindCamera().player = null;
        FindPlayer().SetActive(false);
    }

    private void BeginGameplay()
    {
        mode = MenuMode.Gameplay;
        SceneManager.LoadScene("Level Title");
        Invoke(nameof(LoadLevel), time: levelTitleDelay);
    }

    private void LoadLevel()
    {
        SceneManager.LoadScene("Level 1");
    }

    private PlayerCamera FindCamera()
    {
        var cameraObject = GameObject.FindGameObjectWithTag("MainCamera");
        Debug.Assert(cameraObject != null);
        var camera = cameraObject.GetComponent<PlayerCamera>();
        Debug.Assert(camera != null);
        return camera;
    }

    private GameObject TryFindPlayer()
    {
        return GameObject.FindGameObjectWithTag("Player");
    }

    private GameObject FindPlayer()
    {
        var player = TryFindPlayer();
        Debug.Assert(player != null);
        return player;
    }

    public void CompleteLevel()
    {
        SceneManager.LoadScene("Level Complete", LoadSceneMode.Additive);
        FindPlayer().GetComponent<PlayerController>().DisableControls();
        FindCamera().GetComponent<PlayerCamera>().PlayLevelCompleteSound();
        Invoke(nameof(BeginGameplay), time: levelCompletionDelay);
    }

    /// <summary>
    /// Ensures that only one object with this component exists, and the object
    /// lives over scene transitions. Returns true if we're a duplicate that
    /// will die immediately.
    /// </summary>
    private bool DieForUniquePersistence()
    {
        // Make object not get destroyed when a new scene is loaded.
        DontDestroyOnLoad(this);

        // If one of us already exists, don't let any more in.
        var others = FindObjectsOfType<MenuLogic>();
        Debug.Assert(others.Length >= 1);
        if (others.Length == 1) return false;

        Destroy(gameObject);
        return true;
    }
}
