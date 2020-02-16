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
