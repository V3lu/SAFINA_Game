using UnityEngine;

public class ProjectileVisual : MonoBehaviour
{
    [SerializeField] private Transform _projectileVisual;
    [SerializeField] private Transform _projectileShadow;
    [SerializeField] private SeleniteGeodeProjectile _SGP;

    private float _prevRotation = 0f;
    void Update()
    {
        UpdateProjectileLocation();
    }

    void UpdateProjectileLocation()
    {
        Vector3 projectileMoveDirection = _SGP.GetDirection();

        _projectileVisual.transform.rotation = Quaternion.Euler(0, 0, _prevRotation);
        _projectileShadow.transform.rotation = Quaternion.Euler(0, 0, _prevRotation);

        _prevRotation -= 2f;
    }
}
