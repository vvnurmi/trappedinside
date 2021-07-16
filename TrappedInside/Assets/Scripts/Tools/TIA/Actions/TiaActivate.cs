using System;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

/// <summary>
/// Activates the actor.
/// </summary>
/// <seealso cref="TiaDeactivate"/>
[Serializable]
public class TiaActivate : ITiaAction, IYamlConvertible
{
    [YamlIgnore]
    public string DebugName { get; set; }

    public bool IsDone { get; private set; }

    public void Start(ITiaActionContext context)
    {
    }

    public void Update(ITiaActionContext context)
    {
        context.Actor.SetActive(true);
        IsDone = true;
    }

    public void Finish(ITiaActionContext context)
    {
    }

    #region IYamlConvertible implementation

    public void Read(IParser parser, Type expectedType, ObjectDeserializer nestedObjectDeserializer)
    {
        parser.Expect<Scalar>();
    }

    public void Write(IEmitter emitter, ObjectSerializer nestedObjectSerializer)
    {
    }

    #endregion
}

/// <summary>
/// Deactivates the actor.
/// </summary>
/// <seealso cref="TiaActivate"/>
[Serializable]
public class TiaDeactivate : ITiaAction, IYamlConvertible
{
    [YamlIgnore]
    public string DebugName { get; set; }

    public bool IsDone { get; private set; }

    public void Start(ITiaActionContext context)
    {
    }

    public void Update(ITiaActionContext context)
    {
        context.Actor.SetActive(false);
        IsDone = true;
    }

    public void Finish(ITiaActionContext context)
    {
    }

    #region IYamlConvertible implementation

    public void Read(IParser parser, Type expectedType, ObjectDeserializer nestedObjectDeserializer)
    {
        parser.Expect<Scalar>();
    }

    public void Write(IEmitter emitter, ObjectSerializer nestedObjectSerializer)
    {
    }

    #endregion
}
