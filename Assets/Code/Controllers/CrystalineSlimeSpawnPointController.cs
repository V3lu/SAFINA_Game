using UnityEngine;

public class CrystalineSlimeSpawnPointController : MonoBehaviour
{
    [SerializeField] private GameObject _crystalineSlimePrefab;
    [SerializeField] private Transform _playerTransform;
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

        if(_spawnTimer <= 0)
        {
            _spawnTimer = _spawnRate;
            CrystalineSlime slime = Instantiate(_crystalineSlimePrefab, transform.position, Quaternion.identity).GetComponent<CrystalineSlime>();
            slime.Spawn(_playerTransform);
            _crystalinePathSO.AddEnemy(slime);
        }
    }
}
