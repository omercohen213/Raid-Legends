using UnityEngine;

public class Ice : Ability
{
    public override void UseAbilityOverride(Vector3 abilityPosition)
    {
        //_abilityObject = Instantiate(_abilityObjectPrefab, abilityPosition, Quaternion.identity, _player.TargetedEntity.transform);
        _abilityObject.transform.position = abilityPosition;
        _anim.SetTrigger("Ice");
        int abilityDamage = Mathf.RoundToInt(_baseDamage + _level * _baseDamage * _damageScaling);
        _player.TargetedEntity.ReceiveDamage(abilityDamage, false, _player);
    }

    protected override void Update()
    {
        base.Update();
        //_abilityObject.transform.position = _player.TargetedEntity.transform.position;
    }
}
