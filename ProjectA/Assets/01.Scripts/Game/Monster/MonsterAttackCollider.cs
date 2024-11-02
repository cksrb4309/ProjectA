using UnityEngine;

public class MonsterAttackCollider : MonoBehaviour
{
    [SerializeField] bool isProjectile = false;
    [SerializeField] string projectileName = string.Empty;

    public float damage;
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerController>().Hit(damage * Inventory.CurrentData.monsterDamage);

            if (isProjectile)
            {
                PoolingManager.Instance.ReturnObject(projectileName, gameObject);
            }
        }
    }
}
