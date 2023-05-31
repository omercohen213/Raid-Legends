using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class Entity : MonoBehaviour, IDamageable
{
    protected Player _player;
    private SpriteRenderer _entitySpriteRenderer;

    // Team and type
    [SerializeField] private Team _team;
    [SerializeField] private Type _type;

    // Stats
    protected int _hp;
    [SerializeField] protected int _maxHp;
    [SerializeField] protected int _baseDamage;
    protected int _level;

    // Movement
    protected Vector3 _moveDir;
    [SerializeField] protected float _speed;
    [SerializeField] Transform _hpBar;

    // Targeting
    protected Entity _targetedEntity;
    protected List<Entity> _entitiesInRange;
    protected List<Type> _targetingPriority;

    // Damage
    protected float _critChance;
    //protected float lastDamage;
    //protected float damageDelay;

    public enum Team { Red, Blue }
    public enum Type { Minion, Player, Tower, AIPlayer }
    public Team EntityTeam { get => _team; set => _team = value; }
    public Type EntityType { get => _type; set => _type = value; }
    public int Hp { get => _hp; set => _hp = value; }
    public int MaxHp { get => _maxHp; set => _maxHp = value; }
    public int BaseDamage { get => _baseDamage; set => _baseDamage = value; }
    public Entity TargetedEntity { get => _targetedEntity; set => _targetedEntity = value; }
    public List<Entity> EntitiesInRange { get => _entitiesInRange; set => _entitiesInRange = value; }
    public List<Type> TargetingPriority { get => _targetingPriority; set => _targetingPriority = value; }

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
        _hp = _maxHp;
        _targetedEntity = null;
        _entitiesInRange = new List<Entity>();
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

        Vector2 targetVelocity = new(moveDir.x * _speed, moveDir.y * _speed);
        rb.velocity = targetVelocity;

        // Check if the moveDir is zero, indicating no input
        if (moveDir == Vector3.zero)
        {
            rb.velocity = Vector2.zero;
        }
    }

    protected virtual void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.transform.TryGetComponent(out Entity collEntity))
        {
            if (IsAgainst(collEntity))
            {
                bool isCritical = UnityEngine.Random.value < collEntity._critChance;
                ReceiveDamage(collEntity.BaseDamage, isCritical);
            }
        }
    }

    public bool IsAgainst(Entity other)
    {
        return other._team != _team;
    }

    public virtual void ReceiveDamage(int damageAmount, bool isCritical)
    {
        if (isCritical)
        {
            damageAmount *= 2;
            Debug.Log("crit");
        }
        _hp -= damageAmount;
        if (_hp <= 0)
        {
            _hp = 0;
            Death();
        }
        float spriteHeight = _entitySpriteRenderer.bounds.size.y;
        Vector3 damagePopUpPosition = transform.position + new Vector3(0, spriteHeight / 2);

        if (IsPlayerInRange())
            PopUpSpawner.Instance.ShowDamagePopUp(damageAmount.ToString(), damagePopUpPosition, isCritical);

        OnHpChange();

        /*if (Time.time - lastDamage > damageDelay)
        {
            lastDamage = Time.time;
            if (damageAmount > 0)
            {
                hp -= damageAmount;
                //FloatingTextManager.instance.ShowFloatingText(damageAmount.ToString(), 30, new Color(0.98f, 0.37f, 0), origin, "Hit", 2.0f);
            }
            //else FloatingTextManager.instance.ShowFloatingText("0", 30, new Color(0.98f, 0.37f, 0), origin, "Hit", 2.0f);
        */
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

    public virtual void FindNewTarget(List<Type> targetingPriority)
    {
        foreach (Type entityType in targetingPriority)
        {
            foreach (Entity entity in _entitiesInRange)
            {
                if (entity._type == entityType)
                {
                    TargetEnemy(entity);
                    return;
                }
            }
        }
    }

    public virtual void TargetEnemy(Entity entity)
    {
        _targetedEntity = entity;
    }

    public virtual void StopTargetEnemy()
    {
        _targetedEntity = null;
    }


    public virtual void Attack(Entity entity)
    {
        Debug.Log("Attack");
    }

    public virtual void StopAttack()
    {
        Debug.Log("Stopping Attack");
    }

    public virtual void Death()
    {
        UIManager.Instance.HideUIEntityStats();
    }

    public virtual void ResetHp()
    {
        _hp = _maxHp;
        OnHpChange();
    }

    protected virtual bool IsPlayerInRange()
    {
        return _entitiesInRange.Contains(_player);

    }
}
