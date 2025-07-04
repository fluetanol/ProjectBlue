using System.Collections.Generic;
using UnityEngine;

public class BulletPoolManager : ObjectPoolManager<Bullet, BulletPoolManager>
{
    public WeaponStats weaponStats;
    public List<ObjectPool<Bullet>> bulletPool;

    //bullet 오브젝트에 전부 공통 주입시켜야 해서 필요로 합니다.
    private IMoveData moveData;

    protected override void Awake()
    {
        base.Awake();
        bulletPool = new List<ObjectPool<Bullet>>(weaponStats.Count);
        for (int i = 0; i < weaponStats.Count; i++)
        {
            bulletPool.Add(null);
        }

        moveData = FindAnyObjectByType<PlayerMovement>();
        if (moveData == null)
        {
            Debug.LogError("PlayerMovement not found in the scene.");
        }
    }


    public override void FirstCreate(PoolingInfo poolInfo)
    {
        //풀링할 적 오브젝트 코드
        foreach (var code in poolInfo.PoolTypes)
        {
            GameObject prefab = weaponStats[code].BulletPrefab;

            int count = poolInfo.PoolCount[code];

            print("create count : " + count);
            Bullet[] objs = new Bullet[count];
            for (int i = 0; i < count; i++)
            {
                objs[i] = Instantiate(prefab, Parent).GetComponent<Bullet>();
                objs[i].SetMoveData(moveData);
            }

            print("create code : " + code);
            bulletPool[code] = new ObjectPool<Bullet>(Parent, prefab, enableCreateNew, objs);
        }
    }


    public override void DestroyPoolObject(int num, int count)
    {
        throw new System.NotImplementedException();
    }


    public override Bullet[] Get(int num, int count)
    {
        Bullet[] bullets = new Bullet[count];
        for(int i=0; i<count; i++){
            bullets[i] = bulletPool[num].Get();
        }
        return bullets;
    }

    public override P Get<P>(int num)
    {
        return bulletPool[num].Get() as P;
    }

    public override P Get<P>(int num, Vector3 position, Quaternion rotation, bool isActive = false)
    {
        P bullet = Get<P>(num);
        if (bullet != null)
        {
            bullet.transform.position = position;
            bullet.transform.rotation = rotation;
        }

        bullet.SetMoveData(moveData);

        return bullet;
    }

    public override Bullet Get(int num)
    {
        if (bulletPool[num].TryGet(out Bullet bullet))
        {
            return bullet;
        }
        else
        {
            if (bullet != null)
            {
                bullet.SetMoveData(moveData);
            }
            return null; 
        }
    }

    public override Bullet Get(int num, Vector3 position, Quaternion rotation, bool isActive = false)
    {
        Bullet bullet = Get(num);
        if (bullet != null)
        {
            bullet.transform.position = position;
            bullet.transform.rotation = rotation;
            bullet.gameObject.SetActive(isActive);
        }
        return bullet;
    }

    public override void Return(int type, Bullet obj)
    {
        bulletPool[type].Return(obj);
    }

    public override void Add(Bullet obj)
    {
        throw new System.NotImplementedException();
    }

    public override void Add(Bullet[] obj)
    {
        throw new System.NotImplementedException();
    }
}
