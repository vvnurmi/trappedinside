using UnityEngine;

/// <summary>
/// TIA script asset.
/// </summary>
public class TiaScriptAsset : ScriptableObject
{
    [TextArea(5, 25)]
    public string script = "";

    public TiaScriptAsset(string script)
    {
        this.script = script;
    }

    public override string ToString()
    {
        return script ?? "<null>";
    }
}
