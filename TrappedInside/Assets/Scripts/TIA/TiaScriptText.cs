using UnityEngine;

[CreateAssetMenu(fileName = "TiaScriptText", menuName = "TIA/ScriptText")]
public class TiaScriptText : ScriptableObject
{
    [Tooltip("TIA script in YAML format.")]
    [Multiline(10)]
    public string text;
}
