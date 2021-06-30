using System;
using UnityEngine;

/// <summary>
/// Run-time context for <see cref="ITiaAction"/> instances.
/// Provided by <see cref="TiaPlayer"/>.
/// </summary>
public interface ITiaActionContext
{
    /// <summary>
    /// The <see cref="TiaPlayer"/> component that's running the current script.
    /// </summary>
    TiaPlayer ScriptRunner { get; }

    /// <summary>
    /// The root of the <see cref="GameObject"/> hierarchy where the TIA objects live.
    /// </summary>
    GameObject TiaRoot { get; }

    TiaActor Actor { get; }

    /// <summary>
    /// Creates a new object which is identical to the context.
    /// </summary>
    ITiaActionContext Clone();

    /// <summary>
    /// Sets <paramref name="actionSequence"/> as the active one.
    /// </summary>
    void SetActionSequence(TiaActionSequence actionSequence);

    // ---  maybe not needed below here --- //

    TiaScript GetScript(string name);
    GameObject FindChild(string gameObjectName);
    T FindComponentInChildren<T>(string gameObjectName) where T : MonoBehaviour;
}

/// <summary>
/// Provides context information from <see cref="TiaPlayer"/>
/// to <see cref="ITiaAction"/>.
/// </summary>
public struct TiaActionContext : ITiaActionContext
{
    private TiaActionSequence actionSequence;

    public TiaPlayer ScriptRunner { get; private set; }

    public GameObject TiaRoot { get; private set; }

    public TiaActor Actor => actionSequence.Actor
        ?? throw new NullReferenceException($"No actor set for {actionSequence.DebugName}");

    public TiaScript GetScript(string name)
    {
        throw new NotImplementedException();
    }

    // We're a value type, so this will return a shallow copy.
    public ITiaActionContext Clone() => this;

    public void SetActionSequence(TiaActionSequence actionSequence)
    {
        this.actionSequence = actionSequence;
    }

    public Func<string, GameObject> FindGameObject { get; private set; }

    public TiaActionContext(
        TiaPlayer scriptRunner,
        GameObject tiaRoot)
    {
        actionSequence = null;
        ScriptRunner = scriptRunner;
        TiaRoot = tiaRoot;
        FindGameObject = null;
    }

    public GameObject FindChild(string gameObjectName)
    {
        throw new NotImplementedException();
    }

    public T FindComponentInChildren<T>(string gameObjectName) where T : MonoBehaviour
    {
        throw new NotImplementedException();
    }
}
