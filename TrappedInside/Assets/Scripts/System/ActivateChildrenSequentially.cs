using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Activates each child object in turn after the previous one has deactivated itself.
/// </summary>
public class ActivateChildrenSequentially : MonoBehaviour
{
    private GameObject[] children;
    private int currentChild;

    void Start()
    {
        children = transform
            .Cast<Transform>()
            .Select(transform => transform.gameObject)
            .ToArray();

        foreach (var child in children)
            child.SetActive(false);

        ActivateCurrentChild();
    }

    void Update()
    {
        if (currentChild == children.Length) return;
        if (children[currentChild].activeSelf) return;

        currentChild++;
        ActivateCurrentChild();
    }

    private void ActivateCurrentChild()
    {
        if (currentChild < children.Length)
            children[currentChild].SetActive(true);
    }
}
