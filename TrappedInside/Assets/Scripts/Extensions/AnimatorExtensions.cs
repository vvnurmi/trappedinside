using UnityEngine;

public static class AnimatorExtensions
{
    /// <summary>
    /// Plays the named animation state. If <paramref name="animator"/> is null then does nothing.
    /// </summary>
    public static void TryPlay(this Animator animator, string stateName)
    {
        if (animator == null) return;

        var stateNameHash = Animator.StringToHash(stateName);
        var layerIndex = 0;
        if (animator.HasState(layerIndex, stateNameHash))
            animator.Play(stateNameHash);
        else
            Debug.LogWarning($"Animator in '{animator.gameObject.name}' doesn't have state '{stateName}' on layer {layerIndex}");
    }
}
