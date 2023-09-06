using UnityEngine;

public class FireBall : DirectionAbility
{
    [SerializeField] private FireballCollision _fireballCollision;

    public override void CastAbility(Vector3 abilityPosition, Entity caster)
    {
        _caster = caster;
        if (!_isCd)
        {
            _isCd = true;
            _abilityObject.SetActive(true);
            _abilityObject.transform.position = _player.transform.position + _directionOffset;
            _anim.SetTrigger("FireBall");

            _direction = abilityPosition - _abilityObject.transform.position;
            float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
            _abilityObject.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            // Initialize the object collision detection with variables
            int damage = Mathf.RoundToInt(_baseDamage + _level * _baseDamage * _damageScaling);
            float maxRange = Vector3.Distance(_initialIndicatorPosition, _initialIndicatorPosition + (_direction.normalized * _caster.AttackRange.radius));
            float speed = 10f;
            _fireballCollision.SetAbilityValues(_caster, damage, speed, maxRange, _initialIndicatorPosition, _direction);

           /* _fireballCollision.Caster = caster;
            _fireballCollision.AbilityDamage = Mathf.RoundToInt(_baseDamage + _level * _baseDamage * _damageScaling);
            _fireballCollision.Direction = _direction;
            _fireballCollision.InitialPosition = _initialIndicatorPosition;
            _fireballCollision.Speed = 10f;*/
        }
    }
}

