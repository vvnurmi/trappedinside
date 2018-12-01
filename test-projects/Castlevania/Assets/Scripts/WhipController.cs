using UnityEngine;

public class WhipController : MonoBehaviour {

    public bool IsWhipCollision { get; set; }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter2D(Collider2D collision) {
        IsWhipCollision = true;
    }
}
