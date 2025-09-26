using Assets.Code.Interfaces;
using UnityEngine;

public class SeleniteGeodeProjectile : MonoBehaviour, IProjectile
{
    [SerializeField] private SeleniteGeodeProjectileSO _seleniteGeodeProjectileSO;

    private AnimationCurve _trajectoryAnimationCurveFromGeodeReference;
    private AnimationCurve _axiscorrectionAnimationCurveFromGeodeReference;
    private AnimationCurve _projectileSpeedAnimationCurveFromGeodeReference;
    private Vector3 _direction = Vector3.zero;
    private Vector3 _trajectoryStartingPoint;
    private float _trajectoryMaxRelativeHeight;
    private Vector3 target;
    private float _movespeed;
    private float _maxMoveSpeed;
    private float _distanceToTargetToDestroyProjectile = 0.2f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _trajectoryStartingPoint = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateProjectilePosition();
        if (Vector3.Distance(transform.position, target) < _distanceToTargetToDestroyProjectile)
        {
            ObjectPoolManager.ReturnObjectToPool(this.gameObject, ObjectPoolManager.PoolType.Projectiles);
            ResetAfterPoolReturn();
        }
    }

    void UpdateProjectilePosition()
    {
        Vector3 trajectoryRange = target - _trajectoryStartingPoint;

        if(trajectoryRange.x < 0f)
        {
            //Other side of the player
            _movespeed = -_movespeed;
        }

        float nextPositionX = transform.position.x + _movespeed * Time.deltaTime;
        float nextPositionXNormalized = (nextPositionX - _trajectoryStartingPoint.x) / trajectoryRange.x;
        float nextPositionYNormalized = _trajectoryAnimationCurveFromGeodeReference.Evaluate(nextPositionXNormalized);
        float nextPositionYCorrectionNormalized = _axiscorrectionAnimationCurveFromGeodeReference.Evaluate(nextPositionXNormalized);
        float nextPositionYCorrectionAbsolute = nextPositionYCorrectionNormalized * trajectoryRange.y;
        float nextPositionY = _trajectoryStartingPoint.y + nextPositionYNormalized * _trajectoryMaxRelativeHeight + nextPositionYCorrectionAbsolute;

        Vector3 newPosition = new Vector3(nextPositionX, nextPositionY, 0);

        CalculateProjectileSpeed(nextPositionXNormalized);
        _direction = newPosition - transform.position;
        transform.position = newPosition;
    }
    void ResetAfterPoolReturn()
    {
        _direction = Vector3.zero;
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
        this.target = target;
    }

    public void InitializeProjectile(Vector3 target, float maxMoveSpeed, float trajectoryMaxHeight)
    {
        SetTarget(target);
        this._maxMoveSpeed = maxMoveSpeed;
        float distanceToTarget = target.x - this.transform.position.x;
        this._trajectoryMaxRelativeHeight = Mathf.Abs(distanceToTarget) * trajectoryMaxHeight;
    }

    public Vector3 GetDirection()
    {
        return _direction;
    }
}
