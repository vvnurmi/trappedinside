using System;
using System.Collections;
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
    /// Returns the context object for <paramref name="owner"/>. Context objects are the only way
    /// that TIA script objects are allowed to persist their state.
    /// </summary>
    /// <typeparam name="TContext">The type of the context object.</typeparam>
    /// <returns>False if no context object was set.</returns>
    (bool found, TContext contextObject) TryGet<TContext>(object owner);

    /// <summary>
    /// Stores the context object for <paramref name="owner"/>. Context objects are the only way
    /// that TIA script objects are allowed to persist their state.
    /// </summary>
    /// <typeparam name="TContext">The type of the context object.</typeparam>
    void Set<TContext>(object owner, TContext context);

    /// <summary>
    /// Creates a new context which has the same properties except no context objects.
    /// </summary>
    ITiaActionContext CloneEmpty();
}

/// <summary>
/// Provides context information from <see cref="TiaPlayer"/>
/// to <see cref="ITiaAction"/>.
/// </summary>
public struct TiaActionContext : ITiaActionContext
{
    private Hashtable contexts;

    public TiaPlayer ScriptRunner { get; private set; }

    public GameObject TiaRoot { get; private set; }

    public (bool found, TContext contextObject) TryGet<TContext>(object owner)
    {
        if (!contexts.ContainsKey(owner))
            return (false, default);
        return (true, (TContext)contexts[owner]);
    }

    public void Set<TContext>(object owner, TContext context)
    {
        contexts[owner] = context;
    }

    public ITiaActionContext CloneEmpty()
    {
        // We're a value type, so this will do a shallow copy.
        var clone = this;
        // Clear the context objects.
        clone.contexts = new Hashtable();
        return clone;
    }

    public Func<string, GameObject> FindGameObject { get; private set; }

    public TiaActionContext(
        TiaPlayer scriptRunner,
        GameObject tiaRoot)
    {
        contexts = new Hashtable();
        ScriptRunner = scriptRunner;
        TiaRoot = tiaRoot;
        FindGameObject = null;
    }

    public TiaActionContext(TiaActionContext context)
        : this(context.ScriptRunner, context.TiaRoot)
    {
    }
}
