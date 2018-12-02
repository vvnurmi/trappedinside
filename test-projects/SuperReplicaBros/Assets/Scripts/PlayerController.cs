using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Tooltip("Maximum walking speed")]
    public float maxSpeed = 5;

    private void Start()
    {

    }

    private void Update()
    {
        var horizontalInput = Input.GetAxis("Horizontal");
        var oldPos = gameObject.transform.position;
        var deltaPos = Vector3.right * horizontalInput * maxSpeed * Time.deltaTime;
        gameObject.transform.position = oldPos + deltaPos;
    }
}
