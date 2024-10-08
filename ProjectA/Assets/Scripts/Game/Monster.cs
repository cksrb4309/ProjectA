using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider2D))]
public class Monster : MonoBehaviour
{
    [SerializeField] protected Image hpBar;
    [SerializeField] protected float maxHp;
    [SerializeField] protected float hp;

    protected bool IsAlive
    {
        get
        {
            return hp > 0;
        }
    }

    public virtual void Hit(float damage)
    {
        if (IsAlive)
        {
            hp -= damage;
            if (hp < 0) hp = 0;

            if (hp != 0) hpBar.fillAmount = hp / maxHp;
            else hpBar.fillAmount = 0;
        }
    }
}