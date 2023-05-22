using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : Entity
{
    private float damage;
    private float damageRampRate = 1.2f;

    private TowerRange range;
    private Entity targetedEntity;


    protected override void Awake()
    {
        base.Awake();
        range = GetComponentInChildren<TowerRange>();
    }

    protected override void Start()
    {
        base.Start();
    }

    public override void Attack(Entity entity)
    {
        targetedEntity = entity;
        InvokeRepeating("DealRampingDamage", 0.5f, 0.5f);
    }

    public void StopAttack()
    {
        CancelInvoke("DealRampingDamage");
        ResetDamage();
    }

    public void ResetDamage()
    {
        damage = baseDamage;
    }

    private void DealRampingDamage()
    {
        damage = BaseDamage;
        damage += damage * damageRampRate;
        targetedEntity.ReceiveDamage((int)damage);
    }



}

