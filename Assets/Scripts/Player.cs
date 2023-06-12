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
    [SerializeField] private CircleCollider2D _targetRange;
    [SerializeField] private LineRenderer _rangeLineRenderer;
    private bool _isShowingRange;
    private Vector3 _rangeOffset;
    //[SerializeField] private SpriteRenderer _rangeSpriteRenderer;
    private bool _movingTowardsTarget = false;
    private Ability _currentAbilty;
    private Entity _lastTargetedEntity;

    public Animator Anim { get => _anim; set => _anim = value; }
    public CircleCollider2D AttackRange { get => _attackRange; set => _attackRange = value; }

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

        if (_isShowingRange)
        {
            _rangeOffset = new Vector3(0.17f, -0.34f);
            DrawRange();
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
    public void TryUseAbility(Ability ability, Vector3 abilityPosition)
    {
        _currentAbilty = ability;
        if (!ability.isTargetNeeded)
        {
            AbilityManager.Instance.UseAbility(_currentAbilty, abilityPosition);
        }

        else
        {
            if (_targetedEntity != null)
            {
                _attackRange.radius = _currentAbilty.range;
                if (EntitiesInAttackRange.Contains(_targetedEntity))
                {
                    AbilityManager.Instance.UseAbility(_currentAbilty, _targetedEntity.transform.position);
                }
                else
                {
                    _movingTowardsTarget = true;
                }
            }
            else
            {
                Entity priorityTarget;
                if (_attackRange.radius > _targetRange.radius)
                {
                    priorityTarget = FindPriotityTarget(_entitiesInAttackRange);
                }
                else
                {
                    priorityTarget = FindPriotityTarget(_entitiesInTargetRange);
                }

                if (priorityTarget != null)
                {
                    _currentAbilty = ability;
                    _attackRange.radius = _currentAbilty.range;
                    _targetedEntity = priorityTarget;
                    if (EntitiesInAttackRange.Contains(_targetedEntity))
                    {
                        AbilityManager.Instance.UseAbility(_currentAbilty, abilityPosition);
                    }
                    else
                    {
                        _movingTowardsTarget = true;
                    }
                }
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

        // Reached range
        else
        {
            _movingTowardsTarget = false;
            AbilityManager.Instance.UseAbility(_currentAbilty, _targetedEntity.transform.position);
        }
    }

    public void DrawRange()
    {
        int segments = 50;
        _rangeLineRenderer.positionCount = segments + 2;
        _rangeLineRenderer.widthMultiplier = 0.1f;
        _rangeLineRenderer.startColor = Color.gray;
        _rangeLineRenderer.endColor = Color.gray;
        Material defaultMaterial = new(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
        _rangeLineRenderer.material = defaultMaterial;
        _rangeLineRenderer.sortingLayerName = "HUD";
        _rangeLineRenderer.sortingOrder = 0;

        float angle = 0f;
        float angleStep = 360f / segments;

        for (int i = 0; i <= segments + 1; i++)
        {
            float x = Mathf.Sin(Mathf.Deg2Rad * angle) * _attackRange.radius;
            float y = Mathf.Cos(Mathf.Deg2Rad * angle) * _attackRange.radius;

            Vector3 point = new Vector3(x, y, 0f) + transform.position + _rangeOffset;
            _rangeLineRenderer.SetPosition(i, point);

            angle += angleStep;
        }
    }

    public void UpdateAttackRange(float radius)
    {
        _attackRange.radius = radius;
    }

    // Show range
    public void ShowPlayerRange()
    {
        _isShowingRange = true;
        _rangeLineRenderer.enabled = true;
    }

    // Hide range
    public void HidePlayerRange()
    {
        _isShowingRange = false;
        _rangeLineRenderer.enabled = false;
    }

    // Show range for duration time and then hide
    public void ShowPlayerRange(float duration)
    {
        _isShowingRange = true;
        _rangeLineRenderer.enabled = true;
        StartCoroutine(HidePlayerRange(duration));
    }
    private IEnumerator HidePlayerRange(float duration)
    {
        yield return new WaitForSeconds(duration);
        _isShowingRange = false;
        _rangeLineRenderer.enabled = false;
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
