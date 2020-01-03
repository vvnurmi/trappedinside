using UnityEngine;

[CreateAssetMenu(fileName = "UIControllerConfig", menuName = "System/UIControllerConfig")]
public class UIControllerConfig : ScriptableObject
{
    [Tooltip("The first scene to load when the game starts.")]
    public SceneReference gameStartScene;

    [Tooltip("Something to cover the screen during scene transitions.")]
    public GameObject screenCover;
}
