using System;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEditor.Build.Reporting;
using UnityEngine;

public interface IPoolable{ }
public interface IPoolable<T> :IPoolable where T : MonoBehaviour
{
    void Add(T obj);
    T Get();
    void Return(T obj);                                             
}

[Serializable]
public class ObjectPool<T> : IPoolable<T> where T : MonoBehaviour, IDisposable{
    private Queue<T> _pool;
    private Transform _poolParent;

    public ObjectPool(){
        _pool = new Queue<T>();
    }

    public ObjectPool(Transform parent) {
        T[] objects = parent.GetComponentsInChildren<T>(true);
        _pool = new Queue<T>(objects.Length);
        Add(objects);
        SetPoolParent(parent);
    }

    public ObjectPool(Transform parent, params T[] values) {
        _pool = new Queue<T>(values.Length);
        Add(values);
        SetPoolParent(parent);
    }

    public void Add(T obj){
        _pool.Enqueue(obj);
    }

    public void Add(T[] objects){
        foreach(T obj in objects){
            _pool.Enqueue(obj);
        }
    }

    public T Get(){
        if(_pool.Count == 0){
            Debug.LogWarning("_pool is empty, creating new object.");
            return null;
        }
        return _pool.Dequeue();
    }

    public void Return(T obj){
        obj.Dispose();
        Add(obj);
    }

    public void SetPoolParent(Transform parent){
        _poolParent = parent;
    }
}


public enum PoolType
{
    Enemy,
    Player,
    Bullet,
    Item,
    Effect,
    None
}


public class PoolFactory : MonoBehaviour
{
    public static PoolFactory Instance { get; private set; }

    private void Awake() {
        Instance = this;
    }
}



public class ObjectPoolManager : MonoBehaviour 
{
    public IPoolable<EnemyMovement> EnemyPool{
        get;
        private set;
    }

    public IPoolable<Bullet> BulletPool{
        get;
        private set;
    }

    public static ObjectPoolManager Instance { get; private set; }
    
    void Awake()
    {
        Instance = this;
        EnemyPool =  new ObjectPool<EnemyMovement>();
        BulletPool = new ObjectPool<Bullet>();
        

    }


    public void Get(PoolType type){
        switch(type){
            case PoolType.Enemy:
    
                break;
        }
    }
    
}
