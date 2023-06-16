using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionAbility : IndicatorAbility
{
    protected readonly Vector3 _directionOffset = new(0.17f, -0.35f);
    protected Vector3 _direction;

    public override void OnAbilityTouch(Vector3 fingerPosition)
    {
        base.OnAbilityTouch(fingerPosition);
        _initialIndicatorPosition = _player.transform.position + _directionOffset;
        float angle = GetIndicatorAngle(fingerPosition);

        string indicatorObjectName = _indicatorPrefab.name;
        GameObject existingIndicator = GameObject.Find("AbilityObjects/" + indicatorObjectName);
        if (existingIndicator != null)
        {
            _indicator = existingIndicator;
            _indicator.transform.SetPositionAndRotation(_player.transform.position + _directionOffset, Quaternion.Euler(0f, 0f, angle));
            _indicator.GetComponent<SpriteRenderer>().enabled = true;
        }
        else
        {
            _indicator = Instantiate(_indicatorPrefab, _initialIndicatorPosition, Quaternion.Euler(0f, 0f, angle), GameObject.Find("AbilityObjects").transform);
            _indicator.name = indicatorObjectName;
        }

        _indicator.transform.localScale = new Vector3(1f, _player.AttackRange.radius / 3.8f);
    }
    public override void MoveIndicator(Vector3 fingerPosition)
    {
        float angle = GetIndicatorAngle(fingerPosition);
        _indicator.transform.SetPositionAndRotation(_player.transform.position + _directionOffset, Quaternion.Euler(0f, 0f, angle));
    }

    public override void ReleaseIndicator(Vector3 fingerPosition)
    {
        UseAbility(fingerPosition);
        base.ReleaseIndicator(fingerPosition);
    }

    
}
