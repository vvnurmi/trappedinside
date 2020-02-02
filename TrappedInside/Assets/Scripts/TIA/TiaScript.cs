using System;
using System.Linq;
using UnityEngine;

/// <summary>
/// Plays steps in sequence.
/// TIA = Trapped Inside Animation
/// </summary>
public class TiaScript : MonoBehaviour
{
    [Tooltip("Human-readable name for identification.")]
    public string scriptName;

    [Tooltip("If true then start executing steps immediately.")]
    public bool playOnStart;

    public TiaStep[] steps;

    public bool IsPlaying { get; private set; }

    private int stepIndex;

    private void Start()
    {
        IsPlaying = playOnStart;
        stepIndex = 0;
        if (stepIndex < steps.Length)
            steps[stepIndex].Start(tiaRoot: gameObject);
    }

    private void Update()
    {
        if (!IsPlaying) return;

        while (stepIndex < steps.Length)
        {
            if (!steps[stepIndex].IsDone)
                steps[stepIndex].Update();
            if (!steps[stepIndex].IsDone)
                break;

            stepIndex++;
            if (stepIndex < steps.Length)
                steps[stepIndex].Start(tiaRoot: gameObject);
        }

        IsPlaying = stepIndex < steps.Length;
    }
}

/// <summary>
/// Plays all actions simultaneously, then waits.
/// </summary>
[Serializable]
public class TiaStep
{
    public TiaActionSequence[] sequences;

    public bool IsDone => sequences.All(seq => seq.IsDone);

    public void Start(GameObject tiaRoot)
    {
        foreach (var seq in sequences)
            seq.Start(tiaRoot);
    }

    public void Update()
    {
        foreach (var seq in sequences)
            seq.Update();
    }
}

/// <summary>
/// A sequence of actions that one actor does.
/// </summary>
[Serializable]
public class TiaActionSequence
{
    public TiaActor actor;

    public ITiaAction[] actions;

    public bool IsDone => actionIndex >= actions.Length;

    private int actionIndex;

    public void Start(GameObject tiaRoot)
    {
        actor.Initialize(tiaRoot);
        actionIndex = 0;
        if (actionIndex < actions.Length)
            actions[actionIndex].Start();
    }

    public void Update()
    {
        if (IsDone) return;

        while (actionIndex < actions.Length)
        {
            if (!actions[actionIndex].IsDone)
                actions[actionIndex].Update(actor);
            if (!actions[actionIndex].IsDone)
                break;

            actionIndex++;
            if (actionIndex < actions.Length)
                actions[actionIndex].Start();
        }
    }
}

/// <summary>
/// Associated with a Unity GameObject under the root TIA game object by name.
/// GameObject may need Animator component.
/// Serialized as a name string.
/// </summary>
[Serializable]
public class TiaActor
{
    public string gameObjectName;

    public GameObject GameObject
    {
        get
        {
            if (gameObject == null)
                throw new InvalidOperationException($"{nameof(TiaActor)} has no game object, maybe {nameof(Initialize)} wasn't called?");
            return gameObject;
        }
    }

    private GameObject gameObject;

    /// <summary>
    /// To be called before commencing <see cref="TiaActionSequence"/>.
    /// </summary>
    public void Initialize(GameObject tiaRoot)
    {
        gameObject = tiaRoot.FindChildByName(gameObjectName);
        Debug.Assert(gameObject != null, $"{nameof(TiaActor)} couldn't find '{gameObjectName}' under {tiaRoot.GetFullName()}");
    }
}

/// <summary>
/// An action that an actor can take as part of an action sequence.
/// </summary>
public interface ITiaAction
{
    bool IsDone { get; }

    /// <summary>
    /// Called when the action starts.
    /// </summary>
    void Start();

    /// <summary>
    /// Called regularly after start until <see cref="IsDone"/>
    /// </summary>
    void Update(TiaActor actor);
}

/// <summary>
/// Activates or deactivates the actor.
/// </summary>
[Serializable]
public class TiaActivation : ITiaAction
{
    public bool activated;

    public bool IsDone { get; private set; }

    public void Start()
    {
    }

    public void Update(TiaActor actor)
    {
        actor.GameObject.SetActive(activated);
        IsDone = true;
    }
}

/// <summary>
/// Moves actor along a curve over a given time
/// </summary>
[Serializable]
public class TiaMove : ITiaAction
{
    public BezierCurve curve;
    public float durationSeconds;

    public bool IsDone { get; private set; }

    private float startTime;

    public void Start()
    {
        IsDone = false;
        startTime = Time.time;
    }

    public void Update(TiaActor actor)
    {
        RepositionOnCurve(actor.GameObject);

        IsDone = Time.time >= startTime + durationSeconds;
    }

    private void RepositionOnCurve(GameObject obj)
    {
        float flightTime = Time.time - startTime;
        float curveParam = Mathf.InverseLerp(0, durationSeconds, flightTime);
        var pathPosition = curve.GetPointAt(curveParam);

        obj.transform.SetPositionAndRotation(pathPosition, obj.transform.rotation);
    }
}

/// <summary>
/// Makes actor use this animation.
/// </summary>
[Serializable]
public class TiaAnimation : ITiaAction
{
    public string animationName;

    public bool IsDone => throw new NotImplementedException();

    public void Start()
    {
        throw new NotImplementedException();
    }

    public void Update(TiaActor actor)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Types text in a speech bubble on top of actor.
/// </summary>
[Serializable]
public class TiaSpeech : ITiaAction
{
    public string tmpRichText;
    public float typingSpeedMultiplier;

    [Tooltip("If true then prompt player after text, otherwise just wait for a while.")]
    public bool modal;

    public bool IsDone => throw new NotImplementedException();

    public void Start()
    {
        throw new NotImplementedException();
    }

    public void Update(TiaActor actor)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Waits for given time until continuing to the next action.
/// </summary>
[Serializable]
public class TiaPause : ITiaAction
{
    public float durationSeconds;

    public bool IsDone { get; private set; }

    private float finishTime;

    public void Start()
    {
        finishTime = Time.time + durationSeconds;
    }

    public void Update(TiaActor actor)
    {
        if (IsDone) return;

        IsDone = Time.time >= finishTime;
    }
}

/// <summary>
/// Starts to play another TIA script in the same root TIA game object.
/// </summary>
[Serializable]
public class TiaPlayScript : ITiaAction
{
    public string scriptName;

    public bool IsDone => throw new NotImplementedException();

    public void Start()
    {
        throw new NotImplementedException();
    }

    public void Update(TiaActor actor)
    {
        throw new NotImplementedException();
    }
}
