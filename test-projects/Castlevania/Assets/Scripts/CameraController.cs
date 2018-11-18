using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public GameObject player;
    public float maxDistanceFromCenter = 1.5f;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {

        var currentDistance = transform.position.x - player.transform.position.x;

        if(currentDistance > maxDistanceFromCenter) {
            MoveCamera(player.transform.position.x + maxDistanceFromCenter);
        } else if (currentDistance < -maxDistanceFromCenter) {
            MoveCamera(player.transform.position.x - maxDistanceFromCenter);
        }


	}

    private void MoveCamera(float newXCoordinate) {
        transform.position = new Vector3(newXCoordinate, transform.position.y, transform.position.z);
    }

}
