using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class TouchManager : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private GameObject _touchPrefab;
    private PlayerInput _playerInput;
    private InputAction _screenTouchAction;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _screenTouchAction = _playerInput.actions.FindAction("ScreenTouch");
    }

    private void OnEnable()
    {
        _screenTouchAction.performed += CheckTouchPosition;
    }

    private void OnDisable()
    {
        _screenTouchAction.performed -= CheckTouchPosition;
    }

    private void Start()
    {
    }

    private void CheckTouchPosition(InputAction.CallbackContext context)
    {
        foreach (var touch in Touchscreen.current.touches)
        {
            if (touch.phase.ReadValue() == TouchPhase.Began)
            {
                //touchCircle = Instantiate(touchPrefab, touch.position.ReadValue(), Quaternion.identity, GameObject.Find("UI").transform);              
                Collider2D coll = Physics2D.OverlapPoint(touch.position.ReadValue());
                if (coll != null)
                {
                    if (coll.gameObject.CompareTag("AbilityUI"))
                    {
                        AbilityManager.Instance.UseAbility(coll.gameObject);
                    }
                }

                float touchRadius = 0.5f;

                Vector3 touchPosition = Camera.main.ScreenToWorldPoint(touch.position.ReadValue());
                Collider2D[] colliders = Physics2D.OverlapCircleAll(touchPosition, touchRadius);

                foreach (Collider2D collider in colliders)
                {
                    if (collider.gameObject.TryGetComponent(out Entity entityColl))
                    {
                        if (_player.IsAgainst(entityColl))
                        {
                            _player.StopTargetEnemy();
                            _player.TargetEnemy(entityColl);
                        }
                        UIManager.Instance.ShowUIEntityStats(collider.gameObject);
                        break;
                    }
                }
            }
        }
    }

    private IEnumerator WaitForTapOrHold(TouchControl touch)
    {
        float holdDuration = 1f; // Adjust this duration as needed
        float timer = 0f;

        while (timer < holdDuration && touch.isInProgress)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        if (touch.isInProgress)
        {
            //onAbility1Touch.Invoke();
        }
    }

    private bool IsEntity()
    {
        return true;
        //return (collider.gameObject.CompareTag("Minion") || collider.gameObject.CompareTag("AIPlayer") || collider.gameObject.CompareTag("Player"))
    }
}
