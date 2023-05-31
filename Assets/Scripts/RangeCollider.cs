using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeCollider : MonoBehaviour
{
    private Entity _ownerEntity;

    private void Awake()
    {
        _ownerEntity = GetComponentInParent<Entity>();
        if (_ownerEntity == null)
        {
            Debug.Log("Failed to get entity with range");
        }
    }

    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
        if (otherCollider.TryGetComponent<Entity>(out var otherEntity))
        {
            if (_ownerEntity.IsAgainst(otherEntity))
            {
                _ownerEntity.EntitiesInRange.Add(otherEntity);

                // If there is no currently targeted entity, focus on the first entity in range
                // (Player targets enemies with touch or abilities)
                if (_ownerEntity.TargetedEntity == null && _ownerEntity is not Player)
                {
                    _ownerEntity.TargetEnemy(otherEntity);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D otherCollider)
    {
        if (otherCollider.CompareTag("Minion") || otherCollider.CompareTag("Player") || otherCollider.CompareTag("AIPlayer") || otherCollider.CompareTag("Tower"))
        {
            Entity entity = otherCollider.GetComponent<Entity>();
            _ownerEntity.EntitiesInRange.Remove(entity);

            // If the targeted entity leaves the range, switch focus to the next entity
            if (_ownerEntity.TargetedEntity != null)
            {
                if (_ownerEntity.TargetedEntity.transform == otherCollider.transform)
                {
                    _ownerEntity.StopTargetEnemy();
                    _ownerEntity.FindNewTarget(_ownerEntity.TargetingPriority);
                }
            }
        }
    }
}
