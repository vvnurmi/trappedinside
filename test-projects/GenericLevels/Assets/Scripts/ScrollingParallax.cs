using UnityEngine;

[RequireComponent(typeof(FreeParallax))]
public class ScrollingParallax : MonoBehaviour
{
    public Transform follow;
    public float SpeedMultiplier = 0.5f;

    private Vector3 previousPos;
    private FreeParallax parallax;

    private void Start()
    {
        parallax = GetComponent<FreeParallax>();
    }

    private void Update()
    {
        var movement = follow.position - previousPos;
        previousPos = follow.position;
        parallax.Speed = SpeedMultiplier * movement.x / Time.deltaTime;
    }
}
