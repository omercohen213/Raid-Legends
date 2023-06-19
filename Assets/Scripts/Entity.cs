using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
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
    [SerializeField] protected int _maxHp;
    [SerializeField] protected int _baseDamage;
    protected int _level;
    [SerializeField] protected int _xpAmountToGive;
    [SerializeField] protected int _goldAmountToGive;

    // Movement
    protected Vector3 _moveDir;
    [SerializeField] protected float _movementSpeed;
    [SerializeField] Transform _hpBar;

    // Targeting
    protected Entity _targetedEntity;
    protected List<Entity> _entitiesInTargetRange; // To determine if enemy entity can be targeted
    protected List<Entity> _entitiesInAttackRange; // To determine if entity can attack enemy entity in target range
    protected List<Type> _targetingPriority; // Ordered list of targeting pririty

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
        _targetedEntity = null;
        _entitiesInTargetRange = new List<Entity>();
        _entitiesInAttackRange = new List<Entity>();
    }

    // Checks collision and updates movement
    protected virtual void UpdateMovement(Vector3 moveDir)
    {
        /*Rigidbody2D rb = GetComponent<Rigidbody2D>();
        Vector2 targetVelocity = new Vector2(moveDir.x * speed, moveDir.y * speed);
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, coll.bounds.size, 0f, targetVelocity.normalized, targetVelocity.magnitude * Time.deltaTime, LayerMask.GetMask("Obstacle", "Moving Entity"));
        if (hit.collider == null)
        {
            // No collision, apply the target velocity to the Rigidbody
            rb.velocity = targetVelocity;
        }
        else
        {
            // Collision detected, calculate the slide direction
            Vector2 slideDirection = Vector2.zero;

            // Check if the hit normal is valid for sliding
            if (Vector2.Dot(hit.normal, targetVelocity.normalized) < 0.5f)
            {
                // Determine the slide direction perpendicular to the hit normal
                slideDirection = Vector2.Perpendicular(hit.normal).normalized;
            }

            // Calculate the slide velocity based on the slide direction and target velocity magnitude
            Vector2 slideVelocity = slideDirection * targetVelocity.magnitude;

            // Apply the slide velocity to the Rigidbody
            rb.velocity = slideVelocity;
        }   */
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        Vector2 velocity = new(moveDir.x * _movementSpeed, moveDir.y * _movementSpeed);
        rb.velocity = velocity;

        // Check if the moveDir is zero, indicating no input
        if (moveDir == Vector3.zero)
        {
            rb.velocity = Vector2.zero;
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
        UIManager.Instance.UpdateUIEntityStats();
    }

    public virtual void ResetHp()
    {
        _hp = _maxHp;
        OnHpChange();
    }
}
