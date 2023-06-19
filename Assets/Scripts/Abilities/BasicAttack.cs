using System.Collections;
using UnityEngine;

public class BasicAttack : Ability
{
    private readonly float _attackDuration = 0.2f;
    public override void UseAbilityOverride(Vector3 abilityPosition)
    {
        BasicAttackAnimation();
        _player.ShowPlayerRange(_attackDuration);
        bool isCritical = Random.value < _player.CritChance;
        _player.TargetedEntity.ReceiveDamage(_player.BaseDamage, isCritical, _player);
    }

    // Basic attack animation
    public void BasicAttackAnimation()
    {
        // check if animation in progress
        _player.BasicAttack();
        StartCoroutine(BasicAttackAnimationCancel());
    }

    private IEnumerator BasicAttackAnimationCancel()
    {
        yield return new WaitForSeconds(_attackDuration);
        _player.BasicAttackOff();
    }
}
