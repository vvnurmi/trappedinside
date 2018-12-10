using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PlayerCamera : MonoBehaviour
{
    [Tooltip("The player to follow, or null.")]
    public GameObject player;

    [Tooltip("If true, don't move vertically.")]
    public bool lockY;

    [Tooltip("If true, don't move left ever and don't let the player move left of the view.")]
    public bool blockMoveLeft;

    new private Camera camera;
    private BoxCollider2D playerBlock;

    private void Start()
    {
        camera = GetComponent<Camera>();

        // Create a box collider that can prevent the player from moving left.
        gameObject.layer = LayerMask.NameToLayer("BlockPlayer");
        playerBlock = gameObject.AddComponent<BoxCollider2D>();
        playerBlock.enabled = false;
    }

    private void LateUpdate()
    {
        FollowPlayer();
        RestrictPlayer();
    }

    private void FollowPlayer()
    {
        if (player == null) return;

        var oldCameraPos = camera.transform.position;
        var followedPos = player.transform.position;
        var newCameraPos = new Vector3(
            blockMoveLeft ? Mathf.Max(oldCameraPos.x, followedPos.x) : followedPos.x,
            lockY ? oldCameraPos.y : followedPos.y,
            oldCameraPos.z);
        camera.transform.position = newCameraPos;
    }

    private void RestrictPlayer()
    {
        playerBlock.enabled = blockMoveLeft;
        if (!blockMoveLeft) return;

        var cameraWorldMin = camera.ViewportToWorldPoint(Vector3.zero);
        var cameraWorldMax = camera.ViewportToWorldPoint(Vector3.one);
        var cameraWorldSize = cameraWorldMax - cameraWorldMin;
        playerBlock.size = new Vector2(1, 4 * cameraWorldSize.y);
        playerBlock.offset = new Vector2(-(1 + cameraWorldSize.x) / 2, 0);
    }
}
