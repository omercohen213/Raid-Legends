using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : Entity
{
    // Player Movement
    [SerializeField] private Joystick _joystick;
    [SerializeField] private Transform _bars;
    private readonly float _barsScale = 0.35f;
    private readonly float _joystickMinInput = 0.25f;

    // Abilities
    [SerializeField] private Animator _anim;
    [SerializeField] private CircleCollider2D _attackRange;
    private bool _movingTowardsTarget = false;
    private Ability _currentAbilty;
    private Entity _lastTargetedEntity;
    public Animator Anim { get => _anim; set => _anim = value; }

    protected override void Start()
    {
        base.Start();
        _movementSpeed = 3f;
        _targetingPriority = new List<Type> { Type.Player, Type.AIPlayer, Type.Tower, Type.Minion };
        _critChance = 0.2f;
    }

    private void Update()
    {
        float horizontalInput = _joystick.Horizontal;
        float verticalInput = _joystick.Vertical;

        Vector3 currentPlayerScale = transform.localScale;
        Vector3 currentBarsScale = _bars.transform.localScale;
        // Move only if joystick input exceeds the minimum input threshold
        if (Mathf.Abs(horizontalInput) > _joystickMinInput || Mathf.Abs(verticalInput) > _joystickMinInput)
        {
            _movingTowardsTarget = false;

            // Negative scales to match sprite movement

            if (horizontalInput > _joystickMinInput)
            {
                currentPlayerScale.x = 1f;
                currentBarsScale.x = _barsScale;
            }
            else if (horizontalInput < -_joystickMinInput)
            {
                currentPlayerScale.x = -1f;
                currentBarsScale.x = -_barsScale;
            }
            transform.localScale = currentPlayerScale;
            _bars.localScale = currentBarsScale;

            _moveDir = new Vector3(horizontalInput, verticalInput, 0);
            Walk();
        }

        else
        {
            _moveDir = Vector3.zero;
            WalkOff();
        }
        base.UpdateMovement(_moveDir);

        // Going towards target when trying to attack
        if (_movingTowardsTarget)
        {
            Vector3 targetedEntityPos = _targetedEntity.transform.position;
            if (targetedEntityPos.x < transform.position.x)
            {
                currentPlayerScale.x = -1f;
                currentBarsScale.x = -_barsScale;
            }
            else
            {
                currentPlayerScale.x = 1f;
                currentBarsScale.x = _barsScale;
            }
            transform.localScale = currentPlayerScale;
            _bars.localScale = currentBarsScale;

            MoveTowardsTarget();
            Walk();
        }
    }

    // Target given entity and show target sprite
    public override void TargetEnemy(Entity entity)
    {
        StopTargetEnemy();

        base.TargetEnemy(entity);
        _lastTargetedEntity = entity;
        Transform target = entity.transform.Find("Target");
        if (target != null)
        {
            target.gameObject.SetActive(true);
        }
        else Debug.LogError("Missing target object");

        UIManager.Instance.ShowUIEntityStats(entity.gameObject);
    }

    // Stop targeting last targeted entity
    public override void StopTargetEnemy()
    {
        base.StopTargetEnemy();

        // Disable last targeted entity target sprite
        if (_lastTargetedEntity != null)
        {
            Transform lastTarget = _lastTargetedEntity.transform.Find("Target");
            if (lastTarget != null)
            {
                lastTarget.gameObject.SetActive(false);
            }
            else Debug.LogError("Missing target object");
        }
        UIManager.Instance.HideUIEntityStats();
    }

    // On ability use, start moving towards target.
    // If there is no targeted entity, try to find one in target range
    public void TryUseAbility(Ability ability)
    {
        if (_targetedEntity != null)
        {
            _movingTowardsTarget = true;
            _currentAbilty = ability;
            _attackRange.radius = _currentAbilty.range;
        }
        else
        {
            Entity priorityTarget = FindPriotityTarget(_entitiesInTargetRange);
            if (priorityTarget != null)
            {
                _targetedEntity = priorityTarget;
                _movingTowardsTarget = true;
                _currentAbilty = ability;
                _attackRange.radius = _currentAbilty.range;
            }
        }        
    }

    // On ability use, move towards targeted entity and then use the ability upon reaching the maximum range
    private void MoveTowardsTarget()
    {
        Vector3 targetPos = _targetedEntity.transform.position;
        Vector3 playerPos = transform.position;
        Vector3 moveDir = (targetPos - playerPos).normalized;
        Vector3 movement = _movementSpeed * Time.deltaTime * moveDir;

        float distanceToTarget = Vector3.Distance(playerPos, targetPos);
        float stoppingDistance = _attackRange.radius;

        // Go towards target as long as not in ablity range
        if (distanceToTarget > stoppingDistance)
        {
            transform.Translate(movement);
        }

        // Reached range, now check if targeted entity is inside ablity range
        else
        {
            /*if (_entitiesInAttackRange.Contains(_targetedEntity))
            {

            }
            else _movingTowardsTarget = true;*/
            AbilityManager.Instance.UseAbility(_currentAbilty);
            _movingTowardsTarget = false;
        }
    }

    public void Dead()
    {
        _anim.SetBool("Dead", true);
    }
    public void DeadOff()
    {
        _anim.SetBool("Dead", false);
    }
    public void Walk()
    {
        _anim.SetBool("Walk", true);
    }
    public void WalkOff()
    {
        _anim.SetBool("Walk", false);
    }
    public void Run()
    {
        _anim.SetBool("Run", true);
    }
    public void RunOff()
    {
        _anim.SetBool("Run", false);
    }


}
