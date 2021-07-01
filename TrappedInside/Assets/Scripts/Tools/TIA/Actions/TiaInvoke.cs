using System.Reflection;
using YamlDotNet.Serialization;

/// <summary>
/// Invokes a C# method.
/// </summary>
public class TiaInvoke : ITiaAction
{
    [YamlMember(Alias = "Name")]
    public string MethodName { get; set; }

    [YamlMember(Alias = "Arg1")]
    public string MethodArgument1 { get; set; }

    [YamlMember(Alias = "Arg2")]
    public string MethodArgument2 { get; set; }

    [YamlIgnore]
    public string DebugName { get; set; }

    public bool IsDone { get; private set; }

    public void Start(ITiaActionContext context)
    {
        BindingFlags flags = BindingFlags.Public | BindingFlags.Static;
        var methodInfo = typeof(TiaMethods).GetMethod(MethodName, flags);
        // Note: We only support static methods and at most two string parameters.
        var paramCount = methodInfo.GetParameters().Length;
        var args
            = paramCount == 0 ? new string[0]
            : paramCount == 1 ? new[] { MethodArgument1 }
            : paramCount == 2 ? new[] { MethodArgument1, MethodArgument2 }
            : throw new System.NotImplementedException($"{nameof(TiaInvoke)} can only call methods with max two string parameters, but {MethodName} has {paramCount} parameters");
        methodInfo.Invoke(obj: null, args);
        IsDone = true;
    }

    public void Update(ITiaActionContext context)
    {
    }

    public void Finish(ITiaActionContext context)
    {
    }
}
