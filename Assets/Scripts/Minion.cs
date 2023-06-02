using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(Rigidbody2D))]
public class Minion : Entity
{
    private ObjectPool<Minion> _pool;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        _movementSpeed = 2f;
        if (EntityTeam == Team.Blue)
        {
            _moveDir = Vector2.right;
        }
        else if (EntityTeam == Team.Red)
        {
            _moveDir = Vector2.left;
        }
        _targetingPriority = new List<Type> { Type.Player, Type.AIPlayer, Type.Minion, Type.Tower };
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMovement(_moveDir);
    }

    public void SetPool(ObjectPool<Minion> pool) => _pool = pool;

    public override void Death()
    {
        base.Death();
        gameObject.SetActive(false);
        if (_pool != null)
        {
            _pool.Release(this);
        }
        else
        {
            Debug.Log("No pool", this);
            Destroy(gameObject);
        }
    }

}
