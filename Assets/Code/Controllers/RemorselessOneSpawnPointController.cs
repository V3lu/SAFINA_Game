using UnityEngine;

public class RemorselessOneSpawnPointController : MonoBehaviour
{
    [SerializeField] GameObject _remorselessOneGameObject;
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

        if (_spawnTimer <= 0 && !_exists && GameManager.Player.GetCurrentLvl() > 0)
        {
            _spawnTimer = _spawnRate;
            ObjectPoolManager.SpawnObject(_remorselessOneGameObject, transform.position, Quaternion.identity, ObjectPoolManager.PoolType.Mobs);
            _exists = true;
        }
    }
}
