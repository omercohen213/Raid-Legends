using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class TouchManager : MonoBehaviour
{
    [SerializeField] private Player _player;

    [SerializeField] private GameObject _touchPrefab;
    private Dictionary<int, GameObject> _touchIndicators;

    private int _abilityFingerIndex;
    private Ability _currentTouchedAbility;
    private GameObject _currentAbilityIndicator;

    private void Awake()
    {
        _touchIndicators = new Dictionary<int, GameObject>();
    }

    private void OnEnable()
    {
        TouchSimulation.Enable();
        EnhancedTouchSupport.Enable();
        Touch.onFingerDown += OnFingerDown;
        Touch.onFingerMove += OnFingerMove;
        Touch.onFingerUp += OnFingerUp;

    }

    private void OnDisable()
    {
        Touch.onFingerDown -= OnFingerDown;
        Touch.onFingerMove -= OnFingerMove;
        Touch.onFingerUp -= OnFingerUp;
        TouchSimulation.Disable();
        EnhancedTouchSupport.Disable();
    }

    private void OnFingerDown(Finger finger)
    {
        // Show touch indicators
        int fingerId = finger.index;

        GameObject touchIndicator = Instantiate(_touchPrefab, finger.screenPosition, Quaternion.identity, GameObject.Find("UI").transform);
        _touchIndicators.Add(fingerId, touchIndicator);

        // Check touch position
        Collider2D coll = Physics2D.OverlapPoint(finger.screenPosition);
        if (coll != null)
        {
            if (coll.gameObject.CompareTag("AbilityUI"))
            {
                if (coll.gameObject.TryGetComponent<Ability>(out var touchedAbility))
                {
                    /*if (touchedAbility.isOnHoldTouch)
                    {
                        _isTouchOnAbility2 = true;
                        //StartCoroutine(WaitForTapOrHold(finger));
                    }*/
                    _currentTouchedAbility = touchedAbility;
                    _player.UpdateAttackRange(touchedAbility.Range);
                    // If ability has indicator, instantiate it and wait for touch release
                    if (touchedAbility.HasIndicator)
                    {
                        /*if (touchedAbility.HasDirection)
                        {
                            
                        }*/
                        _abilityFingerIndex = finger.index;
                        GameObject indicatorPrefab = _currentTouchedAbility.IndicatorPrefab;
                        Vector3 indicatorPosition = GetClosestPointToPlayerRange(finger);
                        _currentAbilityIndicator = Instantiate(indicatorPrefab, indicatorPosition, Quaternion.identity, GameObject.Find("AbilityObjects").transform);
                        touchedAbility.ShrinkAbilityImage(touchedAbility);
                        _player.ShowPlayerRange();
                    }

                    // Else use the ability immediately and show touch and range for a brief moment
                    else
                    {
                        float transitionDuration = 0.2f;
                        touchedAbility.ShrinkAbilityImage(touchedAbility, transitionDuration);
                        _player.ShowPlayerRange(transitionDuration);

                        _player.TryUseAbility(touchedAbility, Vector3.zero);
                    }

                }
            }

            float touchRadius = 0.5f;

            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(finger.screenPosition);
            Collider2D[] colliders = Physics2D.OverlapCircleAll(touchPosition, touchRadius); //, LayerMask.GetMask("Entity")

            foreach (Collider2D collider in colliders)
            {
                if (collider.gameObject.TryGetComponent(out Entity entityColl))
                {
                    if (_player.IsAgainst(entityColl))
                    {
                        _player.TargetEnemy(entityColl);
                    }
                    UIManager.Instance.ShowUIEntityStats(collider.gameObject);
                    break;
                }
            }
        }
    }

    private void OnFingerMove(Finger finger)
    {
        int fingerId = finger.index;
        var keyValuePair = _touchIndicators.FirstOrDefault(pair => pair.Key == fingerId);
        _touchIndicators[keyValuePair.Key].transform.position = finger.screenPosition;

        if (_currentTouchedAbility != null && _currentTouchedAbility.HasIndicator && _abilityFingerIndex == finger.index)
        {
            HandleAbilityIndicator(finger);
        }
    }

    private void OnFingerUp(Finger finger)
    {
        // Destroy touch indicator
        int fingerId = finger.index;
        if (_touchIndicators.ContainsKey(fingerId))
        {
            Destroy(_touchIndicators[fingerId]);
            _touchIndicators.Remove(fingerId);
        }

        // Destroy ability indicator and use ability
        if (_currentTouchedAbility != null && _currentTouchedAbility.HasIndicator && _abilityFingerIndex == finger.index)
        {
            Vector3 indicatorPos = _currentAbilityIndicator.transform.position;
            _player.TryUseAbility(_currentTouchedAbility, indicatorPos);
            _currentTouchedAbility.ResetAbilityImage(_currentTouchedAbility);
            _player.HidePlayerRange();

            _currentTouchedAbility = null;
            Destroy(_currentAbilityIndicator);


        }
    }

    // Show indicator according to player's range
    private void HandleAbilityIndicator(Finger finger)
    {
        Vector3 fingerPosition = Camera.main.ScreenToWorldPoint(finger.screenPosition);
        fingerPosition.z = 0;
        if (_currentAbilityIndicator != null)
        {
            float distanceToPlayer = GetDistanceFromPlayer(finger);

            // Check if the finger position is within the ability range
            if (distanceToPlayer <= _currentTouchedAbility.Range)
            {
                _currentAbilityIndicator.transform.position = fingerPosition;
            }

            else
            {
                Vector3 indicatorPosition = GetClosestPointToPlayerRange(finger);
                _currentAbilityIndicator.transform.position = indicatorPosition;
            }
        }
    }

    private float GetDistanceFromPlayer(Finger finger)
    {
        Vector3 playerPosition = _player.transform.position;
        Vector3 fingerPosition = Camera.main.ScreenToWorldPoint(finger.screenPosition);
        fingerPosition.z = 0;
        Vector3 directionToPlayer = fingerPosition - playerPosition;
        float distanceToPlayer = directionToPlayer.magnitude;
        return distanceToPlayer;
    }

    private Vector3 GetClosestPointToPlayerRange(Finger finger)
    {
        Vector3 playerPosition = _player.transform.position;
        Vector3 fingerPosition = Camera.main.ScreenToWorldPoint(finger.screenPosition);
        fingerPosition.z = 0;
        Vector3 directionToPlayer = fingerPosition - playerPosition;
        Vector3 closestPoint = playerPosition + (directionToPlayer.normalized * _currentTouchedAbility.Range);
        return closestPoint;
    }

    private IEnumerator WaitForTapOrHold(Finger finger)
    {
        float holdDuration = 1f; // Adjust this duration as needed
        float timer = 0f;

        while (timer < holdDuration && finger.isActive)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        if (finger.isActive)
        {
            Debug.Log(" co held for 1s");

        }
    }


}
