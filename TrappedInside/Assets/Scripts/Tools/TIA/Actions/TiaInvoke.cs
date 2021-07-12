using System.Linq;
using System.Reflection;
using UnityEngine;
using YamlDotNet.Serialization;

/// <summary>
/// Invokes a static C# method in <see cref="TiaMethods"/>.
/// At most two string arguments can be provided. If the method expects a
/// <see cref="ITiaActionContext"/> then the current action context is provided implicitly.
/// </summary>
[System.Serializable]
public class TiaInvoke : ITiaAction
{
    [YamlMember(Alias = "Name")]
    [field: SerializeField]
    public string MethodName { get; set; }

    [YamlMember(Alias = "Arg1")]
    [field: SerializeField]
    public string MethodArgument1 { get; set; }

    [YamlMember(Alias = "Arg2")]
    [field: SerializeField]
    public string MethodArgument2 { get; set; }

    [YamlIgnore]
    public string DebugName { get; set; }

    public bool IsDone { get; private set; }

    public void Start(ITiaActionContext context)
    {
        var flags = BindingFlags.Public | BindingFlags.Static;
        var methodInfo = typeof(TiaMethods).GetMethod(MethodName, flags);
        var args = GatherArguments(context, methodInfo);
        methodInfo.Invoke(obj: null, args);
        IsDone = true;
    }

    public void Update(ITiaActionContext context)
    {
    }

    public void Finish(ITiaActionContext context)
    {
    }

    private object[] GatherArguments(ITiaActionContext context, MethodInfo methodInfo)
    {
        var parameters = methodInfo.GetParameters();
        int argIndex = 0;
        string NextArg()
        {
            var arg
                = argIndex == 0 ? MethodArgument1
                : argIndex == 1 ? MethodArgument2
                : throw new System.NotImplementedException($"{nameof(TiaInvoke)} can only call methods with max two string parameters, but {MethodName} wants more");
            argIndex++;
            return arg;
        }

        var args = new object[parameters.Length];
        for (int paramIndex = 0; paramIndex < parameters.Length; paramIndex++)
        {
            var paramType = parameters[paramIndex].ParameterType;
            args[paramIndex]
                = paramType == typeof(ITiaActionContext) ? (object)context
                : paramType == typeof(string) ? NextArg()
                : throw new System.NotImplementedException($"{nameof(TiaInvoke)} only supports passing string arguments, but {MethodName} wants {paramType}");
        }
        return args;
    }
}
