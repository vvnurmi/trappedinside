using UnityEngine;

public class StarController : MonoBehaviour {

    private readonly float angularVelocity = 10.0f;
    private Vector3 rotationAxis = new Vector3(0f, 0f, 1f);
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(rotationAxis, 2 * Mathf.PI * angularVelocity * Time.deltaTime);
	}
}
