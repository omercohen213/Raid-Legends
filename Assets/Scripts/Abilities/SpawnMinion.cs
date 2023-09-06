using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SpawnMinion : OnPointAbility
{
    public override void CastAbility(Vector3 abilityPosition, Entity caster)
    {
        if (!_isCd)
        {
            _isCd = true;
            Instantiate(_abilityObject, abilityPosition, Quaternion.identity, GameObject.Find("RunTimeObjects").transform);
        }
    }
}