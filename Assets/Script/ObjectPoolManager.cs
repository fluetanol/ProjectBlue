using System;
using System.Collections.Generic;
using UnityEngine;

public struct PoolingInfo{
    public int size;
    public int[] PoolTypes;
    public int[] PoolCount;
}

public abstract class ObjectPoolManager<T, T2> : MonoBehaviour where T : MonoBehaviour, IDisposable where T2 : MonoBehaviour
{
    public static T2 Instance { get; private set; }

    //풀링 오브젝트 이상으로 오브젝트를 생성시키려 할 때, 새로 생성할 것인지 여부
    public bool enableCreateNew = false;
    public Transform Parent;

    protected virtual void Awake()
    {
        if(Instance == null) Instance = this as T2;
    }
    public P test<P>() where P : T{
        return null;
    }

    public abstract T Get(int num);
    public abstract T Get(int num, Vector3 position, Quaternion rotation, bool isActive = false);
    public abstract P Get<P>(int num) where P : T;
    public abstract P Get<P>(int num, Vector3 position, Quaternion rotation, bool isActive = false) where P : T;
    public abstract T[] Get(int num, int count);
    public abstract void Return(int num, T obj);
    public abstract void FirstCreate(PoolingInfo poolInfo);
    public abstract void DestroyPoolObject(int num, int count);
    public abstract void Add(T obj);
    public abstract void Add(T[] obj);
}