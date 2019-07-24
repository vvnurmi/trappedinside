using UnityEditor;
using UnityEngine;

public class LevelLoadOnTriggerEnter : MonoBehaviour
{
    [Tooltip("Which level to load.")]
    public SceneAsset nextLevel;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        UIController.Instance.LoadLevel(nextLevel);
    }
}
