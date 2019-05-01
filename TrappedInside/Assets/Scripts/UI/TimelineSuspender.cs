using UnityEngine;
using UnityEngine.Playables;

/// <summary>
/// Suspends a timeline while the gameObject is active.
/// </summary>
public class TimelineSuspender : MonoBehaviour
{
    public PlayableDirector timeline;

    /// <summary>
    /// Safety time, in seconds, after resume when we won't pause the timeline again.
    /// This is a workaround to how Timeline likes to play the paused frame
    /// again after resume, resulting in an endless pause-resume loop.
    /// </summary>
    private const float ResumeClearance = 1;

    private double timelinePauseTime;
    private float resumeTime = float.MinValue;

    private void OnEnable()
    {
        if (timeline != null && Time.time > resumeTime + ResumeClearance)
            timelinePauseTime = timeline.time;
    }

    private void Update()
    {
        // Hack: Keep timeline's time in place. This is better than calling
        // timeline.Pause() because pause will revert all values that the timeline
        // has modified back to their original values.
        timeline.time = timelinePauseTime;
    }

    private void OnDisable()
    {
        if (timeline != null)
            resumeTime = Time.time;
    }
}
