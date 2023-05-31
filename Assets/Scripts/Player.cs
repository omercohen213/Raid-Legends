using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : Entity
{
    [SerializeField] private Joystick _joystick;
    [SerializeField] private Animator _anim;
    [SerializeField] private Transform _bars;
    [SerializeField] private SpriteRenderer _rangeSpriteRenderer;

    [SerializeField] private readonly float _barsScale = 0.35f;
    private readonly float _joystickMinInput = 0.25f;
    private readonly float _attackAnimationTime = 1f;

    private Entity _lastTargetedEntity;

    protected override void Start()
    {
        base.Start();
        _targetingPriority = new List<Type> { Type.Player, Type.AIPlayer, Type.Minion };
        _critChance = 0.2f;
    }

    private void Update()
    {
        float horizontalInput = _joystick.Horizontal;
        float verticalInput = _joystick.Vertical;

        // Move only if joystick input exceeds the minimum input threshold
        if (Mathf.Abs(horizontalInput) > _joystickMinInput || Mathf.Abs(verticalInput) > _joystickMinInput)
        {
            // Negative scales to match sprite movement
            Vector3 currentPlayerScale = transform.localScale;
            Vector3 currentBarsScale = _bars.transform.localScale;
            if (horizontalInput > _joystickMinInput)
            {
                currentPlayerScale.x = 1f;
                currentBarsScale.x = _barsScale;
            }
            else if (horizontalInput < -_joystickMinInput)
            {
                currentPlayerScale.x = -1f;
                currentBarsScale.x = -_barsScale;
            }
            transform.localScale = currentPlayerScale;
            _bars.localScale = currentBarsScale;

            _moveDir = new Vector3(horizontalInput, verticalInput, 0);
            Walk();
        }
        else
        {
            _moveDir = Vector3.zero;
            WalkOff();
        }
        base.UpdateMovement(_moveDir);
    }

    public void ShowPlayerRange(float timeBeforeHide)
    {
        _rangeSpriteRenderer.enabled = true;
        StartCoroutine(HideRange(timeBeforeHide));
    }
    private IEnumerator HideRange(float timeBeforeHide)
    {
        yield return new WaitForSeconds(timeBeforeHide);
        _rangeSpriteRenderer.enabled = false;
    }

    public override void TargetEnemy(Entity entity)
    {
        base.TargetEnemy(entity);
        _lastTargetedEntity = entity;
        Transform target = entity.transform.Find("Target");
        if (target != null)
        {
            target.gameObject.SetActive(true);
        }
        else Debug.LogError("Missing target object");

        UIManager.Instance.ShowUIEntityStats(entity.gameObject);
    }

    public override void StopTargetEnemy()
    {
        base.StopTargetEnemy();
        if (_lastTargetedEntity != null)
        {
            Transform target = _lastTargetedEntity.transform.Find("Target");
            if (target != null)
            {
                target.gameObject.SetActive(false);
            }
            else Debug.LogError("Missing target object");
        }      

        UIManager.Instance.HideUIEntityStats();
    }

    public void Jump()
    {
        _anim.SetBool("Jump", true);
    }

    public void JumpOff()
    {
        _anim.SetBool("Jump", false);
    }

    public void Dead()
    {
        _anim.SetBool("Dead", true);
    }

    public void DeadOff()
    {
        _anim.SetBool("Dead", false);
    }
    public void Walk()
    {
        _anim.SetBool("Walk", true);
    }

    public void WalkOff()
    {
        _anim.SetBool("Walk", false);
    }
    public void Run()
    {
        _anim.SetBool("Run", true);
    }
    public void RunOff()
    {
        _anim.SetBool("Run", false);
    }
    public void AttackAnimation()
    {
        // check if animation in progress
        _anim.SetBool("Attack", true);
        StartCoroutine(AttackAnimationCancel());
    }

    private IEnumerator AttackAnimationCancel()
    {
        yield return new WaitForSeconds(_attackAnimationTime);
        _anim.SetBool("Attack", false);
    }



}
