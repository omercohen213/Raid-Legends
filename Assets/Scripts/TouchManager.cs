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
        Collider2D screenColl = Physics2D.OverlapPoint(finger.screenPosition);
        if (screenColl != null)
        {
            if (screenColl.gameObject.CompareTag("AbilityUI"))
            {
                if (screenColl.gameObject.TryGetComponent<Ability>(out var touchedAbility))
                {
                    _abilityFingerIndex = finger.index;
                    _currentTouchedAbility = touchedAbility;
                    Vector3 fingerPosition = Camera.main.ScreenToWorldPoint(finger.screenPosition);
                    fingerPosition.z = 0;
                    touchedAbility.OnAbilityTouch(fingerPosition);
                }
            }
        }

        float touchRadius = 0.5f;
        Vector3 touchPosition = Camera.main.ScreenToWorldPoint(finger.screenPosition);
        Collider2D[] worldColliders = Physics2D.OverlapCircleAll(touchPosition, touchRadius);

        foreach (Collider2D worldColl in worldColliders)
        {
            if (screenColl != null)
            {
                if (screenColl.gameObject.CompareTag("AbilityUI"))
                {
                    return;
                }
            }

            if (worldColl.gameObject.TryGetComponent(out Entity entity))
            {
                OnEntityTouch(entity);
                break;
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
            Vector3 fingerPosition = Camera.main.ScreenToWorldPoint(finger.screenPosition);
            fingerPosition.z = 0;
            _currentTouchedAbility.MoveIndicator(fingerPosition);

            Collider2D coll = Physics2D.OverlapPoint(finger.screenPosition);
            if (coll != null)
            {
                if (coll.gameObject.name == "AbilityCancel")
                {
                    _currentTouchedAbility.AbilityCancelHover();
                }
            }
            else _currentTouchedAbility.AbilityCancelRelease();
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
            Collider2D coll = Physics2D.OverlapPoint(finger.screenPosition);
            if (coll != null)
            {
                if (coll.gameObject.name == "AbilityCancel")
                {
                    _currentTouchedAbility.AbilityCancel();
                    _currentTouchedAbility = null;
                }
                else
                {
                    Vector3 fingerPosition = Camera.main.ScreenToWorldPoint(finger.screenPosition);
                    fingerPosition.z = 0;
                    _currentTouchedAbility.ReleaseIndicator(fingerPosition);
                }
            }
            else
            {
                Vector3 fingerPosition = Camera.main.ScreenToWorldPoint(finger.screenPosition);
                fingerPosition.z = 0;
                _currentTouchedAbility.ReleaseIndicator(fingerPosition);
                _currentTouchedAbility = null;
            }
        }
    }

    private void OnEntityTouch(Entity entity)
    {
        if (_player.IsAgainst(entity))
        {
            _player.TargetEnemy(entity);
        }
        UIManager.Instance.ShowUIEntityStats(entity.gameObject);
    }

    private IEnumerator WaitForHold(Finger finger)
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
            Debug.Log("held for 1s");

        }
    }
}
