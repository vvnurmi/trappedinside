using UnityEngine;

public class ShootingStarMovement : MonoBehaviour
{
    [Tooltip("Prefab of the shooting star.")]
    public GameObject shootingStarPrefab;

    [Tooltip("Variable containing the X pixel coordinate of the title slide.")]
    public FloatVariable inTitleSlideX;

    private GameObject shootingStar;

    // Calculate speed so that we can continue shooting star movement
    // even after the title slide stops. Remember two previous speeds because
    // the last title slide translation is probably incomplete and would give
    // too slow a speed to continue with.
    private float previousShootingStarX;
    private float[] previousSpeeds = new float[2];
    private int previousSpeedsIndex;

    private void OnEnable()
    {
        shootingStar = Instantiate(
            shootingStarPrefab,
            parent: null);

        UpdateShootingStarPosition();
    }

    private void OnDisable()
    {
        Destroy(shootingStar);
        shootingStar = null;
    }

    private void Update()
    {
        UpdateShootingStarPosition();
    }

    private void UpdateShootingStarPosition()
    {
        var camera = FindObjectOfType<Camera>();
        Debug.Assert(camera != null);
        var cameraExtents = new Vector2(
            camera.orthographicSize * camera.aspect,
            camera.orthographicSize);
        var cameraPixelsToWorldUnits = 2 * cameraExtents.x / camera.scaledPixelWidth;

        var shootingStarX = cameraExtents.x + inTitleSlideX.value * cameraPixelsToWorldUnits;
        var speed = (shootingStarX - previousShootingStarX) / Time.deltaTime;

        // Is the title still sliding?
        if (Mathf.Abs(speed) > 1e-4)
        {
            // Move along the title slide.
            previousShootingStarX = shootingStarX;
            previousSpeeds[previousSpeedsIndex] = speed;
            previousSpeedsIndex = (previousSpeedsIndex + 1) % 2;

            shootingStar.transform.SetPositionAndRotation(
                new Vector3(shootingStarX, 0, 0),
                Quaternion.identity);
        }
        else
        {
            // Continue movement based on previous speed.
            shootingStar.transform.Translate(previousSpeeds[previousSpeedsIndex] * Time.deltaTime, 0, 0);
        }

    }
}
