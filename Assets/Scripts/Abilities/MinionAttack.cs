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
        if (!_isCd)
        {
            _isCd = true;
            Vector3 initialPosition = caster.transform.position;
            Vector3 direction = caster.TargetedEntity.transform.position - abilityPosition;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            GameObject abilityCollisionGo = Instantiate(_minionAttackPrefab, initialPosition, Quaternion.AngleAxis(angle, Vector3.forward), transform);
            AbilityCollision abilityCollision = abilityCollisionGo.GetComponent<AbilityCollision>();

            float speed = 1f;
            float maxRange = int.MaxValue;
            abilityCollision.SetAbilityValues(caster, _baseDamage, speed, maxRange, initialPosition, direction);
        }
    }
}

