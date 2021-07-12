using System;
using UnityEngine;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

/// <summary>
/// Associated with a Unity GameObject under the root TIA game object by name.
/// GameObject may need Animator component.
/// Serialized as a name string.
/// </summary>
[Serializable]
public class TiaActor : IYamlConvertible
{
    [field: SerializeField]
    public string GameObjectName { get; set; }

    public GameObject GameObject
    {
        get
        {
            if (gameObject == null)
                throw new InvalidOperationException($"{nameof(TiaActor)} has no game object, maybe {nameof(Initialize)} wasn't called?");
            return gameObject;
        }
    }

    private GameObject gameObject;

    /// <summary>
    /// To be called before commencing <see cref="TiaActionSequence"/>.
    /// Returns true on success, false on failure.
    /// </summary>
    public bool Initialize(GameObject tiaRoot)
    {
        gameObject = tiaRoot.FindChildByName(GameObjectName);
        Debug.Assert(gameObject != null, $"{nameof(TiaActor)} couldn't find '{GameObjectName}' under {tiaRoot.GetFullName()}");
        return gameObject != null;
    }

    public void Read(IParser parser, Type expectedType, ObjectDeserializer nestedObjectDeserializer)
    {
        GameObjectName = parser.Expect<Scalar>().Value;
    }

    public void Write(IEmitter emitter, ObjectSerializer nestedObjectSerializer)
    {
        emitter.Emit(new Scalar(GameObjectName));
    }
}
