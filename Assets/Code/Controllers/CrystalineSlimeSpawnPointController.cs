using UnityEngine;

public class CrystalineSlimeSpawnPointController : MonoBehaviour
{
    [SerializeField] GameObject _crystalineSlimePrefab;
    [SerializeField] float _spawnRate;
    [SerializeField] CrystalinePathSO _crystalinePathSO;

    float _spawnTimer;

    
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        _spawnTimer -= Time.deltaTime;

        if(_spawnTimer <= 0)
        {
            _spawnTimer = _spawnRate;
            ObjectPoolManager.SpawnObject(_crystalineSlimePrefab, transform.position, Quaternion.identity, ObjectPoolManager.PoolType.Mobs);
        }
    }
}
