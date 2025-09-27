using Assets.Code.Interfaces;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;
using UnityEngine.Pool;
using static Unity.VisualScripting.Metadata;

public class ObjectPoolManager : MonoBehaviour
{
    [SerializeField] bool _addToDontDestroyOnLoad = false; //This one is for ex.sounds that you'll want to keep playing when scene loads or sth like that


    GameObject _emptyHolder;

    static GameObject _particleSystemsEmpty;
    static GameObject _gameObjectsEmpty;
    static GameObject _soundFXEmpty;
    static GameObject _mobsEmpty;
    static GameObject _VFXEmpty;
    static GameObject _projectilesEmpty;
    static GameObject _collectablesEmpty;
    static Dictionary<GameObject, ObjectPool<GameObject>> _objectPools;
    static Dictionary<GameObject, GameObject> _spawnedInstanceToOriginalPrefabMap;


    public enum PoolType
    {
        ParticleSystems,
        GameObjects,
        SoundFX,
        Mobs,
        VFXs,
        Projectiles,
        Collectables
    }
    public static PoolType poolingType;


    void Awake()
    {
        _objectPools = new Dictionary<GameObject, ObjectPool<GameObject>>();
        _spawnedInstanceToOriginalPrefabMap = new Dictionary<GameObject, GameObject>();
        SetupEmpties();
    }

    void SetupEmpties()
    {
        _emptyHolder = new GameObject("Object Pools");

        _particleSystemsEmpty = new GameObject("Particle Effects");
        _particleSystemsEmpty.transform.SetParent(_emptyHolder.transform);

        _gameObjectsEmpty = new GameObject("GameObjects");
        _gameObjectsEmpty.transform.SetParent(_emptyHolder.transform);

        _soundFXEmpty = new GameObject("Sound Effects");
        _soundFXEmpty.transform.SetParent(_emptyHolder.transform);

        _mobsEmpty = new GameObject("Mobs");
        _mobsEmpty.transform.SetParent(_emptyHolder.transform);

        _VFXEmpty = new GameObject("VFXs");
        _VFXEmpty.transform.SetParent(_emptyHolder.transform);

        _projectilesEmpty = new GameObject("Projectiles");
        _projectilesEmpty.transform.SetParent(_emptyHolder.transform);

        _collectablesEmpty = new GameObject("Collectables");
        _collectablesEmpty.transform.SetParent(_emptyHolder.transform);

        if (_addToDontDestroyOnLoad)
        {
            DontDestroyOnLoad(_particleSystemsEmpty.transform.root);
        }
    }

    static void CreatePool(GameObject prefab, Vector3 pos, Quaternion rot, PoolType poolType = PoolType.GameObjects)
    {
        ObjectPool<GameObject> pool = new ObjectPool<GameObject>(
            createFunc: () => CreateObject(prefab, pos, rot, poolType),
            actionOnGet: OnGetObject,
            actionOnRelease: OnReleaseObject,
            actionOnDestroy: OnDestroyObject);

        _objectPools.Add(prefab, pool);
    }

    static GameObject CreateObject(GameObject prefab, Vector3 pos, Quaternion rot, PoolType poolType = PoolType.GameObjects)
    {
        prefab.SetActive(false);    //Only way to spawn an object and not have to wait for OnEnabled call guaranteed - saves headache

        GameObject obj = Instantiate(prefab, pos, rot);

        prefab.SetActive(true);

        GameObject parentObject = SetParentObject(poolType);
        obj.transform.SetParent(parentObject.transform);

        return obj;
    }

    static void OnGetObject(GameObject obj)
    {
        // logic for what to do when we get object from the pool
    }

    static void OnReleaseObject(GameObject obj)
    {
        // logic for what to do when we release an object back to the pool
        obj.SetActive(false);
    }

    static void OnDestroyObject(GameObject obj)
    {
        if (_spawnedInstanceToOriginalPrefabMap.ContainsKey(obj))
        {
            _spawnedInstanceToOriginalPrefabMap.Remove(obj);
        }
    }

    static GameObject SetParentObject(PoolType poolType)
    {
        switch(poolType)
        {
            case PoolType.ParticleSystems:

                return _particleSystemsEmpty;

            case PoolType.GameObjects:

                return _gameObjectsEmpty;

            case PoolType.SoundFX:

                return _soundFXEmpty;

            case PoolType.Mobs:

                return _mobsEmpty;

            case PoolType.VFXs:

                return _VFXEmpty;

            case PoolType.Projectiles:

                return _projectilesEmpty;

            case PoolType.Collectables:

                return _collectablesEmpty;

            default:
                return null;
        }
    }

    static T SpawnObject<T>(GameObject objectToSpawn, Vector3 spawnPos, Quaternion spawnRotation, PoolType poolType = PoolType.GameObjects) where T : Object
    {
        if (!_objectPools.ContainsKey(objectToSpawn))
        {
            CreatePool(objectToSpawn, spawnPos, spawnRotation, poolType);
        }

        GameObject obj = _objectPools[objectToSpawn].Get();   //Gets an object from the pool

        if (obj != null)
        {
            if (!_spawnedInstanceToOriginalPrefabMap.ContainsKey(obj))
            {
                _spawnedInstanceToOriginalPrefabMap.Add(obj, objectToSpawn);
            }

            obj.transform.position = spawnPos;
            obj.transform.rotation = spawnRotation;
            obj.SetActive(true);

            if (typeof(T) == typeof(GameObject))
            {
                return obj as T;
            }

            T component = obj.GetComponent<T>();
            if (component == null)
            {
                Debug.LogError($"Object {objectToSpawn.name} doesn't have component of type {typeof(T)}");
                return null;
            }

            return component;
        }

        return null;
    }

    public static T SpawnObject<T>(T typePrefab, Vector3 spawnPos, Quaternion spawnRotation, PoolType poolType = PoolType.GameObjects) where T : Component
    {
        return SpawnObject<T>(typePrefab.gameObject, spawnPos, spawnRotation, poolType);
    }

    public static GameObject SpawnObject(GameObject objectToSpawn, Vector3 spawnPos, Quaternion spawnRotation, PoolType poolType = PoolType.GameObjects)
    {
        return SpawnObject<GameObject>(objectToSpawn, spawnPos, spawnRotation, poolType);
    }

    public static void ReturnObjectToPool(GameObject obj, PoolType poolType = PoolType.GameObjects)
    {
        if (_spawnedInstanceToOriginalPrefabMap.TryGetValue(obj, out GameObject prefab))
        {
            GameObject parentObject = SetParentObject(poolType);

            if (obj.transform.parent != parentObject.transform)
            {
                obj.transform.SetParent(parentObject.transform);
            }

            if (_objectPools.TryGetValue(prefab, out ObjectPool<GameObject> pool))
            {
                pool.Release(obj);
            }
        }
        else
        {
            Debug.LogWarning("Trying to return an object that is not pooled: " + obj.name);
        }
    }
    
    public static Dictionary<GameObject, GameObject> GetAllActiveGameObjectsOfThePool(PoolType poolType = PoolType.GameObjects)
    {
        Dictionary<GameObject, GameObject> activeChildrenWithOriginalPrebaf = new();
        Transform transform = SetParentObject(poolType).GetComponentInChildren<Transform>();
        foreach (Transform child in transform)
        {
            if (_spawnedInstanceToOriginalPrefabMap.TryGetValue(child.gameObject, out GameObject originalPrefab))
            {
                if (child.gameObject.activeSelf) // Match prefab AND is active
                {
                    activeChildrenWithOriginalPrebaf[child.gameObject] = originalPrefab;
                }
            }
            
        }

        return activeChildrenWithOriginalPrebaf;


    }
}
