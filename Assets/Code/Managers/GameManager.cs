using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    static GameManager _instance;

    public static PlayerCtrl Player { get; private set; }

    void Awake()
    {
        // Singleton pattern with DontDestroyOnLoad
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;

        FindPlayer();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindPlayer();
    }

    static void FindPlayer()
    {
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            Player = playerObj.GetComponent<PlayerCtrl>();
        }
        else
        {
            Player = null;
            Debug.LogWarning("[GameManager] Player not found in scene. This is expected during loading screens.");
        }
    }
}
