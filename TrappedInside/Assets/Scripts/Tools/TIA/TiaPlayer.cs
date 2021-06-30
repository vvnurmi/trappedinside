using UnityEngine;

/// <summary>
/// Plays <see cref="TiaScript"/> step by step in sequence.
/// TIA = Trapped Inside Animation
/// </summary>
public class TiaPlayer : MonoBehaviour
{
    public string scriptName;
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

    private async void Start()
    {
        Debug.Assert(script != null || !string.IsNullOrEmpty(scriptName), $"No script assigned to {nameof(TiaPlayer)}");
        var scriptToPlay = script ?? await TiaScriptManager.Instance.Get(scriptName);
        Play(scriptToPlay, pauseImmediately: !script.PlayOnStart);
    }

    private void Update()
    {
        if (!IsPlaying) return;

        while (stepIndex < script.Steps.Length)
        {
            if (!script.Steps[stepIndex].IsDone)
                script.Steps[stepIndex].Update(context);
            if (!script.Steps[stepIndex].IsDone)
                break;

            stepIndex++;
            if (stepIndex < script.Steps.Length)
                script.Steps[stepIndex].Start(context);
        }

        IsPlaying = stepIndex < script.Steps.Length;
    }

    #endregion
}
