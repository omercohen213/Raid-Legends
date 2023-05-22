using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Abilities : MonoBehaviour
{
    [SerializeField] private GameObject touchPrefab;
    [SerializeField] private Player player;
    [SerializeField] private Image basicAttackImage;   
    [SerializeField] private Ability basicAttack;
    private float transitionDuration = 0.2f;

    private InputActionAsset playerInputActions;
    private InputAction screenPressAction;
    private InputAction screenHoldAction;
    private InputAction touchPositionAction;

    //GameObject touchObject;
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
        screenPressAction.performed -= CheckTouchPosition;
        //screenHoldAction.performed += CheckHoldPosition;
    }

    private void Update()
    {
        //screenPressAction.started += CheckTouchPosition;
        //screenPressAction. canceled-= CheckTouchPosition;
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
        float touchValue = context.ReadValue<float>();

        if (touchValue > 0.0f)
        {
            Vector2 touchPosition = touchPositionAction.ReadValue<Vector2>();

            RaycastHit2D hit = Physics2D.Raycast(touchPosition, Vector2.zero);
            Collider2D coll = hit.collider;

            if (coll != null && coll.gameObject.layer == LayerMask.NameToLayer("UI"))
            {
                /* if (!basicAttack.isCd) // Ability is not on cd and no ability animation is active
                 {
                     onBasicAttackTouch.Invoke();
                     DisableAbilityUse(basicAttack);
                 }
                 else if (basicAttack.isCd) // Ability on cd
                     ApplyCooldown(basicAttack);*/

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
        }
    }


    /*GameObject parent = GameObject.Find("UI");
    touchObject = Instantiate(touchPrefab, touchPosition, Quaternion.identity, parent.transform);
    if (screenPressAction.phase == InputActionPhase.Performed)
        Destroy(touchObject);   */

    private void ApplyCooldown(Ability ability)
    {
        /*if (ability.isAnimationActive)
        {
            disableAllTimer -= Time.deltaTime;
            if (disableAllTimer < 0) // Animation has ended and we can use another ability
            {
                ability.isAnimationActive = false;
                disableAll = false;
                disableAllTimer = 0;
            }
        }*/
        // Reduce cd till it reaches 0 so we can use it again
        ability.cdTimer -= Time.deltaTime;

        // Cd is over
        if (ability.cdTimer < 0)
        {
            ability.isCd = false;
            ability.cdTimer = 0;
            //ability.abilityCdImage.fillAmount = 0.0f;
        }
        // Still on cd
        else
        {
            //ability.abilityCdImage.fillAmount = ability.cdTimer / ability.cd;
        }
    }

    // Disable this ability use for cd time, disable all abilties for animation time
    private void DisableAbilityUse(Ability ability)
    {
        // To avoid using 2 abilities at the same time     
        /*        ability.isAnimationActive = true;
                disableAllTimer = ability.animationTime;
                disableAll = true;*/
        ability.isCd = true;
        ability.cdTimer = ability.cd;
    }

    private void BasicAttack()
    {     
        Debug.Log("BasicAttack");
        ShowTouch();
        player.AttackAnimation();
        player.ShowPlayerRange(transitionDuration);         
    }    

    private void ShowTouch()
    {
        float shrinkScale = 0.8f;
        basicAttackImage.transform.localScale = new Vector3(-shrinkScale, shrinkScale, 1f);
        StartCoroutine(ResetImage());
    }

    private IEnumerator ResetImage()
    {
        yield return new WaitForSeconds(transitionDuration);
        float normalScale = 1f;
        basicAttackImage.transform.localScale = new Vector3(-normalScale, normalScale, 1f);
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
