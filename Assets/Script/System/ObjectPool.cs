using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.InputSystem.Interactions;


[Serializable]
public class ObjectPool<T> : IPoolable<T> where T : MonoBehaviour, IDisposable
{
    private Queue<T> _pool;
    public GameObject _prefab;
    public Transform _poolParent;

    private bool _enableCreateNew = true;
    public int Count = 0;

    public ObjectPool()
    {
        _pool = new Queue<T>();
    }

    //자동으로 parent의 자식 내용물을 바탕으로 object 풀을 설정하는 방식
    public ObjectPool(Transform parent, GameObject prefab, bool enableCreateNew = true)
    : this(parent, prefab, enableCreateNew, parent.GetComponentsInChildren<T>(true)){}


    //values를 기준으로 object 풀을 설정하고, 그 값을 parent의 자식으로 설정하는 방식
    public ObjectPool(Transform parent, GameObject prefab, bool enableCreateNew = true, params T[] values)
    {
        _pool = new Queue<T>(values.Length);
        Add(values);
        Count = values.Length;
        SettingPool(prefab, parent, enableCreateNew);
    }

    private void SettingPool(GameObject prefab, Transform parent, bool enableCreateNew = true)
    {
        _prefab = prefab;
        _poolParent = parent;
        _enableCreateNew = enableCreateNew; 
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
        if (_pool.Count == 0 && _enableCreateNew)
        {
            Debug.LogWarning("make new object : no object in pool");
            GameObject newObject = MonoBehaviour.Instantiate(_prefab, _poolParent);
            return newObject.GetComponent<T>();
        }
        else if(_pool.Count == 0 && !_enableCreateNew){
            Debug.LogWarning("no object in pool : cant make new object");  
            return null;
        }

            T obj = _pool.Dequeue();
            Count--;
            return obj;
        
    }

    public void Return(T obj)
    {
        //obj.transform.parent = _poolParent;
        obj.Dispose();
        Add(obj);
        obj.transform.position = Vector3.zero;
        obj.transform.rotation = Quaternion.identity;
        Count++;
    }

}
