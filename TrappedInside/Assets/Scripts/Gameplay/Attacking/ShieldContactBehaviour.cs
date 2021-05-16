using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldContactBehaviour : MonoBehaviour
{
    public enum Behaviour {
        TakesDamage,
        TurnsAround
    };

    public bool hasMomentum = false;
    public float momentum = 1.0f;
    public Behaviour behaviour = Behaviour.TakesDamage;
}
