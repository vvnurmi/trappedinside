﻿using System;
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
        var playerBlocker = new GameObject("Player Blocker");
        playerBlocker.transform.parent = gameObject.transform;
        playerBlocker.layer = LayerMask.NameToLayer("BlockPlayer");
        playerBlock = playerBlocker.AddComponent<BoxCollider2D>();
        playerBlock.enabled = false;
    }

    private void LateUpdate()
    {
        if (player == null) return;

        FollowPlayer();
        RestrictPlayer();
    }

    private void FollowPlayer()
    {
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

        var cameraWorldArea = camera.GetWorldArea();
        playerBlock.size = new Vector2(1, 4 * cameraWorldArea.height);
        playerBlock.offset = new Vector2(-(1 + cameraWorldArea.width) / 2, 0);
    }
}
