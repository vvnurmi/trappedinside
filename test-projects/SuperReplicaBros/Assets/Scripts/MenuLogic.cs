using UnityEngine;

public class MenuLogic : MonoBehaviour
{
    [Tooltip("The player to enable/disable based on menus.")]
    public GameObject player;

    new private PlayerCamera camera;

    private void Start()
    {
        Debug.Assert(player != null);
        camera = GetComponentInParent<PlayerCamera>();
        Debug.Assert(camera != null);

        ShowMenu("title");
        HidePlayer();
    }

    private void Update()
    {
        var isStartPressed = Input.GetButtonDown("Jump");
        if (isStartPressed)
        {
            HideMenu();
            ShowPlayer();
        }
    }

    public void HidePlayer()
    {
        camera.player = null;
        player.SetActive(false);
    }

    public void ShowPlayer()
    {
        camera.player = player;
        player.SetActive(true);
    }

    public void HideMenu()
    {
        foreach (Transform child in transform)
        {
            var menu = child.gameObject;
            menu.SetActive(false);
        }
    }

    public void ShowMenu(string name)
    {
        var child = transform.Find(name);
        Debug.Assert(child != null);
        var mode = child.gameObject;
        if (mode.activeSelf) return;

        HideMenu();
        mode.SetActive(true);
    }
}
