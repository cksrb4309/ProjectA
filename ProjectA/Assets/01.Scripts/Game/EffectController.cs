using UnityEngine;

public class EffectController : MonoBehaviour
{
    private static EffectController instance;
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }
    public void AttackEffect(string effectName, Vector3 pos, bool isLeft)
    {
        GameObject particle = PoolingManager.Instance.GetObject(effectName);

        particle.transform.position = pos;
        particle.transform.rotation = Quaternion.identity;

        if (!isLeft)
            particle.transform.localScale = new Vector3(-1, 1, 1);
    }


    public static void CallAttackEffect(string effectName, Vector3 pos, bool isLeft) // �÷��̾ ������ �Ĵٺ� �� isLeft
    {
        instance.AttackEffect(effectName, pos, isLeft);
    }
}
