using System;
using UnityEngine;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

/// <summary>
/// Activates the actor.
/// </summary>
/// <seealso cref="TiaDeactivate"/>
public class TiaActivate : ITiaAction, IYamlConvertible
{
    public bool IsDone { get; private set; }

    public void Start(GameObject tiaRoot)
    {
    }

    public void Update(TiaActor actor)
    {
        actor.GameObject.SetActive(true);
        IsDone = true;
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
public class TiaDeactivate : ITiaAction, IYamlConvertible
{
    public bool IsDone { get; private set; }

    public void Start(GameObject tiaRoot)
    {
    }

    public void Update(TiaActor actor)
    {
        actor.GameObject.SetActive(false);
        IsDone = true;
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
