using UnityEngine;

public class CrystalineSlimeSpawnPointController : MonoBehaviour
{
    [SerializeField] GameObject _crystalineSlimePrefab;
    [SerializeField] float _spawnRate;

    float _spawnTimer;

    
    void Start()
    {
        // Start the timer at the spawn rate + 1 second so they don't spawn instantly
        _spawnTimer = _spawnRate + 1f;
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
