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

    //기본 최대 풀 사이즈
    public int MaxAvailableSize = 512;

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
        if(_pool.Count > MaxAvailableSize)
        {
            Debug.LogWarning("Object pool size exceeded the maximum limit. Returning null.");
            return null;
        }
        else if (_pool.Count == 0 && _enableCreateNew)
        {
            Debug.LogWarning("make new object : no object in pool");
            GameObject newObject = MonoBehaviour.Instantiate(_prefab, _poolParent);
            return newObject.GetComponent<T>();
        }
        else if (_pool.Count == 0 && !_enableCreateNew)
        {
            Debug.LogWarning("no object in pool : cant make new object");
            return null;
        }

            T obj = _pool.Dequeue();
            Count--;
            return obj;
    }

    public bool TryGet(out T obj)
    {
        if (_pool.Count > MaxAvailableSize)
        {
            Debug.LogWarning("Object pool size exceeded the maximum limit. Returning null.");
            obj = null;
            return false;
        }
        //새로 만든 적에 대해서도 false가 반환되므로, obj가 null인지 확인하는 절차가 부가적으로 필요함
        else if (_pool.Count == 0 && _enableCreateNew)
        {
            Debug.LogWarning("make new object : no object in pool");
            GameObject newObject = MonoBehaviour.Instantiate(_prefab, _poolParent);
            obj = newObject.GetComponent<T>();
            return false;
        }
        else if (_pool.Count == 0 && !_enableCreateNew)
        {
            Debug.LogWarning("no object in pool : cant make new object");
            obj = null;
            return false;
        }


        //큐에 있는 거 가져오면 정상 반환 처리 함
        obj = _pool.Dequeue();
        Count--;
        return true;
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
