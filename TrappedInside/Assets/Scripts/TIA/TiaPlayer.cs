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

    #region MonoBehaviour overrides

    private void Start()
    {
        context = new TiaActionContext(
            scriptRunner: this,
            tiaRoot: gameObject);
        IsPlaying = script.PlayOnStart;
        stepIndex = 0;
        if (stepIndex < script.Steps.Length)
            script.Steps[stepIndex].Start(context);
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
