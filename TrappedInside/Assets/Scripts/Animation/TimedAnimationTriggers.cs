using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Functionality to set animation triggers that reset after a time.
/// </summary>
public struct TimedAnimationTriggers
{
    public readonly Animator animator;
    public readonly float resetTimeout;

    private struct SetTrigger
    {
        public string name;
        public float resetTime;

        public SetTrigger(string name, float resetTime)
        {
            this.name = name;
            this.resetTime = resetTime;
        }
    }
    private List<SetTrigger> setTriggers;

    public TimedAnimationTriggers(Animator animator, float timeout)
    {
        this.animator = animator;
        resetTimeout = timeout;

        setTriggers = new List<SetTrigger>();
    }

    /// <summary>
    /// Set an animation trigger. It will be reset after the timeout.
    /// </summary>
    public void Set(string trigger)
    {
        animator.SetTrigger(trigger);
        setTriggers.Add(new SetTrigger(trigger, Time.time + resetTimeout));
    }

    /// <summary>
    /// To be called from the owning script's Update.
    /// </summary>
    public void Update()
    {
        // Concern: Potentially slow removal from List.
        // In practice there should mostly be 0 or 1 elements, so no worry.
        Debug.Assert(setTriggers.Count < 3);
        while (setTriggers.Count > 0 && setTriggers[0].resetTime <= Time.time)
        {
            animator.ResetTrigger(setTriggers[0].name);
            setTriggers.RemoveAt(0);
        }
    }
}
