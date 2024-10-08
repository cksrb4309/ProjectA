using UnityEngine;

public class CameraController : MonoBehaviour
{
    [HideInInspector] public float Vx = 0;
    [SerializeField] private Transform PlayerTransform;

    private Vector3 pos;

    private void Start()
    {
        pos = transform.position;
    }

    private void FixedUpdate()
    {
        pos.x = transform.position.x;

        if (Mathf.Abs(pos.x - PlayerTransform.position.x) > 0.1f)
        {
            float before = pos.x;

            pos.x = Mathf.Lerp(pos.x, PlayerTransform.position.x, 0.1f);

            Vx = pos.x - before;

            transform.position = pos;
        }
    }
}
