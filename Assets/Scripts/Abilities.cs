using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Abilities : MonoBehaviour
{
    [SerializeField] private GameObject touchPrefab;
    [SerializeField] private Player player;
    [SerializeField] private PlayerMovement playerMovement;
    private InputActionAsset playerInputActions;
    private InputAction screenPressAction;
    private InputAction screenHoldAction;
    private InputAction touchPositionAction;

    GameObject touchObject;
    private readonly UnityEvent onBasicAttackTouch = new();
    private readonly UnityEvent onAbility1Touch = new();
    private readonly UnityEvent onAbility2Touch = new();
    private readonly UnityEvent onAbility3Touch = new();

    private void Awake()
    {
        playerInputActions = GetComponent<PlayerInput>().actions;
        touchPositionAction = playerInputActions.FindAction("TouchPosition");
        screenPressAction = playerInputActions.FindAction("ScreenPress");
        screenHoldAction = playerInputActions.FindAction("ScreenHold");
    }

    private void OnEnable()
    {
        screenPressAction.performed += CheckTouchPosition;
        //screenHoldAction.performed += CheckTouchPosition;
    }

    private void OnDisable()
    {
        screenPressAction.performed += CheckTouchPosition;
        //screenHoldAction.performed += CheckHoldPosition;
    }

    private void CheckHoldPosition(InputAction.CallbackContext obj)
    {
        Debug.Log(touchPositionAction.ReadValue<Vector2>());
    }

    private void Start()
    {
        onBasicAttackTouch.AddListener(BasicAttack);
        onAbility1Touch.AddListener(Ability1);
        onAbility2Touch.AddListener(Ability2);
        onAbility3Touch.AddListener(Ability3);
    }

    private void CheckTouchPosition(InputAction.CallbackContext context)
    {
        Vector2 touchPosition = touchPositionAction.ReadValue<Vector2>();
        
        RaycastHit2D hit = Physics2D.Raycast(touchPosition, Vector2.zero);
        Collider2D coll = hit.collider;

        if (coll != null && coll.gameObject.layer == LayerMask.NameToLayer("UI"))
        {
            switch (coll.gameObject.name)
            {
                case "BasicAttack":
                    onBasicAttackTouch.Invoke();
                    break;
                case "Ability1":
                    onAbility1Touch.Invoke();
                    break;
                case "Ability2":
                    onAbility2Touch.Invoke();
                    break;
                case "Ability3":
                    onAbility3Touch.Invoke();
                    break;
                default: break;
            }
        }

        /*GameObject parent = GameObject.Find("UI");
        touchObject = Instantiate(touchPrefab, touchPosition, Quaternion.identity, parent.transform);
        if (screenPressAction.phase == InputActionPhase.Performed)
            Destroy(touchObject);   */

    }

    private void BasicAttack()
    {
        playerMovement.Attack();

    }

    private void Ability1()
    {
        Debug.Log("ability1");

    }

    private void Ability2()
    {
        Debug.Log("ability2");
    }

    private void Ability3()
    {
        Debug.Log("ability3");
    }
}
