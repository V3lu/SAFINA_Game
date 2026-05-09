using UnityEngine;

public class SeleniusSpawnPointController : MonoBehaviour
{
    [SerializeField] GameObject _seleniusGameObject;
    [SerializeField] float _spawnRate;

    float _spawnTimer;
    bool _exists = false;


    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        _spawnTimer -= Time.deltaTime;

        if (_spawnTimer <= 0 && !_exists)
        {
            _spawnTimer = _spawnRate;
            ObjectPoolManager.SpawnObject(_seleniusGameObject, transform.position, Quaternion.identity, ObjectPoolManager.PoolType.Mobs);
            _exists = true;
        }
    }
}
