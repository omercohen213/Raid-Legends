using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackRange : MonoBehaviour
{
    private Entity _ownerEntity;

    private void Awake()
    {
        _ownerEntity = GetComponentInParent<Entity>();
        if (_ownerEntity == null)
        {
            Debug.Log("Failed to get entity with attack range");
        }
    }

    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
        if (otherCollider.TryGetComponent<Entity>(out var otherEntity))
        {
            if (_ownerEntity.IsAgainst(otherEntity))
            {
                _ownerEntity.EntitiesInAttackRange.Add(otherEntity);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D otherCollider)
    {
        if (otherCollider.CompareTag("Minion") || otherCollider.CompareTag("Player") || otherCollider.CompareTag("AIPlayer") || otherCollider.CompareTag("Tower"))     
        {
            Entity entity = otherCollider.GetComponent<Entity>();
            _ownerEntity.EntitiesInAttackRange.Remove(entity);
        }
    }

    
}
