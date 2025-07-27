using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    [SerializeField] private List<GameObject> _enemyObject;
    [SerializeField] private int       _enemyMaxCode = 2;
    [SerializeField] private float      _spawnCycle = 3f;
    [SerializeField] private int        _spawnCount = 3;
    [SerializeField] private int        _maxSpawn = 30;

    private int     _enemyCount = 0;


    void Start(){
        StartCoroutine(EnemySpawn());
    }

    IEnumerator EnemySpawn(){
        WaitForSeconds waitForSeconds = new WaitForSeconds(_spawnCycle);
        while (_enemyCount < _maxSpawn)
        {

            yield return waitForSeconds;

            for (int i = 0; i < _spawnCount; i++)
            {
                int randomIndex = Random.Range(0,  _enemyMaxCode);
                //print("randomIndex : " + randomIndex  + " " + _enemyObject.Count);
                Enemy em = EnemyPoolManager.Instance.Get(randomIndex, transform.position, Quaternion.identity, true);
                if (em == null)
                {
                    Debug.LogWarning("no enemy in pool");
                    continue;
                }
                em.gameObject.SetActive(true);
                //print("enemy spawn : " + em.gameObject.name);
                //Instantiate(enemyObject, transform.position, Quaternion.identity);
            }

            _enemyCount += _spawnCount;
        }
    }

}
