using UnityEngine;

public static class AudioSourceExtensions
{
    /// <summary>
    /// Plays the named sound effect. If <paramref name="audioSource"/> is null then does nothing.
    /// </summary>
    public static void TryPlay(this AudioSource audioSource, AudioClip sound)
    {
        if (audioSource == null) return;

        if (sound != null)
            audioSource.PlayOneShot(sound);
        else
            Debug.LogWarning($"AudioSource in '{audioSource.gameObject.name}' tried to play null sound");
    }
}
