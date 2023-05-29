using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : Entity
{
    private float damage;
    private float damageRampRate = 2.5f;

    //private TowerRange range;
    [SerializeField] private Transform focusStartingPoint;
    [SerializeField] private LineRenderer focusLine;
    [SerializeField] private SpriteRenderer rangeSpriteRenderer;
    [SerializeField] private Sprite towerRangeSafe;
    [SerializeField] private Sprite towerRangeDanger;

    protected override void Start()
    {
        base.Start();
        focusLine.SetPosition(0, focusStartingPoint.position);
        targetingPriority = new List<Type> { Type.Player, Type.AIPlayer, Type.Minion };
    }

    private void Update()
    {
        if (targetedEntity != null)
        {
            focusLine.SetPosition(1, targetedEntity.transform.position);
            if (entitiesInRange.Contains(player))
                ShowRange();
        }
    }

    public override void TargetEnemy(Entity entity)
    {
        base.TargetEnemy(entity);
        Attack(entity);
    }

    public override void StopTargetEnemy(Entity entity)
    {
        base.StopTargetEnemy(entity);
        StopAttack();
    }

    public override void Attack(Entity entity)
    {
        focusLine.enabled = true;
        ResetDamage();
        InvokeRepeating(nameof(DealRampingDamage), 0.5f, 0.5f);
    }

    public override void StopAttack()
    {
        focusLine.enabled = false;
        rangeSpriteRenderer.enabled = false;
        CancelInvoke(nameof(DealRampingDamage));
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

    private void ShowRange()
    {
        rangeSpriteRenderer.enabled = true;
        Sprite sprite;
        if (targetedEntity is Player)
        {
            sprite = towerRangeDanger;
        }
        else sprite = towerRangeSafe;
        rangeSpriteRenderer.sprite = sprite;
    }

}

