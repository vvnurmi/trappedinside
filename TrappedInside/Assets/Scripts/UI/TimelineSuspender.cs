using System;
using UnityEngine;
using UnityEngine.Playables;

/// <summary>
/// Suspends a timeline while the gameObject is active.
/// </summary>
public class TimelineSuspender : MonoBehaviour
{
    /// <summary>
    /// Safety time, in seconds, after resume when we won't pause the timeline again.
    /// This is a workaround to how Timeline likes to play the paused frame
    /// again after resume, resulting in an endless pause-resume loop.
    /// </summary>
    private const float ResumeClearance = 1;

    // Set about once, probably in Start().
    private Lazy<TimelineSuspenderSettings> settings;

    // Modified during gameplay.
    private double timelinePauseTime;
    private float resumeTime = float.MinValue;

    public TimelineSuspender()
    {
        settings = new Lazy<TimelineSuspenderSettings>(() =>
        {
            var settings = GetComponentInParent<TimelineSuspenderSettings>();
            Debug.Assert(settings != null,
                $"Expected to find {nameof(TimelineSuspenderSettings)} from the parent of {nameof(TimelineSuspender)}");
            return settings;
        });
    }

    private void OnEnable()
    {
        if (settings.Value.timeline != null && Time.time > resumeTime + ResumeClearance)
            timelinePauseTime = settings.Value.timeline.time;
    }

    private void Update()
    {
        // Hack: Keep timeline's time in place. This is better than calling
        // timeline.Pause() because pause will revert all values that the timeline
        // has modified back to their original values.
        settings.Value.timeline.time = timelinePauseTime;
    }

    private void OnDisable()
    {
        if (settings.Value.timeline != null)
            resumeTime = Time.time;
    }
}
