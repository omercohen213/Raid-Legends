using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballCollision : MonoBehaviour
{
    private readonly float _speed = 10f;
    private float _maxRange;
    private Vector3 _direction;
    private Vector3 _initialPosition;
    private int abilityDamage;
    private Player _player;

    public Vector3 Direction { get => _direction; set => _direction = value; }
    public int AbilityDamage { get => abilityDamage; set => abilityDamage = value; }
    public Vector3 InitialPosition { get => _initialPosition; set => _initialPosition = value; }

    private void Start()
    {
        if (!GameObject.Find("Player").TryGetComponent(out _player))
        {
            Debug.LogError("Missing Player object");
        }
    }

    private void Update()
    {
        _maxRange = Vector3.Distance(_initialPosition, _player.transform.position + (_direction.normalized * _player.AttackRange.radius));
        Vector3 movement = _speed * Time.deltaTime * _direction.normalized;
        transform.position += movement;
        if (Vector3.Distance(transform.position, _initialPosition) >= _maxRange)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Entity collEntity))
        {
            if (collEntity.IsAgainst(_player))
            {
                collEntity.ReceiveDamage(abilityDamage, false, true);
                gameObject.SetActive(false);
            }
        }
    }
}
