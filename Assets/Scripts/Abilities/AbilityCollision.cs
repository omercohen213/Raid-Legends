using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The ability moving object collision detection
public class AbilityCollision : MonoBehaviour
{
    protected float _speed;
    protected float _maxRange;
    protected Vector3 _direction;
    protected Vector3 _initialPosition;
    protected int _abilityDamage;
    protected Entity _caster;
    public Vector3 Direction { get => _direction; set => _direction = value; }
    public int AbilityDamage { get => _abilityDamage; set => _abilityDamage = value; }
    public Vector3 InitialPosition { get => _initialPosition; set => _initialPosition = value; }
    public Entity Caster { get => _caster; set => _caster = value; }
    public float Speed { get => _speed; set => _speed = value; }

    protected virtual void Update()
    {
        Vector3 movement = _speed * Time.deltaTime * _direction.normalized;
        transform.position += movement;

        // Disable object if reaches max range (object pooling?)
        if (Vector3.Distance(transform.position, _initialPosition) >= _maxRange)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if hits an enemy
        if (other.TryGetComponent(out Entity collEntity))
        {
            if (collEntity.IsAgainst(_caster))
            {
                collEntity.ReceiveDamage(_abilityDamage, false, _caster);
                gameObject.SetActive(false);
                //Debug.Log("hit " + collEntity + " " + _abilityDamage);
            }
        }
    }

    public void SetAbilityValues(Entity caster, int damage, float speed, float maxRange, Vector3 initialPosition, Vector3 direction)
    {
        _caster = caster;
        _abilityDamage = damage;
        _speed = speed;
        _maxRange = maxRange;
        _initialPosition = initialPosition;
        _direction = direction;
    }
}
