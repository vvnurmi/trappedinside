using System;
using UnityEngine;

/// <summary>
/// Associated with a Unity GameObject under the root TIA game object by name.
/// GameObject may need Animator component.
/// Serialized as a name string.
/// </summary>
public class TiaActor
{
    public string gameObjectName { get; set; }

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
    /// </summary>
    public void Initialize(GameObject tiaRoot)
    {
        gameObject = tiaRoot.FindChildByName(gameObjectName);
        Debug.Assert(gameObject != null, $"{nameof(TiaActor)} couldn't find '{gameObjectName}' under {tiaRoot.GetFullName()}");
    }
}
