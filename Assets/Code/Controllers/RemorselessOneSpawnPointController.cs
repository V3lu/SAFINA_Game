using UnityEngine;

public class RemorselessOneSpawnPointController : MonoBehaviour
{
    [SerializeField] GameObject _remorselessOneGameObject;
    [SerializeField] float _spawnRate;

    float _spawnTimer;
    bool _exists = false;
    int _startingLevel = -1;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (_startingLevel == -1)
        {
            if (GameManager.Player != null)
            {
                _startingLevel = GameManager.Player.GetCurrentLvl();
            }
            else
            {
                return;
            }
        }

        _spawnTimer -= Time.deltaTime;

        if (_spawnTimer <= 0 && !_exists && GameManager.Player.GetCurrentLvl() >= _startingLevel + 2)
        {
            _spawnTimer = _spawnRate;
            ObjectPoolManager.SpawnObject(_remorselessOneGameObject, transform.position, Quaternion.identity, ObjectPoolManager.PoolType.Mobs);
            _exists = true;
        }
    }
}
