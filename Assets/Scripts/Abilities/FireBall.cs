using UnityEngine;

public class FireBall : DirectionAbility
{
    [SerializeField] private FireballCollision fireballCollision;

    public override void UseAbilityOverride(Vector3 abilityPosition)
    {
        //GameObject fireBall = Instantiate(_abilityObject, abilityPosition, Quaternion.identity, GameObject.Find("").transform);
        _abilityObject.SetActive(true);
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

