using UnityEngine;

public class EnemyWithWaypoints : Enemy
{
    [SerializeField]
    private Waypoints _waypoints;
    private int _currentWaypoint = -1;
    private Vector3 _currentDirection = Vector3.zero;
    private Vector3 _targetPosition = Vector3.zero;
    private float _deltaRelativeX;

    public Waypoints Waypoints 
    {
        get
        {
            return _waypoints;
        }

        set
        {
            _waypoints = value;
        }
    }

    protected int CurrentWaypoint => _currentWaypoint;

    protected override void Move(bool isAlive)
    {
        if (isAlive)
        {
            if (_currentWaypoint == -1)
            {
                var startPosition = transform.position;
                _deltaRelativeX = (_waypoints.GetWaypoints[0].position.x - startPosition.x) * -1;
                SetNextWaypoint();
            }

            var moveDirection = _currentDirection * Time.deltaTime * _speed;
            transform.Translate(moveDirection);

            const float nearEnoughToTargetDistance = 1f;
            var distanceToGoal = Vector3.Distance(transform.position, _targetPosition);
            if (_currentWaypoint < _waypoints.GetWaypoints.Length - 1 && distanceToGoal < nearEnoughToTargetDistance)
            {
                SetNextWaypoint();
            }

            if (ClampBoundaries(moveDirection))
            {
                CalculateDirection();
            }
        }
    }

    protected virtual void SetNextWaypoint()
    {
        _currentWaypoint = Mathf.Clamp(_currentWaypoint + 1, 0, _waypoints.GetWaypoints.Length - 1);
        _targetPosition = _waypoints.GetWaypoints[_currentWaypoint].position;
        _targetPosition.x += _deltaRelativeX;
        CalculateDirection();
    }

    private void CalculateDirection()
    {
        _currentDirection = Vector3.Normalize(_targetPosition - transform.position);
    }
}
