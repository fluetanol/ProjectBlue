using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolingTest : MonoBehaviour
{
    [SerializeField] private float waittime = 3f;
    [SerializeField] private float time = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(test());
    }

    

    // Update is called once per frame
    IEnumerator test(){
        int i = 0;
        Queue<EnemyMovement> enemyQueue = new Queue<EnemyMovement>();
        yield return new WaitForSeconds(waittime);
        while (true){
            print("get enemy from pool");
            var get = ObjectPoolManager.Instance.Get<EnemyMovement>(PoolType.Enemy);
            if(get!= null) enemyQueue.Enqueue(get);
    
            yield return new WaitForSeconds(time);
            i++;
            if(i>5){
                ObjectPoolManager.Instance.Return(PoolType.Enemy, enemyQueue.Dequeue());
            }
        }
 
    }
}
