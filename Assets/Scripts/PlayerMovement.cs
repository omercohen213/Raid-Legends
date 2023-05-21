using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Player))]
public class PlayerMovement : EntityMovement
{
    [SerializeField] private Joystick joystick;
    [SerializeField] private Animator anim;
    [SerializeField] private float playerScale = 0.2f;
    private readonly float joystickMinInput = 0.25f;
    private readonly float attackAnimationTime = 1f;

    void Update()
    {
        float horizontalInput = joystick.Horizontal;
        float verticalInput = joystick.Vertical;

        // Move only if joystick input acceeds the minimal input
        if (horizontalInput > joystickMinInput || verticalInput > joystickMinInput || horizontalInput < -joystickMinInput || verticalInput < -joystickMinInput)
        {
            Vector3 currentScale = transform.localScale;
            if (horizontalInput > joystickMinInput)
                currentScale.x = playerScale;
            else if (horizontalInput < -joystickMinInput)
                currentScale.x = -playerScale;
            transform.localScale = currentScale;

            moveDir = new Vector3(horizontalInput * speed, verticalInput * speed, 0);
            Walk();
            UpdateMovement(moveDir);
        }       
        else WalkOff();
    }

    public void Jump()
    {
        anim.SetBool("Jump", true);
    }

    public void JumpOff()
    {
        anim.SetBool("Jump", false);
    }

    public void Dead()
    {
        anim.SetBool("Dead", true);
    }

    public void DeadOff()
    {
        anim.SetBool("Dead", false);
    }
    public void Walk()
    {
        anim.SetBool("Walk", true);
    }

    public void WalkOff()
    {
        anim.SetBool("Walk", false);
    }
    public void Run()
    {
        anim.SetBool("Run", true);
    }
    public void RunOff()
    {
        anim.SetBool("Run", false);
    }
    public void Attack()
    {
        // check if animation in progress
        Debug.Log("BasicAttack");
        anim.SetBool("Attack", true);
        StartCoroutine(AttackCancel());
    }

    private IEnumerator AttackCancel()
    {
        yield return new WaitForSeconds(attackAnimationTime);
        AttackOff();
    }

    public void AttackOff()
    {
        anim.SetBool("Attack", false);
    }
}
