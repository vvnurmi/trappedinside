using System;
using UnityEngine;

public class HitController : MonoBehaviour {

    public float immobilityTime = 0.9f;
    public float totalHitEffectTime = 1.5f;
    public float currentHitTimer = 0f;
    public float reboundForce = 2f;

    public bool HitEffectOn {
        get {
            return currentHitTimer > 0;
        }
    }

    public bool PlayerImmobile {
        get {
            return currentHitTimer > immobilityTime;
        }
    }

    public void HandleCollision(Rigidbody2D rb2d) {
        SetHitEffectOn();
        rb2d.AddForce(GetHitForce(rb2d.velocity.x), ForceMode2D.Impulse);
    }

    public void SetHitEffectOn() {
        currentHitTimer = totalHitEffectTime;
    }

    public Vector2 GetHitForce(double playerXVelocity) {
        var xVelocitySign = Math.Sign(playerXVelocity);
        xVelocitySign = xVelocitySign == 0 ? 1 : xVelocitySign;
        return new Vector2(-reboundForce * xVelocitySign, reboundForce);
    }


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (currentHitTimer > 0f) {
            currentHitTimer -= Time.deltaTime;
        }

    }
}
