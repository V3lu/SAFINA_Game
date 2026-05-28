using TMPro;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _textMeshProUGUI;

    public static float _time;
    public static bool IsStopped = false;

    // Update is called once per frame
    void Update()
    {
        if (IsStopped) return;

        if (HUDManager.Instance != null && HUDManager.Instance.IsTutorialActive())
        {
            _time = 0f;
        }
        else
        {
            _time += Time.deltaTime;
        }
        
        var timeTextObj = GameObject.FindGameObjectWithTag("TimeText");
        if (timeTextObj != null)
        {
            _textMeshProUGUI = timeTextObj.GetComponent<TextMeshProUGUI>();
            if (_textMeshProUGUI != null)
            {
                _textMeshProUGUI.text = _time.ToString("F2");
            }
        }
    }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
