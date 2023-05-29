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
    [SerializeField] private Player player;
    [SerializeField] private GameObject touchPrefab;
    private PlayerInput playerInput;
    private InputAction screenTouchAction;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        screenTouchAction = playerInput.actions.FindAction("ScreenTouch");
    }

    private void OnEnable()
    {
        screenTouchAction.performed += CheckTouchPosition;
    }

    private void OnDisable()
    {
        screenTouchAction.performed -= CheckTouchPosition;
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
                Collider2D worldSpcaeColl = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(touch.position.ReadValue()));
                if (worldSpcaeColl != null)
                {
                    if (worldSpcaeColl.gameObject.CompareTag("Minion") || worldSpcaeColl.gameObject.CompareTag("AIPlayer") || worldSpcaeColl.gameObject.CompareTag("Player"))
                    {
                        player.TargetEnemy(worldSpcaeColl.gameObject.GetComponent<Entity>());
                        UIManager.Instance.ShowUIEntityStats(worldSpcaeColl.gameObject);
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


}
