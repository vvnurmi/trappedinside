using UnityEngine;

/// <summary>
/// Settings for <see cref="NarrativeTypist"/> components.
/// The settings should reside in a parent object of the objects that contain
/// the actual typists.
/// </summary>
[CreateAssetMenu(fileName = "NarrativeTypistSettings", menuName = "System/NarrativeTypistSettings")]
public class NarrativeTypistSettings : ScriptableObject
{
    [Tooltip("How many characters to type per second.")]
    public float charsPerSecond = 10;

    [Tooltip("The sound to play when typing a character.")]
    public AudioClip characterSound;
}
