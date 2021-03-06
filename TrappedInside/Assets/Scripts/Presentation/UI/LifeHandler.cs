﻿using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class LifeHandler : MonoBehaviour
{
    private Image[] hearts;
    public GameObject particleEffect;


    private void Awake()
    {
        hearts = GetComponentsInChildren<Image>();
    }

    public int NumberOfHearts => hearts.Count(heart => heart.enabled);

    public void SetNumberOfHearts(int value, bool createParticleEffect)
    {
        Debug.Assert(value <= hearts.Length);
        var diff = NumberOfHearts - value;

        if (diff > 0)
            RemoveHearts(diff, createParticleEffect);
        else if (diff < 0)
            AddHearts(-diff);
    }

    private void AddHearts(int value)
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (value > 0 && !hearts[i].enabled)
            {
                hearts[i].enabled = true;
                value--;
            }
        }
    }

    private void RemoveHearts(int value, bool createParticleEffect)
    {
        for (int i = hearts.Length - 1; i >= 0; i--)
        {
            if (value > 0 && hearts[i].enabled)
            {
                if (createParticleEffect)
                    CreateParticleEffect(hearts[i].transform);

                hearts[i].enabled = false;
                value--;
            }
        }
    }

    private void CreateParticleEffect(Transform parent)
    {
        var obj = Instantiate(particleEffect, Vector3.zero, Quaternion.identity);
        obj.transform.SetParent(parent, false);
    }
}
