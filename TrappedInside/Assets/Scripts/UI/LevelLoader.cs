﻿using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    [Tooltip("Which level to load on wakeup.")]
    public string nextLevel;

    private void Awake()
    {
        SceneManager.LoadScene(nextLevel);
    }
}