using UnityEngine;

public class Spawn : MonoBehaviour
{
    [SerializeField] private GameObject _enemyObject;
    [SerializeField] private float _spawnCycle = 3f;
    [SerializeField] private int    _spawnCount = 3;
    [SerializeField] private int   _maxSpawn = 30;

    private float _deltaTime = 3f;
    private int _enemyCount = 0;

    // Update is called once per frame
    void Update()
    {
        if(_enemyCount < _maxSpawn){
            _deltaTime -= Time.smoothDeltaTime;
            if(_deltaTime <= 0){
                _deltaTime = _spawnCycle;
                for(int i = 0; i < _spawnCount; i++){
                    Instantiate(_enemyObject, transform.position, Quaternion.identity);
                }
                _enemyCount += _spawnCount;
            }
        }

    }
}
