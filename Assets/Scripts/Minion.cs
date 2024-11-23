using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(Rigidbody2D))]
public class Minion : Entity
{
    private ObjectPool<Minion> _pool;
    private EntityState _currentState;

    private List<Vector3> _path;
    private int _currentPathIndex = 0;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        _movementSpeed = 2f;
        if (EntityTeam == Team.Blue)
        {
            _moveDir = Vector2.right;
        }
        else if (EntityTeam == Team.Red)
        {
            _moveDir = Vector2.left;
        }
        _targetingPriority = new List<Type> { Type.Player, Type.AIPlayer, Type.Minion, Type.Tower };
        _currentState = EntityState.MovingWithoutTarget;
    }

    protected override void UpdateMovement(Vector3 moveDir)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(moveDir.x * _movementSpeed, moveDir.y * _movementSpeed);
    }

    private void StartPathfinding()
    {
        _path = Pathfinding.FindPath(transform.position, _targetedEntity.transform.position);
        _currentPathIndex = 0;
    }

    private void Update()
    {
        switch (_currentState)
        {
            case EntityState.MovingTowardsTarget:
                MoveTowardsTarget();
                break;

            case EntityState.MovingWithoutTarget:
                MoveWithoutTarget();
                break;

            case EntityState.CastingAbility:
                CastAbility();
                break;
        }
    }

    // Move in the predefined direction when no target is in range
    private void MoveWithoutTarget()
    {
        if (_targetedEntity != null)
        {
            _currentState = EntityState.MovingTowardsTarget;
        }

        _movementSpeed = 2f;
        UpdateMovement(_moveDir);
    }

    private void CastAbility()
    {
        _movementSpeed = 0;
        MageMinionAttack minionAttack = GetComponentInChildren<MageMinionAttack>();
        minionAttack.CastAbility(transform.position, this);

        if (_targetedEntity == null)
        {
            _currentState = EntityState.MovingWithoutTarget;
        }
    }

    // Move towards target enemy within range
    private void MoveTowardsTarget()
    {
        // No target in range: move without target
        if (_targetedEntity == null)
        {
            _currentState = EntityState.MovingWithoutTarget;
            return;
        }

        // Calculate distance to the targeted entity
        float distanceToTarget = Vector3.Distance(transform.position, _targetedEntity.transform.position);
        float stoppingDistance = _attackRange.radius;

        // Stop moving and switch to attack if within range
        if (distanceToTarget <= stoppingDistance)
        {
            _currentState = EntityState.CastingAbility;
            return;
        }

        // Recalculate path if null or completed
        if (_path == null || _currentPathIndex >= _path.Count)
        {
            StartPathfinding();
            return;
        }

        // Move towards the next waypoint on the path
        Vector3 targetWaypoint = _path[_currentPathIndex];
        if (Vector3.Distance(transform.position, targetWaypoint) < 0.5f)
        {
            _currentPathIndex++;
            if (_currentPathIndex >= _path.Count) return;
        }

        Vector3 pathDirection = (targetWaypoint - transform.position).normalized;
        Vector3 boidAdjustment = CalculateBoidAdjustment();

        // Combine path direction with boid adjustment
        Vector3 moveDirection = (pathDirection + boidAdjustment).normalized;
        UpdateMovement(moveDirection);
    }

    public void SetPool(ObjectPool<Minion> pool) => _pool = pool;

    public override void Death(Entity entity)
    {
        base.Death(entity);
        gameObject.SetActive(false);
        if (_pool != null)
        {
            _pool.Release(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Find the movement direction according to the nearby minions and return it
    private Vector3 CalculateBoidAdjustment()
    {
        Vector3 separation = Vector3.zero;
        Vector3 alignment = Vector3.zero;
        Vector3 cohesion = Vector3.zero;
        int count = 0;

        Collider2D[] nearbyMinions = Physics2D.OverlapCircleAll(transform.position, 2f);
        foreach (Collider2D other in nearbyMinions)
        {
            if (other.gameObject != gameObject && other.TryGetComponent(out Minion minion))
            {
                // Separation: Avoid collision
                separation += (transform.position - other.transform.position).normalized / (transform.position - other.transform.position).magnitude;

                // Alignment: Match velocity
                alignment += new Vector3 (minion.GetComponent<Rigidbody2D>().velocity.x , minion.GetComponent<Rigidbody2D>().velocity.y,0);

                // Cohesion: Move towards average position
                cohesion += other.transform.position;

                count++;
            }
        }

        if (count > 0)
        {
            separation /= count;
            alignment /= count;
            cohesion /= count;

            cohesion = (cohesion - transform.position).normalized;
        }

        return (separation * 2.5f + alignment * 0.5f + cohesion * 1.0f).normalized;
    }

    private enum EntityState
    {
        MovingTowardsTarget,
        MovingWithoutTarget,
        CastingAbility
    }
}


