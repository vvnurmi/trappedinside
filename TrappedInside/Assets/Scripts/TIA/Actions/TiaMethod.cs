using System.Reflection;
using YamlDotNet.Serialization;

/// <summary>
/// Calls a C# method.
/// </summary>
public class TiaMethod : ITiaAction
{
    [YamlMember(Alias = "Name")]
    public string MethodName { get; set; }

    [YamlIgnore]
    public string DebugName { get; set; }

    public bool IsDone { get; private set; }

    public void Start(ITiaActionContext context)
    {
        BindingFlags flags = BindingFlags.Public | BindingFlags.Static;
        var methodInfo = typeof(TiaMethods).GetMethod(MethodName, flags);
        methodInfo.Invoke(null, null);
        IsDone = true;
    }

    public void Update(ITiaActionContext context)
    {
    }

    public void Finish(ITiaActionContext context)
    {
    }
}
