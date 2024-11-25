using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class Entity : MonoBehaviour, IDamageable
{
    protected Player _player;
    private SpriteRenderer _entitySpriteRenderer;

    // Team and type
    [SerializeField] protected Team _team;
    [SerializeField] protected Type _type;

    // Stats
    protected int _hp;
    protected int _mp;
    [SerializeField] protected int _maxHp;
    [SerializeField] protected int _maxMp;
    [SerializeField] protected int _baseDamage;
    protected int _level;
    [SerializeField] protected int _xpAmountToGive;
    [SerializeField] protected int _goldAmountToGive;

    // Movement
    protected Vector3 _moveDir;
    [SerializeField] protected float _movementSpeed;
    [SerializeField] Transform _hpBar;
    [SerializeField] protected Transform _bars;

    // Targeting
    protected Entity _targetedEntity;
    protected List<Entity> _entitiesInTargetRange; // To determine if enemy entity can be targeted
    protected List<Entity> _entitiesInAttackRange; // To determine if entity can attack enemy entity in target range
    protected List<Type> _targetingPriority; // Ordered list of targeting pririty
    [SerializeField] protected CircleCollider2D _attackRange;
    [SerializeField] protected CircleCollider2D _targetRange;
    protected bool _movingTowardsTarget;

    // Damage
    protected float _critChance;

    public enum Team { Red, Blue }
    public enum Type { Minion, Player, Tower, AIPlayer }
    public Team EntityTeam { get => _team; set => _team = value; }
    public Type EntityType { get => _type; set => _type = value; }
    public int Hp { get => _hp; set => _hp = value; }
    public int MaxHp { get => _maxHp; set => _maxHp = value; }
    public int BaseDamage { get => _baseDamage; set => _baseDamage = value; }
    public Entity TargetedEntity { get => _targetedEntity; set => _targetedEntity = value; }
    public List<Entity> EntitiesInTargetRange { get => _entitiesInTargetRange; set => _entitiesInTargetRange = value; }
    public List<Entity> EntitiesInAttackRange { get => _entitiesInAttackRange; set => _entitiesInAttackRange = value; }
    public CircleCollider2D AttackRange { get => _attackRange; set => _attackRange = value; }
    public float CritChance { get => _critChance; }

    protected virtual void Awake()
    {
        if (!GameObject.Find("Player").TryGetComponent(out _player))
        {
            Debug.LogError("Missing Player object");
        }

        if (!transform.Find("Sprite").TryGetComponent(out _entitySpriteRenderer))
        {
            Debug.LogError("Missing Entity SpriteRenderer");
        }
    }

    protected virtual void Start()
    {
        _level = 1;
        _hp = _maxHp;
        _mp = _maxMp;
        _targetedEntity = null;
        _entitiesInTargetRange = new List<Entity>();
        _entitiesInAttackRange = new List<Entity>();
    }

    // Checks collision and updates movement
    protected virtual void UpdateMovement(Vector3 moveDir)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        Vector2 velocity = new(moveDir.x * _movementSpeed, moveDir.y * _movementSpeed);
        rb.velocity = velocity;

        // Check if the moveDir is zero, indicating no input
        if (moveDir == Vector3.zero)
        {
            rb.velocity = Vector2.zero;
        }
    }

    protected virtual void ChangeBarsScale()
    {
        Vector3 currentEntityScale = transform.localScale;
        Vector3 currentBarsScale = _bars.transform.localScale;

        // Going towards target when trying to attack
        if (_targetedEntity != null)
        {
            Vector3 targetedEntityPos = _targetedEntity.transform.position;
            if (targetedEntityPos.x < transform.position.x)
            {
                currentEntityScale.x = -1f;
                currentBarsScale.x = -1f;
            }
            else
            {
                currentEntityScale.x = 1f;
                currentBarsScale.x = 1f;
            }
            transform.localScale = currentEntityScale;
            _bars.localScale = currentBarsScale;

        }
    }

    public bool IsAgainst(Entity other)
    {
        return other._team != _team;
    }

    public virtual void ReceiveDamage(int damageAmount, bool isCritical, Entity attacker)
    {
        if (isCritical)
            damageAmount *= 2;

        _hp -= damageAmount;
        if (_hp <= 0)
        {
            _hp = 0;
            Death(attacker);
        }
        float spriteHeight = _entitySpriteRenderer.bounds.size.y;
        Vector3 damagePopUpPosition = transform.position + new Vector3(0, spriteHeight / 2);

        // Show damage popups only for player
        if (this is Player || attacker is Player)
            PopUpSpawner.Instance.ShowDamagePopUp(damageAmount.ToString(), damagePopUpPosition, isCritical);

        OnHpChange();
    }
    public void OnHpChange()
    {
        float percentage = (float)_hp / _maxHp;
        Vector3 newScale = _hpBar.localScale;
        if (percentage > 0)
            newScale.x = percentage;
        else newScale.x = 0;
        _hpBar.localScale = newScale;
        if (_player._targetedEntity != null)
        {
            if (_player._targetedEntity == this)
            {
                UIManager.Instance.ShowUIEntityStats(gameObject);
            }
        }
    }

    public virtual void Attack()
    {
        Debug.Log("Attack");
    }

    public virtual void TargetEnemy(Entity entity)
    {
        _targetedEntity = entity;
    }

    public virtual void StopTargetEnemy()
    {
        _targetedEntity = null;
    }

    // Try to find a target in range. Prioritize targets according to targeting priority
    public virtual Entity FindPriotityTarget(List<Entity> entitiesInRange)
    {
        foreach (Type entityType in _targetingPriority)
        {
            Entity closestEntity = FindClosestEntity(entitiesInRange, entityType);
            if (closestEntity != null)
            {
                if (entitiesInRange.Contains(closestEntity))
                {
                    TargetEnemy(closestEntity);
                    return closestEntity;
                }
            }
        }
        return null;
    }

    // Try to find closest entity in range
    protected Entity FindClosestEntity(List<Entity> entitiesInRange, Type entityType)
    {
        Entity closestEntity = null;
        float closestDistance = Mathf.Infinity;

        foreach (Entity entity in entitiesInRange)
        {
            if (entity._type == entityType)
            {
                float distance = Vector3.Distance(entity.transform.position, transform.position);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEntity = entity;
                }
            }
        }
        return closestEntity;
    }

    public virtual void Death(Entity attacker)
    {
        UIManager.Instance.HideUIEntityStats();
        if (attacker is Player)
        {
            Player playerAttacker = attacker.GetComponent<Player>();
            playerAttacker.GainXp(_xpAmountToGive);
            playerAttacker.GainGold(_goldAmountToGive);

        }
    }

    public virtual void OnLevelUp()
    {
        _level++;
        _baseDamage += 10;
        _maxHp += 100;
    }

    public virtual void ResetHp()
    {
        _hp = _maxHp;
        OnHpChange();
    }
}
