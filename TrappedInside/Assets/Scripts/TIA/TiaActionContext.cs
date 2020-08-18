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

    /// <summary>
    /// Returns a TIA script by name.
    /// </summary>
    TiaScript GetScript(string name);

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

    public TiaActor Actor => actionSequence.Actor;

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
    //!!!Func<string, GameObject> findGameObject)
    {
        actionSequence = null;
        ScriptRunner = scriptRunner;
        TiaRoot = tiaRoot;
        //!!!FindGameObject = findGameObject;
        FindGameObject = null;
    }

    public GameObject FindChild(string gameObjectName)
    {
        throw new NotImplementedException();
        /*!!!
        name =>
        {
            var obj = tiaRoot.FindChildByName(name);
            Debug.Assert(obj != null, $"{nameof(TiaActionContext)} couldn't find '{name}' under {tiaRoot.GetFullName()}");
            return obj;
        });
        */
    }

    public T FindComponentInChildren<T>(string gameObjectName) where T : MonoBehaviour
    {
        throw new NotImplementedException();
    }
}
