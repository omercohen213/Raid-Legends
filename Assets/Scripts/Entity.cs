using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour, IDamageable
{
    public enum Team { Red, Blue }
    [SerializeField] private Team team;
    public Team EntityTeam { get => team; set => team = value; }

    private int hp;
    protected int Hp { get => hp; set => hp = value; }

    [SerializeField] private int maxHp;
    protected int MaxHp { get => maxHp; set => maxHp = value; }

    [SerializeField] private int baseDamage;
    protected int BaseDamage { get => baseDamage; set => baseDamage = value; }
    protected float lastDamage;
    protected float damageDelay;
    

    public bool IsAgainst (Entity other)
    {
        return other.team != team;
    }

    public virtual void ReceiveDamage(int damageAmount)
    {
        if (Time.time - lastDamage > damageDelay)
        {
            lastDamage = Time.time;
            if (damageAmount > 0)
            {
                hp -= damageAmount;
                //FloatingTextManager.instance.ShowFloatingText(damageAmount.ToString(), 30, new Color(0.98f, 0.37f, 0), origin, "Hit", 2.0f);
            }
            //else FloatingTextManager.instance.ShowFloatingText("0", 30, new Color(0.98f, 0.37f, 0), origin, "Hit", 2.0f);

            if (hp <= 0)
            {
                hp = 0;
                Death();
            }
        }
        //OnHpChange();
    }

    public virtual void Death()
    {
        Debug.Log("Dead");
    }
}
