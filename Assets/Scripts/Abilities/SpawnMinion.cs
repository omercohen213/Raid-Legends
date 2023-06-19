using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SpawnMinion : OnPointAbility
{
    public override void UseAbilityOverride(Vector3 abilityPosition)
    {
        Instantiate(_abilityObject, abilityPosition, Quaternion.identity, GameObject.Find("RunTimeObjects").transform);
    }
}