using UnityEngine;

/// <summary>
/// Settings for <see cref="NarrativeTypist"/> components.
/// The settings should reside in a parent object of the objects that contain
/// the actual typists.
/// </summary>
public class NarrativeTypistSettings : MonoBehaviour
{
    [Tooltip("How many characters to type per second.")]
    public float charsPerSecond = 10;

    [Tooltip("The sound to play when typing a character.")]
    public AudioClip characterSound;

    [Tooltip("Where to play the typing sound.")]
    public AudioSource audioSource;
}
