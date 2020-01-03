using UnityEngine;

public class LevelLoadOnAwake : MonoBehaviour
{
    [Tooltip("Which level to load.")]
    public SceneReference nextLevel;

    private void Awake()
    {
        UIController.Instance.LoadLevel(nextLevel);
    }
}
