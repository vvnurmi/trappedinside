﻿using UnityEngine;

/// <summary>
/// An action that an actor can take as part of an action sequence.
/// </summary>
public interface ITiaAction
{
    string DebugName { get; set; }

    /// <summary>
    /// Returns true if the action has finished.
    /// </summary>
    bool IsDone(ITiaActionContext context);

    /// <summary>
    /// Called when the action starts.
    /// </summary>
    void Start(ITiaActionContext context, GameObject actor);

    /// <summary>
    /// Called regularly after start until <see cref="IsDone"/>
    /// </summary>
    void Update(ITiaActionContext context);

    /// <summary>
    /// Called when the action is done and the action sequence is
    /// moving to the next action.
    /// </summary>
    void Finish(ITiaActionContext context);
}
