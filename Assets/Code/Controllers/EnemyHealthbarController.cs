using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthbarController : MonoBehaviour
{
    [SerializeField] Slider _healthBarSlider;
    [SerializeField] Color _healthBarColor;
    [SerializeField] Vector3 _offset;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _healthBarSlider.transform.position = Camera.main.WorldToScreenPoint(transform.parent.position + _offset);
    }

    public void Sethealth(float health, float maxHealth)
    {
        _healthBarSlider.gameObject.SetActive(health < maxHealth);
        _healthBarSlider.value = health;
        _healthBarSlider.maxValue = maxHealth;
        _healthBarSlider.fillRect.GetComponentInChildren<Image>().color = _healthBarColor;
    }
}
