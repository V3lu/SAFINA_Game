using Unity.VisualScripting;
using UnityEngine;

public class ProjectileVisual : MonoBehaviour
{
    [SerializeField] Transform _projectileVisual;
    [SerializeField] Transform _projectileShadow;
    [SerializeField] SeleniteWalkerProjectile _SWP;

    float _prevRotation = 0f;
    Vector3 _target;
    Vector3 _trajectoryStartPosition;
    float shadowPositionDivider = 6f;

    void Start()
    {
        _trajectoryStartPosition = transform.position;
    }
    void Update()
    {
        UpdateProjectileRotation();
        UpdateShadowPosition();

    }
    
    void UpdateShadowPosition()
    {
        Vector3 newPosition = transform.position;
        Vector3 _trajectoryRange = _target - _trajectoryStartPosition;

        if(Mathf.Abs(_trajectoryRange.normalized.x) < Mathf.Abs(_trajectoryRange.normalized.y))
        {
            // Projectile is curved on the X axis
            newPosition.x = _trajectoryStartPosition.x + _SWP.GetNextXTrajectoryPosition() / shadowPositionDivider + _SWP.GetNextPositionXCorrectionAbsolute();

        }
        else
        {
            // Projectile is curved on the Y axis
            newPosition.y = _trajectoryStartPosition.y + _SWP.GetNextYTrajectoryPosition() / shadowPositionDivider + _SWP.GetNextPositionYCorrectionAbsolute();
        }



        _projectileShadow.position = newPosition;
    }

    void UpdateProjectileRotation()
    {
        Vector3 projectileMoveDirection = _SWP.GetDirection();

        if (GameManager.Player.transform.position.x > projectileMoveDirection.x)
        {
            _projectileVisual.transform.rotation = Quaternion.Euler(0, 0, _prevRotation);
            _projectileShadow.transform.rotation = Quaternion.Euler(0, 0, _prevRotation);

            _prevRotation -= 2f;
        }
        else
        {
            _projectileVisual.transform.rotation = Quaternion.Euler(0, 0, _prevRotation);
            _projectileShadow.transform.rotation = Quaternion.Euler(0, 0, _prevRotation);

            _prevRotation += 2f;
        }

    }

    public void SetTarget(Vector3 target)
    {
        this._target = target;
    }
}
