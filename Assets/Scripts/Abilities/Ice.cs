using UnityEngine;

public class Ice : Ability
{  
    public override void UseAbility(Vector3 abilityPosition)
    {
        base.UseAbility(abilityPosition);

        if (!_isCd)
        {
            _isCd = true;
            _abilityObject.transform.position = abilityPosition;
            _anim.SetTrigger("Ice");
            int abilityDamage = Mathf.RoundToInt(_baseDamage + _level * _baseDamage * _damageScaling);
            _player.TargetedEntity.ReceiveDamage(abilityDamage, false, true);
        }    
    }

    protected override void Update()
    {
        base.Update();
        //_abilityObject.transform.position = abilityPosition;
    }
}
