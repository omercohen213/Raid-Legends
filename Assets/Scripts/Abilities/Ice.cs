using UnityEngine;

public class Ice : Ability
{  
    public override void UseAbility(Vector3 abilityPosition)
    {
        if (!_isCd)
        {
            _isCd = true;
            _abilityObject.transform.position = abilityPosition;
            _anim.SetTrigger("Ability3");
            int abilityDamage = Mathf.RoundToInt(_baseDamage + _level * _baseDamage * _damageScaling);
            _player.TargetedEntity.ReceiveDamage(abilityDamage, false, true);
        }    
    }
}