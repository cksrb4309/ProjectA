using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class BladeofShock : StackEffect
{
    public float damage;
    public float minDistance;
    public GameObject[] spark;
    public LineRenderer sparkLine;
    List<Monster> MonsterPos { get { return Battle.instance.monsterPos; } }
    public override void Play()
    {
        List<Monster> attackList = new List<Monster>();

        Monster target = GameManager.Instance.lastHitMonster;

        attackList.Add(target);

        int cnt = 2;

        while (cnt--> 0)
        {
            Monster mob = null;

            Vector2 targetPos = target.transform.position;

            float minValue = 100f;

            for (int i = 0; i < MonsterPos.Count; i++)
            {
                bool isContains = false;
                for (int j = 0; j < attackList.Count; j++)
                {
                    if (attackList[j].id == MonsterPos[i].id)
                        isContains = true;
                }

                if (!isContains)
                {
                    Vector2 vec = targetPos - (Vector2)MonsterPos[i].transform.position;

                    float distance = Mathf.Abs(vec.x) + Mathf.Abs(vec.y);

                    if (minDistance > distance)
                    {
                        if (minValue > distance)
                        {
                            minValue = distance;

                            mob = MonsterPos[i];
                        }
                    }
                }
            }
            if (mob != null)
            {
                target = mob;

                attackList.Add(mob);
            }
            else break;
        }

        for (int i = 0; i < attackList.Count; i++)
        {
            attackList[i].Hit(12, false);
        }

        sparkLine.gameObject.SetActive(true);
        sparkLine.positionCount = attackList.Count;
        for (int i = 0; i < attackList.Count; i++)
        {
            spark[i].SetActive(true);
            spark[i].transform.position = attackList[i].GetCenterPosition();
            sparkLine.SetPosition(i, attackList[i].GetCenterPosition());
        }

        StartCoroutine(DisableCoroutine());
    }
    IEnumerator DisableCoroutine()
    {
        yield return new WaitForSeconds(1f);

        for (int i = 0; i < 3; i++) spark[i].SetActive(false);
        sparkLine.gameObject.SetActive(false);
    }
}
