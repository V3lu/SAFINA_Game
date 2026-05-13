using TMPro;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _textMeshProUGUI;

    public static float _time;

    // Update is called once per frame
    void Update()
    {
        _time += Time.deltaTime;
        this._textMeshProUGUI = GameObject.FindGameObjectWithTag("TimeText").GetComponent<TextMeshProUGUI>();
        _textMeshProUGUI.text = _time.ToString("F2");
    }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
