using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SpawnMinion : Ability
{
    public override void UseAbility(Vector3 abilityPosition)
    {
        if (!_isCd)
        {
            _isCd = true;
            Instantiate(_abilityObject, abilityPosition, Quaternion.identity, GameObject.Find("AbilityObjects").transform);
        }
    }
}
