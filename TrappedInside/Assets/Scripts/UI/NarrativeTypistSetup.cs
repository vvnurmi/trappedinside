/// <summary>
/// Helper struct to pass parameters to <see cref="NarrativeTypist"/>.
/// </summary>
// Note: This is a class instead of a struct because struct members in classes are not
// displayed properly in the debugger at least with Visual Studio 2017 (15.9.8) and
// Unity 2019.2.18f1. This would complicate debugging NarrativeTypist.
public class NarrativeTypistSetup
{
    public string fullText;
    public string speaker;
    public string[] choices;
}
