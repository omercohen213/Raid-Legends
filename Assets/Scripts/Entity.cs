using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class Entity : MonoBehaviour, IDamageable
{
    protected Player player;

    // Team and type
    [SerializeField] private Team team;
    [SerializeField] private Type type;

    // Stats
    protected int hp;
    [SerializeField] protected int maxHp;
    [SerializeField] protected int baseDamage;
    protected int level;

    // Movement
    private Collider2D coll;
    protected Vector3 moveDir;
    [SerializeField] protected float speed;
    [SerializeField] Transform hpBar;

    // Targeting
    protected Entity targetedEntity;
    protected List<Entity> entitiesInRange;
    protected List<Type> targetingPriority;

    //protected float lastDamage;
    //protected float damageDelay;

    public enum Team { Red, Blue }
    public enum Type { Minion, Player, Tower, AIPlayer }
    public Team EntityTeam { get => team; set => team = value; }
    public Type EntityType { get => type; set => type = value; }
    public int Hp { get => hp; set => hp = value; }
    public int MaxHp { get => maxHp; set => maxHp = value; }
    public int BaseDamage { get => baseDamage; set => baseDamage = value; }
    public Entity TargetedEntity { get => targetedEntity; set => targetedEntity = value; }
    public List<Entity> EntitiesInRange { get => entitiesInRange; set => entitiesInRange = value; }
    public List<Type> TargetingPriority { get => targetingPriority; set => targetingPriority = value; }

    protected virtual void Awake()
    {
        coll = GetComponent<Collider2D>();
        player = GameObject.Find("Player").GetComponent<Player>();
    }

    protected virtual void Start()
    {
        hp = maxHp;
        targetedEntity = null;
        entitiesInRange = new List<Entity>();
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

        Vector2 targetVelocity = new(moveDir.x * speed, moveDir.y * speed);
        rb.velocity = targetVelocity;

        // Check if the moveDir is zero, indicating no input
        if (moveDir == Vector3.zero)
        {
            rb.velocity = Vector2.zero;
        }
    }

    private void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.transform.TryGetComponent(out Entity collEntity))
        {
            if (IsAgainst(collEntity))
            {
                ReceiveDamage(collEntity.BaseDamage);
                //Debug.Log(this + " Damage Taken " + collEntity.BaseDamage);
            }
        }
    }

    public bool IsAgainst(Entity other)
    {
        return other.team != team;
    }

    public virtual void ReceiveDamage(int damageAmount)
    {
        hp -= damageAmount;
        if (hp <= 0)
        {
            hp = 0;
            Death();
        }
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
        float percentage = (float)hp / maxHp;
        Vector3 newScale = hpBar.localScale;
        if (percentage > 0)
            newScale.x = percentage;
        else newScale.x = 0;
        hpBar.localScale = newScale;
        if (player.targetedEntity != null)
        {
            if (player.targetedEntity == this)
            {
                UIManager.Instance.ShowUIEntityStats(gameObject);
            }
        }
    }

    public virtual void FindNewTarget(List<Type> targetingPriority)
    {
        foreach (Type entityType in targetingPriority)
        {
            foreach (Entity entity in entitiesInRange)
            {
                if (entity.type == entityType)
                {
                    TargetEnemy(entity);
                    return;
                }
            }
        }
    }

    public virtual void TargetEnemy(Entity entity)
    {
        if (this is Player)
        {
            Transform target = entity.transform.Find("Target");
            if (target != null)
            {
                target.gameObject.SetActive(true);
            }
            else Debug.LogError("Missing target object");
        }
        targetedEntity = entity;
        UIManager.Instance.ShowUIEntityStats(entity.gameObject);
    }

    public virtual void StopTargetEnemy(Entity entity)
    {
        if (this is Player)
        {
            Transform target = entity.transform.Find("Target");
            if (target != null)
            {
                target.gameObject.SetActive(false);
            }
            else Debug.LogError("Missing target object");
        }
        targetedEntity = null;
        UIManager.Instance.HideUIEntityStats();
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
        Debug.Log("Dead");
    }

    public virtual void ResetHp()
    {
        hp = maxHp;
        OnHpChange();
    }

}
