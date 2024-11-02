using System.Collections;
using UnityEngine;

public class Campfire : MonoBehaviour
{
    public PlayerController pc;
    public Transform pt;
    float t = 0;
    float maxT = 1f;

    private void FixedUpdate()
    {
        t += Time.fixedDeltaTime;
        if (Vector3.Distance(pt.position, transform.position) < 0.8f)
        {
            if (t > maxT) { Apply(); t = 0; }
        }
    }

    public void Apply()
    {
        pc.Hit(3);
    }
}