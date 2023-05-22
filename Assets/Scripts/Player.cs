using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : Entity
{
    [SerializeField] private Joystick joystick;
    [SerializeField] private Animator anim;
    [SerializeField] private Transform bars;
    [SerializeField] private SpriteRenderer rangeSpriteRenderer;

    [SerializeField] private readonly float barsScale = 0.35f;
    private readonly float joystickMinInput = 0.25f;
    private readonly float attackAnimationTime = 1f;

    void Update()
    {
        float horizontalInput = joystick.Horizontal;
        float verticalInput = joystick.Vertical;

        // Move only if joystick input acceeds the minimal input
        if (horizontalInput > joystickMinInput || verticalInput > joystickMinInput || horizontalInput < -joystickMinInput || verticalInput < -joystickMinInput)
        {
            // Negative scales to match sprite movement
            Vector3 currentPlayerScale = transform.localScale;
            Vector3 currentBarsScale = bars.transform.localScale;
            if (horizontalInput > joystickMinInput)
            {
                currentPlayerScale.x = 1f;
                currentBarsScale.x = barsScale;
            }
            else if (horizontalInput < -joystickMinInput)
            {
                currentPlayerScale.x = -1f;
                currentBarsScale.x = -barsScale;
            }
            transform.localScale = currentPlayerScale;
            bars.localScale = currentBarsScale;

            moveDir = new Vector3(horizontalInput * speed, verticalInput * speed, 0);
            Walk();
            UpdateMovement(moveDir);
        }
        else WalkOff();
    }

    public void ShowPlayerRange(float timeBeforeHide)
    {
        rangeSpriteRenderer.enabled = true;
        StartCoroutine(HideRange(timeBeforeHide));
    }
    private IEnumerator HideRange(float timeBeforeHide)
    {
        yield return new WaitForSeconds(timeBeforeHide);
        rangeSpriteRenderer.enabled = false;
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
    public void AttackAnimation()
    {
        // check if animation in progress
        anim.SetBool("Attack", true);
        StartCoroutine(AttackAnimationCancel());
    }

    private IEnumerator AttackAnimationCancel()
    {
        yield return new WaitForSeconds(attackAnimationTime);
        AttackOff();
    }

    public void AttackOff()
    {
        anim.SetBool("Attack", false);
    }


}
