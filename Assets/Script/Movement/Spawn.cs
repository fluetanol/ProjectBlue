using System.Collections;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    [SerializeField] private GameObject _enemyObject;
    [SerializeField] private float      _spawnCycle = 3f;
    [SerializeField] private int        _spawnCount = 3;
    [SerializeField] private int        _maxSpawn = 30;

    private int     _enemyCount = 0;


    void Start(){
        StartCoroutine(EnemySpawn());
    }

    IEnumerator EnemySpawn(){
        while(_enemyCount < _maxSpawn){

            yield return new WaitForSeconds(_spawnCycle);

            for(int i = 0; i < _spawnCount; i++){
                Instantiate(_enemyObject, transform.position, Quaternion.identity);
            }

            _enemyCount += _spawnCount;
        }
    }

}
