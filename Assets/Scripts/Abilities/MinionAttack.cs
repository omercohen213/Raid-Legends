using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;

public class MinionAttack : Ability
{
    [SerializeField] private GameObject _minionAttackPrefab;

    public override void CastAbility(Vector3 abilityPosition, Entity caster)
    {
        GameObject abilityCollisionGo = Instantiate(_minionAttackPrefab, transform.position, Quaternion.identity, transform);
        AbilityCollision abilityCollision = abilityCollisionGo.GetComponent<AbilityCollision>();

        if (!_isCd)
        {
            _isCd = true;
            Vector3 direction = abilityPosition - caster.TargetedEntity.transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            abilityCollision.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            float speed = 1f;
            float maxRange = int.MaxValue;
            abilityCollision.SetAbilityValues(caster, _baseDamage, speed, maxRange, transform.position, direction);
        }
    }
}

