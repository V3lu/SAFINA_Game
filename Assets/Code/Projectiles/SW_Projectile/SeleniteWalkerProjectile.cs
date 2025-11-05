using Assets.Code.Interfaces;
using Unity.VisualScripting;
using UnityEngine;

public class SeleniteWalkerProjectile : MonoBehaviour, IProjectile
{
    [SerializeField] SeleniteWalkerProjectileSO _seleniteWalkerProjectileSO;
    [SerializeField] SeleniteWalkerAoE _seleniteWalkerAoE;

    AnimationCurve _trajectoryAnimationCurveFromGeodeReference;
    AnimationCurve _axiscorrectionAnimationCurveFromGeodeReference;
    AnimationCurve _projectileSpeedAnimationCurveFromGeodeReference;
    Vector3 _direction = Vector3.zero;
    Vector3 _trajectoryStartingPoint;
    float _trajectoryMaxRelativeHeight;
    Vector3 _targetVec3;
    Transform _targetTransform;
    float _movespeed;
    float _maxMoveSpeed;
    float _distanceToTargetToDestroyProjectile = 0.2f;
    float _nextYTrajectoryPosition;
    float _nextPositionYCorrectionAbsolute;
    Vector3 _trajectoryRange;
    float _nextXTrajectoryPosition;
    float _nextPositionXCorrectionAbsolute;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateProjectilePosition();
        if (Vector3.Distance(transform.position, _targetVec3) < _distanceToTargetToDestroyProjectile)
        {
            ObjectPoolManager.ReturnObjectToPool(this.gameObject, ObjectPoolManager.PoolType.Projectiles);
            this.GetComponentInChildren<TrailRenderer>().enabled = false;
            ResetAfterPoolReturn();
            ObjectPoolManager.SpawnObject(_seleniteWalkerAoE, this.gameObject.transform.position, Quaternion.identity, ObjectPoolManager.PoolType.VFXs);
        }
    }

    void UpdateProjectilePosition()
    {
        _trajectoryRange = _targetVec3 - _trajectoryStartingPoint;

        if(Mathf.Abs(_trajectoryRange.normalized.x) < Mathf.Abs(_trajectoryRange.normalized.y))
        {
            if (_trajectoryRange.y < 0)
            { 
                _movespeed = -_movespeed;
            }
            // Projectile will be curved on the X axis
            UpdatePositionWithXCurve();
        }
        else
        {
            if (_trajectoryRange.x < 0)
            {
                _movespeed = -_movespeed;
            }
            UpdatePositionWithYCurve();
        }
    }
    void ResetAfterPoolReturn()
    {
        _direction = Vector3.zero;
        this.GetComponentInChildren<TrailRenderer>().enabled = true;
    }

    void UpdatePositionWithXCurve()
    {
        float nextPositionY = transform.position.y + _movespeed * Time.deltaTime;
        float nextPositionYNormalized = (nextPositionY - _trajectoryStartingPoint.y) / _trajectoryRange.y;
        float nextPositionXNormalized = _trajectoryAnimationCurveFromGeodeReference.Evaluate(nextPositionYNormalized);
        _nextXTrajectoryPosition = nextPositionXNormalized * _trajectoryMaxRelativeHeight;
        float nextPositionXCorrectionNormalized = _axiscorrectionAnimationCurveFromGeodeReference.Evaluate(nextPositionYNormalized);
        _nextPositionXCorrectionAbsolute = nextPositionXCorrectionNormalized * _trajectoryRange.x;

        if(_trajectoryRange.x > 0 && _trajectoryRange.y > 0)
        {
            _nextXTrajectoryPosition = -_nextXTrajectoryPosition;
        }

        if (_trajectoryRange.x < 0 && _trajectoryRange.y < 0)
        {
            _nextXTrajectoryPosition = -_nextXTrajectoryPosition;
        }

        float nextPositionX = _trajectoryStartingPoint.x + _nextXTrajectoryPosition + _nextPositionXCorrectionAbsolute;

        Vector3 newPosition = new Vector3(nextPositionX, nextPositionY, 0);

        CalculateProjectileSpeed(nextPositionYNormalized);
        _direction = newPosition - transform.position;
        transform.position = newPosition;
    }

    void UpdatePositionWithYCurve()
    {
        float nextPositionX = transform.position.x + _movespeed * Time.deltaTime;
        float nextPositionXNormalized = (nextPositionX - _trajectoryStartingPoint.x) / _trajectoryRange.x;
        float nextPositionYNormalized = _trajectoryAnimationCurveFromGeodeReference.Evaluate(nextPositionXNormalized);
        _nextYTrajectoryPosition = nextPositionYNormalized * _trajectoryMaxRelativeHeight;
        float nextPositionYCorrectionNormalized = _axiscorrectionAnimationCurveFromGeodeReference.Evaluate(nextPositionXNormalized);
        _nextPositionYCorrectionAbsolute = nextPositionYCorrectionNormalized * _trajectoryRange.y;
        float nextPositionY = _trajectoryStartingPoint.y + _nextYTrajectoryPosition + _nextPositionYCorrectionAbsolute;

        Vector3 newPosition = new Vector3(nextPositionX, nextPositionY, 0);

        CalculateProjectileSpeed(nextPositionXNormalized);
        _direction = newPosition - transform.position;
        transform.position = newPosition;
    }
    void OnTriggerEnter2D(Collider2D collision)
    {

    }

    public void InitializeAnimationCurves(AnimationCurve curve, AnimationCurve axisCorrectionCurve, AnimationCurve speedCorrectionCurve)
    {
        this._trajectoryAnimationCurveFromGeodeReference = curve;
        this._axiscorrectionAnimationCurveFromGeodeReference = axisCorrectionCurve;
        this._projectileSpeedAnimationCurveFromGeodeReference = speedCorrectionCurve;
    }

    void CalculateProjectileSpeed(float nextPositionXNormalized)
    {
        float nextMoveSpeedNormalized = _projectileSpeedAnimationCurveFromGeodeReference.Evaluate(nextPositionXNormalized);

        _movespeed = nextMoveSpeedNormalized * _maxMoveSpeed;
    }

    public void SetTarget(Vector3 target)
    {
        this._targetVec3 = target;
    }

    public void InitializeProjectile(Transform targetTransform, Vector3 target, float maxMoveSpeed, float trajectoryMaxHeight, Vector3 trajectoryStartingPoint)
    {
        SetTarget(target);
        this._targetTransform = targetTransform;
        ProjectileVisual visual = this.gameObject.GetComponentInChildren<ProjectileVisual>();
        visual.SetTarget(_targetTransform.position);
        this._maxMoveSpeed = maxMoveSpeed;
        float distanceToTarget = target.x - this.transform.position.x;
        this._trajectoryMaxRelativeHeight = Mathf.Abs(distanceToTarget) * trajectoryMaxHeight;
        this._trajectoryStartingPoint = trajectoryStartingPoint;
    }

    public Vector3 GetDirection()
    {
        return _direction;
    }

    public float GetNextYTrajectoryPosition()
    {
        return this._nextYTrajectoryPosition;
    }

    public float GetNextPositionYCorrectionAbsolute()
    {
        return this._nextPositionYCorrectionAbsolute;
    }

    public float GetNextXTrajectoryPosition()
    {
        return this._nextXTrajectoryPosition;
    }

    public float GetNextPositionXCorrectionAbsolute()
    {
        return this._nextPositionXCorrectionAbsolute;
    }
}
