using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetRange : MonoBehaviour
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
                _ownerEntity.EntitiesInTargetRange.Add(otherEntity);

                // If there is no currently targeted entity, target enemy in range
                if (_ownerEntity.TargetedEntity == null)
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
            _ownerEntity.EntitiesInTargetRange.Remove(entity);

            // If the targeted entity leaves the range, switch target to the next enemy in range
            if (_ownerEntity.TargetedEntity != null)
            {
                if (_ownerEntity.TargetedEntity.transform == otherCollider.transform)
                {
                    _ownerEntity.StopTargetEnemy();
                    _ownerEntity.FindPriotityTarget(_ownerEntity.EntitiesInTargetRange);
                }
            }
        }
    }
}
