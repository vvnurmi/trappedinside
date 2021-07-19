using UnityEngine;

/// <summary>
/// Plays <see cref="TiaScript"/> step by step in sequence.
/// TIA = Trapped Inside Animation
/// </summary>
public class TiaPlayer : MonoBehaviour
{
    public TiaScript script;

    public bool IsPlaying { get; private set; }

    private TiaActionContext context;
    private int stepIndex;

    /// <summary>
    /// Starts to play <paramref name="script"/>.
    /// If another script is already playing, it's abandoned.
    /// </summary>
    /// <param name="pauseImmediately">If true, playback will remain paused
    /// until <see cref="IsPlaying"/> is set.</param>
    public void Play(TiaScript script, bool pauseImmediately = false)
    {
        Debug.Assert(script != null);

        this.script = script;
        context = new TiaActionContext(
            scriptRunner: this,
            tiaRoot: gameObject);
        IsPlaying = !pauseImmediately;
        stepIndex = 0;
        if (stepIndex < script.Steps.Length)
            script.Steps[stepIndex].Start(context);
    }

    #region MonoBehaviour overrides

    private void Start()
    {
        if (script != null)
            Play(script, pauseImmediately: !script.PlayOnStart);
    }

    private void Update()
    {
        if (!IsPlaying) return;

        while (stepIndex < script.Steps.Length)
        {
            if (!script.Steps[stepIndex].IsDone(context))
                script.Steps[stepIndex].Update(context);
            if (!script.Steps[stepIndex].IsDone(context))
                break;

            stepIndex++;
            if (stepIndex < script.Steps.Length)
                script.Steps[stepIndex].Start(context);
        }

        IsPlaying = stepIndex < script.Steps.Length;
    }

    #endregion
}
