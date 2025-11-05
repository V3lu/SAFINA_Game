using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerLightController : MonoBehaviour
{
    [SerializeField] private Transform _character; // Assign your character in the Inspector
    private float _maxDistance = 20f; // distance where light intensity = 0

    private Light2D _light2D;

    void Start()
    {
        _light2D = GetComponent<Light2D>();
    }

    void Update()
    {
        if (_character == null || _light2D == null) return;

        float distance = Vector2.Distance(transform.position, _character.position);

        // Calculate intensity falloff
        float intensity = Mathf.Pow(Mathf.Clamp01(1f - (distance / _maxDistance)), 2f);

        _light2D.intensity = intensity;
    }
}
