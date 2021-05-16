using UnityEngine;

public class WebDrawer : MonoBehaviour
{
    // Start is called before the first frame update
    Texture2D texture;

    public int imgWidth = 30;
    public int imgHeight = 150;
    public float pixelsPerUnit = 100f;
    private Vector2 startingPosition;
    private SpriteRenderer spriteRenderer;
    private int xOffset;

    private Color[] transparentPixels;

    void Start()
    {
        texture = new Texture2D(imgWidth, imgHeight)
        {
            filterMode = FilterMode.Point
        };
        spriteRenderer = GetComponent<SpriteRenderer>();
        startingPosition = transform.position;
        xOffset = imgWidth / 2;
        transparentPixels = new Color[imgWidth * imgHeight];
        for (int i = 0; i < imgWidth * imgHeight; i++)
            transparentPixels[i] = Color.clear;
    }

    // Update is called once per frame
    void Update()
    {
        texture.SetPixels(transparentPixels);

        var xMovementInPixels = pixelsPerUnit * (transform.position.x - startingPosition.x);
        var yMovementInPixels = pixelsPerUnit * (transform.position.y - startingPosition.y);

        DrawLine(xOffset, 0, (int)xMovementInPixels + xOffset, (int)yMovementInPixels);

        texture.Apply();

        float xMovementInRatio = xMovementInPixels / imgWidth;
        float yMovementInRatio = yMovementInPixels / imgHeight;

        spriteRenderer.sprite = Sprite.Create(texture, new Rect(0, 0, imgWidth, imgHeight), new Vector2(0.5f + xMovementInRatio, 1.0f + yMovementInRatio));
    }

    private void DrawLine(int x0, int y0, int x1, int y1)
    {
        if (Mathf.Abs(y1 - y0) < Mathf.Abs(x1 - x0))
        {
            if (x0 > x1)
            {
                DrawLineLow(x1, y1, x0, y0);
            }
            else
            {
                DrawLineLow(x0, y0, x1, y1);
            }
        }
        else
        {
            if (y0 > y1)
            {
                DrawLineHigh(x1, y1, x0, y0);
            }
            else
            {
                DrawLineHigh(x0, y0, x1, y1);
            }
        }
    }

    private void DrawLineLow(int x0, int y0, int x1, int y1)
    {
        var dx = x1 - x0;
        var dy = y1 - y0;
        int yi = 1;

        if (dy < 0)
        {
            yi = -1;
            dy = -dy;
        }

        var delta = 2 * dy - dx;

        int x = x0;
        int y = y0;

        while (x < x1)
        {
            DrawPixel(x, y);

            if (delta < 0)
            {
                y += yi;
                delta -= 2 * dx;
            }
            delta += 2 * dy;
            x += 1;
        }
    }


    private void DrawLineHigh(int x0, int y0, int x1, int y1)
    {
        var dx = x1 - x0;
        var dy = y1 - y0;

        var xi = 1;

        if (dx < 0)
        {
            xi = -1;
            dx = -dx;
        }

        var delta = 2 * dx - dy;
        int x = x0;
        int y = y0;

        while (y < y1)
        {
            DrawPixel(x, y);
            if (delta > 0)
            {
                x += xi;
                delta -= 2 * dy;
            }
            delta += 2 * dx;
            y += 1;
        }
    }

    void DrawPixel(int x, int y)
    {
        texture.SetPixel(x, y, Color.gray);
    }

}
