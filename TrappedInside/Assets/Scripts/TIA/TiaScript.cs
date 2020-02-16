using System;
using System.IO;
using System.Linq;
using UnityEngine;
using YamlDotNet.Serialization;

/// <summary>
/// Plays <see cref="TiaScript"/> step by step in sequence.
/// TIA = Trapped Inside Animation
/// </summary>
public class TiaPlayer : MonoBehaviour
{
    public TiaScript script;

    public bool IsPlaying { get; private set; }

    private int stepIndex;

    #region MonoBehaviour overrides

    private void Start()
    {
        IsPlaying = script.playOnStart;
        stepIndex = 0;
        if (stepIndex < script.steps.Length)
            script.steps[stepIndex].Start(tiaRoot: gameObject);
    }

    private void Update()
    {
        if (!IsPlaying) return;

        while (stepIndex < script.steps.Length)
        {
            if (!script.steps[stepIndex].IsDone)
                script.steps[stepIndex].Update();
            if (!script.steps[stepIndex].IsDone)
                break;

            stepIndex++;
            if (stepIndex < script.steps.Length)
                script.steps[stepIndex].Start(tiaRoot: gameObject);
        }

        IsPlaying = stepIndex < script.steps.Length;
    }

    #endregion
}

public class TiaScript
{
    /// <summary>
    /// Human-readable name for identification.
    /// </summary>
    public string scriptName { get; set; }

    /// <summary>
    /// If true then start executing steps immediately.
    /// </summary>
    public bool playOnStart { get; set; }

    public TiaStep[] steps { get; set; }

    /// <summary>
    /// Creates a new <see cref="TiaScript"/> for serialized form.
    /// </summary>
    public static TiaScript Read(string serialized)
    {
        var input = new StringReader(serialized);

        var deserializer = new DeserializerBuilder()
            .Build();

        return deserializer.Deserialize<TiaScript>(input);
    }
}

/// <summary>
/// Plays all actions simultaneously, then waits.
/// </summary>
public class TiaStep
{
    public TiaActionSequence[] sequences { get; set; }

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
public class TiaActionSequence
{
    public TiaActor actor { get; set; }

    public ITiaAction[] actions { get; set; }

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
public class TiaActor
{
    public string gameObjectName { get; set; }

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
public class TiaActivation : ITiaAction
{
    public bool activated { get; set; }

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
public class TiaMove : ITiaAction
{
    public BezierCurve curve { get; set; }
    public float durationSeconds { get; set; }

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
public class TiaAnimation : ITiaAction
{
    public string animationName { get; set; }

    public bool IsDone { get; private set; }

    public void Start()
    {
    }

    public void Update(TiaActor actor)
    {
        var animator = actor.GameObject.GetComponent<Animator>();
        animator.Play(animationName);
        IsDone = true;
    }
}

/// <summary>
/// Types text in a speech bubble on top of actor.
/// </summary>
public class TiaSpeech : ITiaAction
{
    /// <summary>
    /// TextMesh Pro rich text to display in the speech bubble.
    /// </summary>
    public string tmpRichText { get; set; }
    public float typingSpeedMultiplier { get; set; }

    /// <summary>
    /// If true then prompt player after text, otherwise just wait for a while.
    /// </summary>
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
public class TiaPause : ITiaAction
{
    public float durationSeconds { get; set; }

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
public class TiaPlayScript : ITiaAction
{
    public string scriptName { get; set; }

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
