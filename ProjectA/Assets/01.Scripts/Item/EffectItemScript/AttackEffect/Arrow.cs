using System.Collections;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public Vector3 dir;

    public float speed;

    public float destroyDelay;

    private void FixedUpdate()
    {
        transform.position += dir * speed * Time.fixedDeltaTime;
    }
    private IEnumerator ReturnCoroutine()
    {
        yield return new WaitForSeconds(destroyDelay);

        PoolingManager.Instance.ReturnObject("Arrow", gameObject);
    }
    public void CallDestroy()
    {
        StartCoroutine(ReturnCoroutine());
    }
}