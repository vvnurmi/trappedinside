using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObjectAfterTimeLimit : MonoBehaviour
{
    public float timeLimit = 5.0f;

    void Start()
    {
        Destroy(gameObject, timeLimit);
    }
}
