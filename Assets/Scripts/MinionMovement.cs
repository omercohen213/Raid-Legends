using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Minion))]
public class MinionMovement : EntityMovement
{
    private Minion minion;

    protected override void Awake()
    {
        base.Awake();
        minion = GetComponent<Minion>();
    }

    private void Start()
    {
        if (minion.EntityTeam == Entity.Team.Blue)
        {
            moveDir = Vector2.right;
        }
        else if (minion.EntityTeam == Entity.Team.Red)
        {
            moveDir = Vector2.left;
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMovement(moveDir);
    }
}
