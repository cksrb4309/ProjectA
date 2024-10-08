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
                // 불러오는 오브젝트가 아직 꺼낸 적 없는 오브젝트 일 경우
                if (!objectPoolList.ContainsKey(name)) // ObjectPool 할당
                    objectPoolList[name] = new ObjectPool<T>(poolingObjectList[i].Prefab, transform, 5);

                // 오브젝트 풀 가져옴
                ObjectPool<T> pool = (ObjectPool<T>)objectPoolList[name];

                // 반환
                return pool.Get();
            }
        }
        Debug.LogError(name + " 프리팹 생성 오류!");
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