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
        string indicatorObjectName = _indicatorPrefab.name;
        GameObject existingIndicator = GameObject.Find("RunTimeObjects/" + indicatorObjectName);

        if (existingIndicator != null)
        {
            _indicator = existingIndicator;
            _indicator.transform.SetPositionAndRotation(_initialIndicatorPosition, Quaternion.identity);
            _indicator.GetComponent<SpriteRenderer>().enabled = true;
        }
        else
        {
            _indicator = Instantiate(_indicatorPrefab, _initialIndicatorPosition, Quaternion.identity, GameObject.Find("RunTimeObjects").transform);
            _indicator.name = indicatorObjectName;
        }
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
