using System;
using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class AbilityManager : MonoBehaviour
{
    private static AbilityManager instance;
    
    private Player _player;
    private readonly float _transitionDuration = 0.2f;
    [SerializeField] private Image _basicAttackImage;
    public static AbilityManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<AbilityManager>();
                if (instance == null)
                {
                    GameObject singletonObject = new GameObject();
                    instance = singletonObject.AddComponent<AbilityManager>();
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        if (!GameObject.Find("Player").TryGetComponent(out _player))
        {
            Debug.LogError("Missing Player object");
        }
    }

    public void UseAbility(GameObject gameObject)
    {
        if (gameObject.TryGetComponent<Ability>(out var ability))
        {
            Debug.Log(ability.abilityName);
            MethodInfo methodInfo = GetType().GetMethod(ability.abilityName);
            methodInfo?.Invoke(this, new object[] { ability }); // Call ability method    
        }
    }

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

        
    public void BasicAttack (Ability ability)
    {
        ShowAbilityTouch();
        _player.AttackAnimation();
        _player.ShowPlayerRange(_transitionDuration);
    }

    private void ShowAbilityTouch()
    {
        float shrinkScale = 0.8f;
        _basicAttackImage.transform.localScale = new Vector3(-shrinkScale, shrinkScale, 1f);
        StartCoroutine(ResetImage());
    }

    private IEnumerator ResetImage()
    {
        yield return new WaitForSeconds(_transitionDuration);
        float normalScale = 1f;
        _basicAttackImage.transform.localScale = new Vector3(-normalScale, normalScale, 1f);
    }

    public void Ability1(Ability ability)
    {
        Debug.Log("ability1");

    }

    public void Ability2(Ability ability)
    {
        Debug.Log("ability2");
    }

    public void Ability3(Ability ability)
    {
        Debug.Log("ability3");
    }

    
}
