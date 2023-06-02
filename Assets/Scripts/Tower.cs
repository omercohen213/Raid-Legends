using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : Entity
{
    private float _damage;
    private readonly float _initialDamageDelay = 0.5f;
    private readonly float _damageRampRate = 1.1f;
    private readonly float _damageRate = 0.3f;

    //private TowerRange range;
    [SerializeField] private Transform _focusStartingPoint;
    [SerializeField] private LineRenderer _focusLine;
    [SerializeField] private SpriteRenderer _rangeSpriteRenderer;
    [SerializeField] private Sprite _towerRangeSafe;
    [SerializeField] private Sprite _towerRangeDanger;

    protected override void Start()
    {
        base.Start();
        _focusLine.SetPosition(0, _focusStartingPoint.position);
        _targetingPriority = new List<Type> { Type.Minion, Type.Player, Type.AIPlayer };
    }

    private void Update()
    {
        if (_targetedEntity != null)
        {
            _focusLine.SetPosition(1, _targetedEntity.transform.position);
            if (_entitiesInTargetRange.Contains(_player))
                ShowRange();
        }
    }

    public override void TargetEnemy(Entity entity)
    {
        base.TargetEnemy(entity);
        Attack();
    }

    public override void StopTargetEnemy()
    {
        base.StopTargetEnemy();
        StopAttack();
    }

    public override void Attack()
    {
        _focusLine.enabled = true;
        ResetDamage();
        InvokeRepeating(nameof(DealRampingDamage), _initialDamageDelay, _damageRate);

    }

    private void StopAttack()
    {
        _focusLine.enabled = false;
        _rangeSpriteRenderer.enabled = false;
        CancelInvoke(nameof(DealRampingDamage));
        ResetDamage();
    }

    public void ResetDamage()
    {
        _damage = _baseDamage;
    }

    private void DealRampingDamage()
    {
        _damage *= _damageRampRate;
        _targetedEntity.ReceiveDamage((int)_damage, false, false);
    }

    private void ShowRange()
    {
        _rangeSpriteRenderer.enabled = true;
        Sprite sprite;
        if (_targetedEntity is Player)
        {
            sprite = _towerRangeDanger;
        }
        else sprite = _towerRangeSafe;
        _rangeSpriteRenderer.sprite = sprite;
    }

    public override Entity FindPriotityTarget(List<Entity> entities)
    {
        foreach (Type entityType in _targetingPriority)
        {
            Entity foundEntity = _entitiesInTargetRange.Find(entity => entity.EntityType == entityType);

            foreach (Entity entity in _entitiesInTargetRange)
            {
                if (entity.EntityType == entityType)
                {
                    TargetEnemy(entity);
                    return entity;
                }
            }
        }
        return null;
    }

}

