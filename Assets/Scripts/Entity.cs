using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class Entity : MonoBehaviour, IDamageable
{
    public enum Team { Red, Blue }
    [SerializeField] Transform hpBar;
    [SerializeField] Vector3 targetPos;
    [SerializeField] private Team team;
    protected int hp;
    protected int level;
    [SerializeField] protected int maxHp;
    [SerializeField] protected int baseDamage;
    protected float lastDamage;
    protected float damageDelay;
    protected Transform target;
    public Team EntityTeam { get => team; set => team = value; }
    protected int Hp { get => hp; set => hp = value; }
    protected int MaxHp { get => maxHp; set => maxHp = value; }
    public int BaseDamage { get => baseDamage; set => baseDamage = value; }


    [SerializeField] protected string[] blockingLayers;
    [SerializeField] protected float speed;
    protected Vector3 moveDir;

    private Collider2D coll;


    protected virtual void Awake()
    {
        coll = GetComponent<Collider2D>();
    }

    protected virtual void Start()
    {
        hp = maxHp;
    }

    // Checks collision and updates movement
    protected virtual void UpdateMovement(Vector3 moveDir)
    {
        Vector3 size = coll.bounds.extents;
        float rayLengthX = size.x;
        float rayLengthY = size.y;
        Debug.DrawRay(transform.position, new Vector2(moveDir.x, 0) * rayLengthX, Color.red);
        Debug.DrawRay(transform.position, new Vector2(0, moveDir.y) * rayLengthY, Color.blue);


        RaycastHit2D hitX = Physics2D.Raycast(transform.position, new Vector2(moveDir.x, 0), rayLengthX, LayerMask.GetMask(blockingLayers));
        if (hitX.collider == null)
        {
            transform.Translate(new Vector2(Time.deltaTime * moveDir.x, 0), Space.World);
        }

        RaycastHit2D hitY = Physics2D.Raycast(transform.position, new Vector2(0, moveDir.y), rayLengthY, LayerMask.GetMask(blockingLayers));
        if (hitY.collider == null)
        {
            transform.Translate(new Vector2(0, Time.deltaTime * moveDir.y), Space.World);
        }
    } 
     
    private void OnCollisionEnter2D(Collision2D coll)
    {
        Entity collEntity;
        coll.transform.TryGetComponent(out collEntity);
        if (IsAgainst(collEntity))
        {
            ReceiveDamage(collEntity.BaseDamage);
            Debug.Log(this + " Damage Taken " + collEntity.BaseDamage);
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
    }

    public virtual void Attack(Entity entity)
    {
        Debug.Log("Attack");
    }

    public virtual void Death()
    {
        Debug.Log("Dead");
    }

   
}
