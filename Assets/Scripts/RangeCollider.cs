using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeCollider : MonoBehaviour
{
    private Entity ownerEntity;

    private void Awake()
    {
        ownerEntity = GetComponentInParent<Entity>();
        if (ownerEntity == null)
        {
            Debug.Log("Failed to get entity with range");
        }
    }

    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
        if (otherCollider.TryGetComponent<Entity>(out var otherEntity))
        {
            if (ownerEntity.IsAgainst(otherEntity))
            {
                ownerEntity.EntitiesInRange.Add(otherEntity);
                // If there is no currently targeted entity, focus on the first entity in range
                if (ownerEntity.TargetedEntity == null)
                {
                    ownerEntity.TargetEnemy(otherEntity);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D otherCollider)
    {
        if (otherCollider.CompareTag("Minion") || otherCollider.CompareTag("Player") || otherCollider.CompareTag("AIPlayer") || otherCollider.CompareTag("Tower"))
        {
            Entity entity = otherCollider.GetComponent<Entity>();
            ownerEntity.EntitiesInRange.Remove(entity);

            // If the targeted entity leaves the range, switch focus to the next entity
            if (ownerEntity.TargetedEntity != null)
            {
                if (ownerEntity.TargetedEntity.transform == otherCollider.transform)
                {
                    ownerEntity.StopTargetEnemy(entity);
                    ownerEntity.FindNewTarget(ownerEntity.TargetingPriority);
                }
            }
        }
    }
}
