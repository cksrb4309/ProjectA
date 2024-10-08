using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolingManager : MonoBehaviour
{
    [SerializeField] private List<PoolSet> poolingObjectList;

    private Dictionary<string, object> objectPoolList = new Dictionary<string, object>();

    public T Get<T>(string name) where T : Component
    {
        for (int i = 0; i < poolingObjectList.Count; i++)
        {
            if (name == poolingObjectList[i].Name)
            {
                // �ҷ����� ������Ʈ�� ���� ���� �� ���� ������Ʈ �� ���
                if (!objectPoolList.ContainsKey(name)) // ObjectPool �Ҵ�
                    objectPoolList[name] = new ObjectPool<T>(poolingObjectList[i].Prefab, transform, 5);

                // ������Ʈ Ǯ ������
                ObjectPool<T> pool = (ObjectPool<T>)objectPoolList[name];

                // ��ȯ
                return pool.Get();
            }
        }
        Debug.LogError(name + " ������ ���� ����!");
        return null;
    }
    public void Set<T>(string name, T obj) where T : Component
    {
        ((ObjectPool<T>)objectPoolList[name]).Set(obj);
    }
}

[System.Serializable]
public struct PoolSet
{
    public string Name;
    public Component Prefab;
}