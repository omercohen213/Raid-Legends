using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OnPointAbility : IndicatorAbility
{
    public override void OnAbilityTouch(Vector3 fingerPosition)
    {
        base.OnAbilityTouch(fingerPosition);
        _initialIndicatorPosition = GetClosestPointToPlayerRange(fingerPosition);
        _indicator = Instantiate(_indicatorPrefab, _initialIndicatorPosition, Quaternion.identity, GameObject.Find("AbilityObjects").transform);
    }

    public override void MoveIndicator(Vector3 fingerPosition)
    {
        float distanceToPlayer = GetDistanceFromPlayer(fingerPosition);

        // Check if the finger position is within the ability range
        if (distanceToPlayer <= _range)
        {
            _indicator.transform.position = fingerPosition;
        }

        else
        {
            Vector3 indicatorPosition = GetClosestPointToPlayerRange(fingerPosition);
            _indicator.transform.position = indicatorPosition;
        }
    }

    public override void ReleaseIndicator(Vector3 fingerPosition)
    {
        Vector3 indicatorPos = _indicator.transform.position;
        UseAbility(indicatorPos);
        base.ReleaseIndicator(fingerPosition);
    }

    private float GetDistanceFromPlayer(Vector3 fingerPosition)
    {
        Vector3 playerPosition = _player.transform.position;
        Vector3 directionToPlayer = fingerPosition - playerPosition;
        float distanceToPlayer = directionToPlayer.magnitude;
        return distanceToPlayer;
    }

    private Vector3 GetClosestPointToPlayerRange(Vector3 fingerPosition)
    {
        Vector3 playerPosition = _player.transform.position;
        Vector3 directionToPlayer = fingerPosition - playerPosition;
        Vector3 closestPoint = playerPosition + (directionToPlayer.normalized * _range);
        return closestPoint;
    }
}
