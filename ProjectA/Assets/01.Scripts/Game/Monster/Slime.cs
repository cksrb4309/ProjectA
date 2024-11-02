using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Slime : Monster
{
    public SlimeSearchCollider slimeSearchCollider;
    public Animator animator;
    public Transform PlayerPos {
        get { 
            if (playerPos == null)
                playerPos = GameManager.Instance.player.transform;
            return playerPos; 
        } 
    }
    private Transform playerPos = null;

    public float moveTickMin;
    public float moveTickMax;

    public override void StartMonster()
    {
        base.StartMonster();

        slimeSearchCollider.enabled = true;

        StartCoroutine(BehaviorCoroutine());
    }
    public override void EndMonster()
    {
        base.EndMonster();

        slimeSearchCollider.enabled = false;
    }
    private void Behavior()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("LeftIdle") ||
            animator.GetCurrentAnimatorStateInfo(0).IsName("RightIdle"))
        {
            if (PlayerPos.position.x < transform.position.x) animator.SetTrigger("LeftMove");
            else animator.SetTrigger("RightMove");
        }
    }

    private IEnumerator BehaviorCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Mathf.Lerp(moveTickMin, moveTickMax, Random.value));

            if (!IsAlive) yield break; // ������� ���� ��쿡 �ൿ �ڷ�ƾ ����

            Behavior();
        }
    }

    public override void Hit(float damage, bool isMainAttack)
    {
        base.Hit(damage, isMainAttack);

        if (IsAlive)
            animator.SetTrigger("Hit");
        else
            Dead();
    }

    private void Dead()
    {
        animator.SetBool("Dead", true);

        cd.enabled = false;
    }

    public void CompleteDead()
    {
        PoolingManager.Instance.ReturnObject("Slime", gameObject);
    }

    public void LeftAttack()
    {
        animator.SetTrigger("LeftAttack");
    }
    public void RightAttack()
    {
        animator.SetTrigger("RightAttack");
    }
}