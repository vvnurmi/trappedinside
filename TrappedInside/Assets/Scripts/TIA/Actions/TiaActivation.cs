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
