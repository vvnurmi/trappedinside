using UnityEngine;

public class ButterFlyScript : MonoBehaviour
{
    private readonly float butterflySpeed = 0.5f;
    private readonly float maxYDistance = 0.2f;
    private readonly float butterflyFlyingTime = 3.0f;

    private float originalY;
    private float ySpeed;
    private float previousYSpeedCalculationTime;
    private float butterflyDisturbedTime;
    private int butterflyDirection;

    void Start()
    {
        originalY = transform.position.y;
        butterflyDirection = -1;
        ySpeed = 0f;
        previousYSpeedCalculationTime = Time.time;
        butterflyDisturbedTime = -10.0f;
    }

    // Update is called once per frame
    void Update()
    {
        // Fly only limited time after disturbed
        if (Time.time > butterflyDisturbedTime + butterflyFlyingTime)
            return;

        if (Time.time > previousYSpeedCalculationTime + 0.2f)
        {
            ySpeed = CalculateYSpeed();
            previousYSpeedCalculationTime = Time.time;
        }

        transform.Translate(new Vector3(butterflyDirection * butterflySpeed, ySpeed) * Time.deltaTime);
    }

    private float CalculateYSpeed()
    {
        var ySpeed = ((float)RandomNumber.NextDouble() - 0.5f) * 0.5f;
        if (transform.position.y > originalY + maxYDistance)
            return -Mathf.Abs(ySpeed);
        else if (transform.position.y < originalY - maxYDistance)
            return Mathf.Abs(ySpeed);
        else
            return ySpeed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            butterflyDisturbedTime = Time.time;
            butterflyDirection = RandomNumber.Next(-1, 1) < 0 ? -1 : 1;

            if (butterflyDirection > 0)
                GetComponent<SpriteRenderer>().flipX = true;
            else
                GetComponent<SpriteRenderer>().flipX = false;
        }
    }
}
