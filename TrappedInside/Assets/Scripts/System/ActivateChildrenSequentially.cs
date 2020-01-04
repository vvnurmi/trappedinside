using System.Linq;
using UnityEngine;

public interface ISequentialChild
{
    bool IsDone();
}

/// <summary>
/// Activates each child object in turn after the previous is "done."
/// A child is considered "done" if it has deactivated itself,
/// or if it implements <see cref="ISequentialChild"/> and returns
/// true from <see cref="ISequentialChild.IsDone"/>.
/// </summary>
public class ActivateChildrenSequentially : MonoBehaviour
{
    private GameObject[] children;
    private int currentChild;

    #region MonoBehaviour overrides

    private void Start()
    {
        children = transform
            .Cast<Transform>()
            .Select(transform => transform.gameObject)
            .ToArray();

        foreach (var child in children)
            child.SetActive(false);

        ActivateCurrentChild();
    }

    private void Update()
    {
        if (currentChild == children.Length) return;
        if (!IsCurrentChildDone()) return;

        currentChild++;
        ActivateCurrentChild();
    }

    #endregion

    private bool IsCurrentChildDone()
    {
        var child = children[currentChild];

        if (!child.activeSelf) return true;

        var sequentialChild = child.GetComponent<ISequentialChild>();
        if (sequentialChild == null) return false;

        return sequentialChild.IsDone();
    }

    private void ActivateCurrentChild()
    {
        if (currentChild < children.Length)
            children[currentChild].SetActive(true);
    }
}
