using UnityEngine;

public class SeleniteGeodeSpawnPointController : MonoBehaviour
{
    [SerializeField] private GameObject _seleniteGeodePrefab;
    [SerializeField] private float _spawnRate;
    [SerializeField] private CrystalinePathSO _crystalinePathSO;

    private float _spawnTimer;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _spawnTimer -= Time.deltaTime;

        if (_spawnTimer <= 0)
        {
            _spawnTimer = _spawnRate;
            ObjectPoolManager.SpawnObject(_seleniteGeodePrefab, transform.position, Quaternion.identity, ObjectPoolManager.PoolType.Mobs);
        }
    }
}
