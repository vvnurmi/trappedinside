using UnityEngine;

/// <summary>
/// An action that an actor can take as part of an action sequence.
/// </summary>
[System.Obsolete("Use ITiaActionNew and rename it to ITiaAction")]
public interface ITiaAction
{
    bool IsDone { get; }

    /// <summary>
    /// Called when the action starts.
    /// </summary>
    void Start(GameObject tiaRoot);

    /// <summary>
    /// Called regularly after start until <see cref="IsDone"/>
    /// </summary>
    void Update(TiaActor actor);
}

/// <summary>
/// An action that an actor can take as part of an action sequence.
/// </summary>
public interface ITiaActionNew
{
    bool IsDone { get; }

    /// <summary>
    /// Called when the action starts.
    /// </summary>
    void Start(ITiaActionContext context);

    /// <summary>
    /// Called regularly after start until <see cref="IsDone"/>
    /// </summary>
    void Update(ITiaActionContext context);
}
