using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(Rigidbody2D))]
public class Minion : Entity
{
    private ObjectPool<Minion> _pool;
    private EntityState _currentState;

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
        MinionAttack minionAttack = GetComponentInChildren<MinionAttack>();
        minionAttack.CastAbility(transform.position, this);

        if (_targetedEntity == null)
        {
            _currentState = EntityState.MovingWithoutTarget;
        }
    }

    private void MoveTowardsTarget()
    {

        if (_targetedEntity != null)
        {
            Vector3 targetPos = _targetedEntity.transform.position;
            Vector3 minionPos = transform.position;
            Vector3 moveDir = (targetPos - minionPos).normalized;
            Vector3 movement = _movementSpeed * Time.deltaTime * moveDir;
           

            float distanceToTarget = Vector3.Distance(minionPos, targetPos);
            float stoppingDistance = _attackRange.radius;

            // Go towards the target as long as not in ability range
            if (distanceToTarget > stoppingDistance)
            {
                UpdateMovement(moveDir);
                //transform.Translate(movement);
            }

            else
            {
                _currentState = EntityState.CastingAbility;
            }
        }
        else _currentState = EntityState.MovingWithoutTarget;
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
    private enum EntityState
    {
        MovingTowardsTarget,
        MovingWithoutTarget,
        CastingAbility
    }
}


