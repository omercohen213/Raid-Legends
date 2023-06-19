using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : Entity
{
    [SerializeField] private Animator _anim;

    // Player Movement
    [SerializeField] private Joystick _joystick;
    [SerializeField] private Transform _bars;
    private readonly float _barsScale = 0.35f;
    private readonly float _joystickMinInput = 0.25f;

    // Stats
    protected int _xp;
    protected int _gold;
    protected readonly int _startingGold = 500;

    [SerializeField] private GameObject _levelUpPrefab;
    [SerializeField] private TextMeshProUGUI _levelText;

    // Abilities
    [SerializeField] private CircleCollider2D _attackRange;
    [SerializeField] private CircleCollider2D _targetRange;
    [SerializeField] private LineRenderer _rangeLineRenderer;
    private bool _isShowingRange;
    private bool _movingTowardsTarget = false;
    private Ability _currentAbilty;
    private Entity _lastTargetedEntity;

    public CircleCollider2D AttackRange { get => _attackRange; set => _attackRange = value; }
    public int StartingGold { get => _startingGold; }
    public int Gold { get => _gold; set => _gold = value; }

    protected override void Start()
    {
        base.Start();
        _xp = 0;
        _gold = _startingGold;
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
    // If there is no targeted entity, try to find one in range
    public void TryUseAbility(Ability ability)
    {
        _currentAbilty = ability;

        if (_targetedEntity != null)
        {
            _attackRange.radius = _currentAbilty.Range;
            if (EntitiesInAttackRange.Contains(_targetedEntity))
            {
                ability.UseAbility(_targetedEntity.transform.position);
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
                _attackRange.radius = _currentAbilty.Range;
                _targetedEntity = priorityTarget;
                if (EntitiesInAttackRange.Contains(_targetedEntity))
                {
                    ability.UseAbility(_targetedEntity.transform.position);
                }
                else
                {
                    _movingTowardsTarget = true;
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
            _currentAbilty.UseAbility(_targetedEntity.transform.position);
        }
    }

    public void DrawRange()
    {
        Vector3 rangeOffset = new (0.17f, -0.34f);
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

            Vector3 point = new Vector3(x, y, 0f) + transform.position + rangeOffset;
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

    public void GainXp(int xpGain)
    {
        _xp += xpGain;
        int _xpToLevelUp = GameManager.Instance.GetXpToLevelUp(_level);
        if (_xp > _xpToLevelUp)
        {
            _xp = 0;
            OnLevelUp();
        }
    }
    public void GainGold(int goldGain)
    {
        _gold += goldGain;
        UIManager.Instance.UpdateGoldUI();
    }

    public override void OnLevelUp()
    {
        base.OnLevelUp();
        // check if changes animation scale when player turns
        _levelText.text = _level.ToString();
        GameObject levelUpObject = Instantiate(_levelUpPrefab, transform.position, Quaternion.identity, GameObject.Find("Player").transform);
        UIManager.Instance.ShowLevelUpAbility();
        StartCoroutine(LevelUpOff(levelUpObject));
    }

    private IEnumerator LevelUpOff(GameObject levelUpObject)
    {
        yield return new WaitForSeconds(1.5f);
        Destroy(levelUpObject);
    }


    public void BasicAttack()
    {
        _anim.SetBool("Attack", true);

    }
    public void BasicAttackOff()
    {
        _anim.SetBool("Attack", false);
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
