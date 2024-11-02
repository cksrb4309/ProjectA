using System.Collections;
using UnityEngine;

public class RestrictedAreaWarning : MonoBehaviour
{
    public SpriteRenderer warningSr;

    Color srColor = Color.white;

    float Alpha
    {
        set
        {
            srColor.a = value;

            warningSr.color = srColor;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StopAllCoroutines();

            StartCoroutine(WarningCoroutine());
        }
    }

    IEnumerator WarningCoroutine()
    {
        double t = 0;

        while (t < 1)
        {
            t += Time.deltaTime * 0.5f;

            Alpha = (float)t;

            yield return null;
        }

        // ���� �־��� ��� ��ġ ����ٰ�~
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StopAllCoroutines();

            StartCoroutine(StopWarningCoroutine());
        }
    }
    IEnumerator StopWarningCoroutine()
    {
        double t = srColor.a;

        while (t > 0)
        {
            t -= Time.deltaTime * 1f;

            Alpha = (float)t;

            yield return null;
        }
    }
}
