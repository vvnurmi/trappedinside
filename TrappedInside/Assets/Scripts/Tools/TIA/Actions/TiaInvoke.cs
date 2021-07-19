using System.Reflection;
using UnityEngine;
using YamlDotNet.Serialization;

/// <summary>
/// Invokes a static C# method in <see cref="TiaMethods"/>.
/// At most two string arguments can be provided. If the method expects a
/// <see cref="ITiaActionContext"/> then the current action context is provided implicitly.
/// </summary>
[System.Serializable]
public class TiaInvoke : SimpleTiaActionBase, ITiaAction
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

    /// <summary>
    /// Returns true if the action is done.
    /// </summary>
    public override bool Update(ITiaActionContext context, GameObject actor)
    {
        var flags = BindingFlags.Public | BindingFlags.Static;
        var methodInfo = typeof(TiaMethods).GetMethod(MethodName, flags);
        var args = GatherArguments(context, methodInfo);
        methodInfo.Invoke(obj: null, args);
        return true;
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
