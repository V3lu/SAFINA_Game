using UnityEngine;

public class MehrenSpawnPointController : MonoBehaviour
{
    [SerializeField] GameObject _mehrenPrefab;
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

        if (_spawnTimer <= 0)
        {
            _spawnTimer = _spawnRate;
            
            bool playerTooClose = false;
            if (GameManager.Player != null && Vector3.Distance(transform.position, GameManager.Player.transform.position) < 5f)
            {
                playerTooClose = true;
            }

            if (!playerTooClose)
            {
                ObjectPoolManager.SpawnObject(_mehrenPrefab, transform.position, Quaternion.identity, ObjectPoolManager.PoolType.Mobs);
            }
        }
    }
}
