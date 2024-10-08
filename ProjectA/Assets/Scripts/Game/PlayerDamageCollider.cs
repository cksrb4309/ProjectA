using UnityEngine;

public class PlayerDamageCollider : MonoBehaviour
{
    [SerializeField] private float damage;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Monster"))
        {
            //Vector2 hitPoint = other.ClosestPoint(transform.position);

            //hitPointObj.position = hitPoint;

            other.GetComponent<Monster>().Hit(damage);
        }
    }
}
