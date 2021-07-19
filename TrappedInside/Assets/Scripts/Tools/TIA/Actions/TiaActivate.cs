using System;
using UnityEngine;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

/// <summary>
/// Activates the actor.
/// </summary>
/// <seealso cref="TiaDeactivate"/>
[Serializable]
public class TiaActivate : SimpleTiaActionBase, ITiaAction, IYamlConvertible
{
    /// <summary>
    /// Returns true if the action is done.
    /// </summary>
    public override bool Update(ITiaActionContext context, GameObject actor)
    {
        actor?.SetActive(true);
        return true;
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
public class TiaDeactivate : SimpleTiaActionBase, ITiaAction, IYamlConvertible
{
    /// <summary>
    /// Returns true if the action is done.
    /// </summary>
    public override bool Update(ITiaActionContext context, GameObject actor)
    {
        actor?.SetActive(false);
        return true;
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
