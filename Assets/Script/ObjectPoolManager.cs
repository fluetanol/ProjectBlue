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
    
    public Transform Parent;

    protected virtual void Awake()
    {
        if(Instance == null) Instance = this as T2;
    }
    public P test<P>() where P : T{
        return null;
    }

    public abstract T Get(int num);
    public abstract T Get(int num, Vector3 position, Quaternion rotation);
    public abstract P Get<P>(int num) where P : T;
    public abstract P Get<P>(int num, Vector3 position, Quaternion rotation) where P : T;
    public abstract T[] Get(int num, int count);
    public abstract void Return(int num, T obj);
    public abstract void FirstCreate(PoolingInfo poolInfo);
    public abstract void DestroyPoolObject(int num, int count);
    public abstract void Add(T obj);
    public abstract void Add(T[] obj);
}