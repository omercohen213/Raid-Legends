using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

public class FireBall : Ability
{
    [SerializeField] private FireballCollision fireballCollision;
    private Vector3 _direction;


    public override void UseAbility(Vector3 abilityPosition)
    {
        if (!_isCd)
        {
            _isCd = true;
            //GameObject fireBall = Instantiate(_abilityObject, abilityPosition, Quaternion.identity, GameObject.Find("AbilityObjects").transform);
            _abilityObject.gameObject.SetActive(true);
            _abilityObject.transform.position = _player.transform.position + _directionOffset;
            _anim.SetTrigger("FireBall");

            _direction = abilityPosition - _abilityObject.transform.position;
            float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
            _abilityObject.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            fireballCollision.AbilityDamage = Mathf.RoundToInt(_baseDamage + _level * _baseDamage * _damageScaling);
            fireballCollision.Direction = _direction;
            fireballCollision.InitialPosition = _initialIndicatorPosition;
        }
    }
}
