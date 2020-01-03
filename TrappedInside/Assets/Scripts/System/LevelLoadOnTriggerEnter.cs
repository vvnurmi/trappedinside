using UnityEngine;

public class LevelLoadOnTriggerEnter : MonoBehaviour
{
    [Tooltip("Which level to load.")]
    public SceneReference nextLevel;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        UIController.Instance.LoadLevel(nextLevel);
    }
}
