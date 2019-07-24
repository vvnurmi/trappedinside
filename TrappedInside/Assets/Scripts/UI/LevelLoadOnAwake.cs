using UnityEditor;
using UnityEngine;

public class LevelLoadOnAwake : MonoBehaviour
{
    [Tooltip("Which level to load.")]
    public SceneAsset nextLevel;

    private void Awake()
    {
        UIController.Instance.LoadLevel(nextLevel);
    }
}
