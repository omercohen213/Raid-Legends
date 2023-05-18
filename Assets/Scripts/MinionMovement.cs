using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionMovement : EntityMovement
{
    private Vector3 startingPos;
    private Minion minion;

    protected override void Awake()
    {
        base.Awake();
        if (GetComponent<Minion>() != null)
        {
            minion = GetComponent<Minion>();
        }
        else Debug.Log("Missing component Minion");
    }

    protected override void Start()
    {
        base.Start();
        startingPos = transform.position;
        if (minion.EntityTeam == Entity.Team.Blue)
        {
            moveDir = Vector2.right;
        }
        else if (minion.EntityTeam == Entity.Team.Red)
        {
            moveDir = Vector2.left;
        }
        speed = 2f;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMovement(moveDir);
    }
}
