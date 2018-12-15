using UnityEngine;
using UnityEngine.SceneManagement;

enum MenuMode
{
    Title,
    Gameplay,
}

public class MenuLogic : MonoBehaviour
{
    private MenuMode mode = MenuMode.Title;

    void OnEnable()
    {
        if (DieForUniquePersistence())
            return;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
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
        mode = MenuMode.Title;
        SceneManager.LoadScene("Game Over", LoadSceneMode.Additive);
        Invoke("ReturnToGameTitle", time: 2.0f);
    }

    private void ReturnToGameTitle()
    {
        SceneManager.LoadScene("Level 1");
        SceneManager.LoadScene("Game Title", LoadSceneMode.Additive);
    }

    public void HidePlayer()
    {
        FindCamera().player = null;
        FindPlayer().SetActive(false);
    }

    private void BeginGameplay()
    {
        mode = MenuMode.Gameplay;
        SceneManager.LoadScene("Level Title");
        Invoke("LoadLevel", time: 2.0f);
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
