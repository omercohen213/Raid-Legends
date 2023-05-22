using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerRange : AttackRange
{
    private Tower tower;
    [SerializeField] private Transform focusStartingPoint;
    [SerializeField] private LineRenderer focusLine;
    [SerializeField] private SpriteRenderer rangeSpriteRenderer;
    [SerializeField] private Sprite towerRangeSafe;
    [SerializeField] private Sprite towerRangeDanger;

    private void Awake()
    {
        if (!TryGetComponent(out rangeSpriteRenderer))
        {
            Debug.LogError("RangeSpriteRenderer component not found!");
        }
        tower = GetComponentInParent<Tower>();
        if (tower == null)
        {
            Debug.LogError("Tower component not found!");
        }
    }

    protected override void Start()
    {
        base.Start();
        focusLine.SetPosition(0, focusStartingPoint.position);
    }

    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.CompareTag("Minion") || coll.CompareTag("Player"))
        {
            Entity collEntity = coll.GetComponent<Entity>();
            if (tower.IsAgainst(collEntity))
            {
                if (coll.CompareTag("Player"))
                {
                    Player player = coll.GetComponent<Player>();
                    playersInRange.Add(player);
                }
                else if (coll.CompareTag("Minion"))
                {
                    Minion minion = collEntity.GetComponent<Minion>();
                    minionsInRange.Add(minion);
                }
                else Debug.LogError("Unrecognized entity", collEntity);

                // If there is no currently focused entity, focus on the first entity in range
                if (targetedEntity == null)
                {
                    Target(collEntity);
                    Debug.Log(targetedEntity);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D coll)
    {
        if (coll.CompareTag("Minion") || coll.CompareTag("Player"))
        {
            if (coll.CompareTag("Player"))
            {
                Player player = coll.GetComponent<Player>();
                playersInRange.Remove(player);
            }
            else if (coll.CompareTag("Minion"))
            {
                Minion minion = coll.GetComponent<Minion>();
                minionsInRange.Remove(minion);
            }

            if (targetedEntity != null)
            {
                // If the focused player leaves the range, switch focus to the next entity 
                if (coll.transform == targetedEntity.transform)
                {
                    if (minionsInRange.Count > 0)
                    {
                        Target(minionsInRange[0]);
                    }
                    else if (playersInRange.Count > 0)
                    {
                        Target(playersInRange[0]);
                    }
                    else
                    {
                        StopTargeting();
                    }
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D coll)
    {
        if (targetedEntity != null)
        {
            focusLine.SetPosition(1, targetedEntity.transform.position);
        }
        ShowRange();
    }

    private void Target(Entity entity)
    {
        focusLine.enabled = true;
        targetedEntity = entity;
        tower.ResetDamage();
        tower.Attack(entity);
    }

    private void StopTargeting()
    {
        focusLine.enabled = false;
        rangeSpriteRenderer.enabled = false;
        targetedEntity = null;
        tower.StopAttack();
    }

    private void ShowRange()
    {
        if (playersInRange.Count > 0)
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
        else rangeSpriteRenderer.enabled = false;
    }

}
