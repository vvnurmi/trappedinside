using System;
using UnityEngine;

/// <summary>
/// Starts to play another TIA script in the same root TIA game object.
/// </summary>
public class TiaPlayScript : ITiaAction, ITiaActionNew
{
    [YamlDotNet.Serialization.YamlMember(Alias = "Name")]
    public string ScriptName { get; set; }

    public bool IsDone => throw new NotImplementedException();

    public void Start(GameObject tiaRoot)
    {
        throw new NotImplementedException();
    }

    public void Start(ITiaActionContext context)
    {
        throw new NotImplementedException();
    }

    public void Update(TiaActor actor)
    {
        throw new NotImplementedException();
    }

    public void Update(ITiaActionContext context)
    {
        throw new NotImplementedException();
    }
}
