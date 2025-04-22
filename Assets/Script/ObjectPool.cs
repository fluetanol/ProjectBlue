using UnityEngine;
using System.Collections.Generic;
using System;


[Serializable]
public class ObjectPool<T> : IPoolable<T> where T : MonoBehaviour, IDisposable
{
    private Queue<T> _pool;
    public Transform _poolParent;
    public int Count = 0;

    public ObjectPool()
    {
        _pool = new Queue<T>();
    }

    //자동으로 parent의 자식 내용물을 바탕으로 object 풀을 설정하는 방식
    public ObjectPool(Transform parent)
    {
        T[] objects = parent.GetComponentsInChildren<T>(true);
        _pool = new Queue<T>(objects.Length);
        Add(objects);
        SetPoolParent(parent);
        Count = objects.Length;
    }

    //values를 기준으로 object 풀을 설정하고, 그 값을 parent의 자식으로 설정하는 방식
    public ObjectPool(Transform parent, params T[] values)
    {
        _pool = new Queue<T>(values.Length);
        Add(values);
        SetPoolParent(parent);
        Count = values.Length;
    }

    public void Add(T obj)
    {
        obj.gameObject.SetActive(false);
        _pool.Enqueue(obj);
    }

    public void Add(T[] objects)
    {
        foreach (T obj in objects)
        {
            Add(obj);
        }
    }

    public T Get()
    {
        if (_pool.Count == 0)
        {
            Debug.LogWarning("_pool is empty, creating new object.");
            return null;
        }
        T obj = _pool.Dequeue();
       // obj.gameObject.SetActive(true);
        Count--;
        return obj;
    }

    public void Return(T obj)
    {
        obj.transform.parent = _poolParent;
        obj.Dispose();
        Add(obj);
        obj.transform.position = Vector3.zero;
        obj.transform.rotation = Quaternion.identity;
        Count++;
    }


    public void SetPoolParent(Transform parent)
    {
        _poolParent = parent;
    }
}
