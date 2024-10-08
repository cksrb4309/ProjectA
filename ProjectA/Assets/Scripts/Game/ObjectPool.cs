using UnityEngine;
using System.Collections.Generic;
using NUnit.Framework;
using System.Linq;


public class ObjectPool<T> where T : Component
{
    private Transform parent;
    private Queue<T> objectPool = new Queue<T>();
    private T prefab;
    
    public ObjectPool(Component prefab, Transform parent ,int count)
    {
        this.prefab = (T)prefab;
        this.parent = parent;

        for (int i = 0; i < count; i++) Add();
    }
    public void Set(T obj)
    {
        obj.gameObject.SetActive(false);
        obj.transform.parent = parent;
        objectPool.Enqueue(obj);
    }
    public T Get()
    {
        if (objectPool.Count == 0) Add();

        T obj = objectPool.Dequeue();
        obj.gameObject.SetActive(true);
        return obj;
    }
    private void Add()
    {
        T obj = GameObject.Instantiate(prefab, parent);
        obj.gameObject.SetActive(false);
        objectPool.Enqueue(obj);
    }
}
