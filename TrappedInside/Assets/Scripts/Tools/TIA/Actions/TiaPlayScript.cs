using System;
using UnityEngine;
using YamlDotNet.Serialization;

/// <summary>
/// Starts to play another TIA script in the same root TIA game object.
/// </summary>
[Serializable]
public class TiaPlayScript : ITiaAction
{
    [YamlMember(Alias = "Name")]
    [field: SerializeField]
    public string ScriptName { get; set; }

    [YamlIgnore]
    public string DebugName { get; set; }

    public bool IsDone => throw new NotImplementedException();

    public void Start(ITiaActionContext context)
    {
        throw new NotImplementedException();
    }

    public void Update(ITiaActionContext context)
    {
        throw new NotImplementedException();
    }

    public void Finish(ITiaActionContext context)
    {
        throw new NotImplementedException();
    }
}
