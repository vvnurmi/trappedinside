using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PlayerCamera : MonoBehaviour
{
    [Tooltip("The player to follow.")]
    public GameObject player;

    [Tooltip("If true, don't move vertically.")]
    public bool lockY;

    [Tooltip("If true, don't move left ever.")]
    public bool dontMoveLeft;

    new private Camera camera;

    private void Start()
    {
        camera = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        FollowPlayer();
        RestrictPlayer();
    }

    private void FollowPlayer()
    {
        var oldCameraPos = camera.transform.position;
        var followedPos = player.transform.position;
        var newCameraPos = new Vector3(
            dontMoveLeft ? Mathf.Max(oldCameraPos.x, followedPos.x) : followedPos.x,
            lockY ? oldCameraPos.y : followedPos.y,
            oldCameraPos.z);
        camera.transform.position = newCameraPos;
    }

    private void RestrictPlayer()
    {
        if (!dontMoveLeft) return;

        var oldPlayerPos = player.transform.position;
        var playerMinInWorld = player.GetComponent<BoxCollider2D>().bounds.min;
        var cameraMinInWorld = camera.ViewportToWorldPoint(Vector3.zero);
        if (playerMinInWorld.x < cameraMinInWorld.x)
            player.transform.position += Vector3.right * (cameraMinInWorld.x - playerMinInWorld.x);
    }
}
