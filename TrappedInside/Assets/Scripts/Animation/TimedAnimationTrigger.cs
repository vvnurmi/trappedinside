using UnityEngine;

/// <summary>
/// Animation trigger that resets after a time.
/// </summary>
public struct TimedAnimationTrigger
{
    public readonly Animator animator;
    public readonly string trigger;
    public readonly float resetTimeout;

    private float resetTime;

    public TimedAnimationTrigger(Animator animator, string trigger, float timeout)
    {
        this.animator = animator;
        this.trigger = trigger;
        resetTimeout = timeout;
        resetTime = 0;
    }

    public void Set()
    {
        animator.SetTrigger(trigger);
        resetTime = Time.time + resetTimeout;
    }

    public void Update()
    {
        if (Time.time < resetTime) return;

        resetTime = 0;
        animator.ResetTrigger(trigger);
    }
}
