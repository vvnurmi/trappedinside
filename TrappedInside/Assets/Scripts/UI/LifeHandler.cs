using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class LifeHandler : MonoBehaviour
{
    private Image[] hearts;
    private Camera camera;
    public GameObject particleEffect;


    private void Awake()
    {
        hearts = GetComponentsInChildren<Image>();
        camera = FindObjectOfType<Camera>();
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
                    CreateParticleEffect(hearts[i].transform.position);

                hearts[i].enabled = false;
                value--;
            }
        }
    }

    private void CreateParticleEffect(Vector3 position)
    {
        var pos = camera.ScreenToWorldPoint(position);
        Instantiate(particleEffect, pos + new Vector3(0f, 0f, 2f), Quaternion.identity);
    }
}
