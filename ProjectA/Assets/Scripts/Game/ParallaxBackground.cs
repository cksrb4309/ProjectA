using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [SerializeField] private CameraController cam;

    [SerializeField] private float BaseSpeed;

    [SerializeField] private bool IsRight;

    private RectTransform rect;
    
    private Vector2 pos;

    private float speed;
    private float width;

    private void Start()
    {
        rect = GetComponent<RectTransform>();

        speed = Screen.width / 10f * BaseSpeed;

        width = Screen.width;

        pos.Set(0, 0);

        if (IsRight)
        {
            pos.x = width;

            rect.anchoredPosition = pos;
        }
    }

    private void FixedUpdate()
    {
        if (cam.Vx != 0)
        {
            pos.x += cam.Vx * speed * Time.fixedDeltaTime;

            rect.anchoredPosition = pos;

            if (rect.anchoredPosition.x > width)
            {
                pos.x -= (width * 2);

                rect.anchoredPosition = pos;
            }
            else if (rect.anchoredPosition.x < -width)
            {
                pos.x += (width * 2);

                rect.anchoredPosition = pos;
            }
        }
    }
}
