using System;

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
