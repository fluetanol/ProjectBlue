using System;
using System.Collections.Generic;
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
        obj.gameObject.SetActive(false);
        _pool.Enqueue(obj);
    }

    public void Add(T[] objects){
        foreach(T obj in objects){
            //Debug.Log("added to pool: " + obj.name);
            _pool.Enqueue(obj);
        }
    }

    public T Get(){
        if(_pool.Count == 0){
            Debug.LogWarning("_pool is empty, creating new object.");
            return null;
        }
        T obj = _pool.Dequeue();
        obj.gameObject.SetActive(true);
        return obj;
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

    public Transform EnemyPoolParent;
    public Transform BulletPoolParent;

    void Awake()
    {
        Instance = this;
        EnemyPool = new ObjectPool<EnemyMovement>();
        BulletPool = new ObjectPool<Bullet>();

        if (LoadSceneTest.Instance == null){
            print("load scne test instance is null in Awake");
        }
        else{
            for(int i=0; i< 50; i++){
                GameObject g = Instantiate(LoadSceneTest.Instance.prefab, EnemyPoolParent);
                int x = UnityEngine.Random.Range(-50,50);
                int z = UnityEngine.Random.Range(-50,50);
                g.transform.position = new Vector3(x, 4.5f, z);
                EnemyPool.Add(g.GetComponent<EnemyMovement>());
            }
        }

    }

    void Start()
    {
    }


    public T Get<T>(PoolType type) where T : MonoBehaviour, IDisposable{
        T obj = null;
        switch(type){
            case PoolType.Enemy:
                obj = EnemyPool.Get() as T;
                break;
            case PoolType.Bullet:
                obj = BulletPool.Get() as T;
                break;
        }
        if(obj!=null) print("get enemy success");

        return obj;
    }
    
    public void Return<T>(PoolType type, T obj) where T : MonoBehaviour, IDisposable{
        try{
            switch(type){
                case PoolType.Enemy:
                    EnemyPool.Return(obj as EnemyMovement);
                    break;
                case PoolType.Bullet:
                    BulletPool.Return(obj as Bullet);
                    break;
            }
            print("return enemy success");
        }
        catch(Exception e){
            Debug.LogError("Error while returning object to pool: " + e.Message);
        }
    }
}
