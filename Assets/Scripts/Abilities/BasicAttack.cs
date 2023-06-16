using System.Collections;
using UnityEngine;

public class BasicAttack : Ability
{
    private float _attackDuration = 0.2f;
    public override void UseAbility(Vector3 abilityPosition)
    {
        if (!_isCd)
        {
            _isCd = true;
            BasicAttackAnimation();
            _player.ShowPlayerRange(_attackDuration);
            bool isCritical = Random.value < _player.CritChance;
            _player.TargetedEntity.ReceiveDamage(_player.BaseDamage, isCritical, true);
        }
    }

    // Basic attack animation
    public void BasicAttackAnimation()
    {
        // check if animation in progress
        _player.Anim.SetBool("Attack", true);
        StartCoroutine(BasicAttackAnimationCancel());
    }

    private IEnumerator BasicAttackAnimationCancel()
    {
        yield return new WaitForSeconds(_attackDuration);
        _player.Anim.SetBool("Attack", false);
    }
}
