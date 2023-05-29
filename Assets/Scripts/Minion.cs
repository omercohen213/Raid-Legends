using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Minion : Entity
{
    private Minion minion;

    protected override void Awake()
    {
        base.Awake();
        minion = GetComponent<Minion>();
    }

    protected override void Start()
    {
        base.Start();
        if (EntityTeam == Team.Blue)
        {
            moveDir = Vector2.right;
        }
        else if (EntityTeam == Team.Red)
        {
            moveDir = Vector2.left;
        }
        targetingPriority = new List<Type> { Type.Player, Type.AIPlayer, Type.Minion, Type.Tower};
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMovement(moveDir);
    }

    public override void Death()
    {
        gameObject.SetActive(false);
    }
}
