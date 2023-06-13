using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

public class FireBall : Ability
{
    public override void UseAbility(Finger finger)
    {
        if (!_isCd)
        {
/*            _isCd = true;
            GameObject fireBall = Instantiate(_abilityObject, abilityPosition, Quaternion.identity, GameObject.Find("AbilityObjects").transform);
            _anim.SetTrigger("FireBall");
            int abilityDamage = Mathf.RoundToInt(_baseDamage + _level * _baseDamage * _damageScaling);
            _player.TargetedEntity.ReceiveDamage(abilityDamage, false, true);

            Vector3 direction = targetPosition - fireballTransform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            _abilityObject.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);*/
        }
    }
}
