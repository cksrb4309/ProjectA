using UnityEngine;

public class ParticleAutoReturn : MonoBehaviour
{
    void ReturnParticle()
    {
        PoolingManager.Instance.ReturnObject(gameObject.name, gameObject);
    }
}
