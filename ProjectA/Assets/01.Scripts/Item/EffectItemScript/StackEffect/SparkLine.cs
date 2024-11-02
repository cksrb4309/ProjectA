using UnityEngine;

public class SparkLine : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public float duration = 2.0f; // ���� ���� �����ϴ� �� �ɸ��� �ð�

    private float t = 1f;
    private Gradient originalGradient;

    void OnEnable()
    {
        t = 1f;

        // LineRenderer ���� ��������
        lineRenderer = GetComponent<LineRenderer>();

        // ������ ������ ������ �����ϱ� ���� �ʱ� ���� ���� ����
        originalGradient = lineRenderer.colorGradient;
    }

    void FixedUpdate()
    {
        t -= Time.fixedDeltaTime;
        float alpha = Mathf.Lerp(0, 1f, t);

        Gradient newGradient = new Gradient();

        GradientColorKey[] colorKeys = originalGradient.colorKeys;
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[colorKeys.Length];

        // ���� ������ ���� ���� ���ҽ�Ŵ
        for (int i = 0; i < colorKeys.Length; i++)
        {
            alphaKeys[i] = new GradientAlphaKey(alpha, colorKeys[i].time);
        }

        newGradient.SetKeys(colorKeys, alphaKeys);
        lineRenderer.colorGradient = newGradient;
    }
}
