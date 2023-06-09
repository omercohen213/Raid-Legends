using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class AbilityManager : MonoBehaviour
{
    private static AbilityManager instance;

    private Player _player;
    [SerializeField] private Image _basicAttackImage;
    private readonly float _showRangeDuration = 0.2f;
    [SerializeField] private SpriteRenderer _playerRangeSpriteRenderer;
    [SerializeField] private Animator _anim;
    [SerializeField] private GameObject ice;
    private List<Ability> abilities;

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

    private void Start()
    {
        Ability[] abilityComponents = GetComponentsInChildren<Ability>();
        abilities = new List<Ability>(abilityComponents);
    }

    private void Update()
    {
        foreach (Ability ability in abilities)
        {
            if (ability.isCd)
            {
                ApplyCooldown(ability);
            }
        }
    }

    // Find ability on UI game object touch 
    public void FindAblityOnTouch(GameObject gameObject)
    {
        if (gameObject.TryGetComponent<Ability>(out var ability))
        {
            ShowAbilityTouch(ability.abilityImage);          
            _player.TryUseAbility(ability);
        }
    }

    // Use given ability
    public void UseAbility(Ability ability)
    {
        //_player.StartMovingTowardsTarget(ability);
        if (!ability.isCd)
        {
            MethodInfo methodInfo = GetType().GetMethod(ability.abilityName);
            methodInfo?.Invoke(this, new object[] { ability }); // Call ability method    
        }

    }

    // Apply ability cooldown
    private void ApplyCooldown(Ability ability)
    {
        ability.cdTimer -= Time.deltaTime;

        // Cd is over
        if (ability.cdTimer < 0)
        {
            ability.isCd = false;
            ability.cdTimer = ability.cd;
        }
        //ability.abilityCdImage.fillAmount = 0.0f;
        //ability.abilityCdImage.fillAmount = ability.cdTimer / ability.cd;
    }

    // Disable this ability use for cd time, disable all abilties for animation time
    /* private void DisableAbilityUse(Ability ability)
     {
         // To avoid using 2 abilities at the same time     
         ability.isAnimationActive = true;
         disableAllTimer = ability.animationTime;
         disableAll = true;
         ability.isCd = true;
         ability.cdTimer = ability.cd;
     }*/


    public void BasicAttack(Ability ability)
    {
        BasicAttackAnimation(ability);
        ShowPlayerRange(_showRangeDuration);
        bool isCritical = Random.value < _player.CritChance;
        _player.TargetedEntity.ReceiveDamage(_player.BaseDamage, isCritical, true);
        ability.isCd = true;
    }

    public void Ability3 (Ability ability)
    {
        ice.transform.position = _player.TargetedEntity.transform.position;
        _anim.SetTrigger("Ability3");
        _player.TargetedEntity.ReceiveDamage(_player.BaseDamage * 5, false, true);
        ability.isCd = true;
    }

    // Basic attack animation
    public void BasicAttackAnimation(Ability ability)
    {
        // check if animation in progress
        _player.Anim.SetBool("Attack", true);
        StartCoroutine(BasicAttackAnimationCancel(ability));
    }

    private IEnumerator BasicAttackAnimationCancel(Ability ability)
    {
        yield return new WaitForSeconds(ability.animationTime);
        _player.Anim.SetBool("Attack", false);
    }

    public void ShowPlayerRange(float timeBeforeHide)
    {
        _playerRangeSpriteRenderer.enabled = true;
        StartCoroutine(HideRange(timeBeforeHide));
    }
    private IEnumerator HideRange(float timeBeforeHide)
    {
        yield return new WaitForSeconds(timeBeforeHide);
        _playerRangeSpriteRenderer.enabled = false;
    }


    private void ShowAbilityTouch(Image image)
    {
        float shrinkScale = 0.8f;
        image.transform.localScale = new Vector3(shrinkScale, shrinkScale);
        StartCoroutine(ResetImage(image));
    }

    private IEnumerator ResetImage(Image image)
    {
        yield return new WaitForSeconds(_showRangeDuration);
        float normalScale = 1f;
        image.transform.localScale = new Vector3(normalScale, normalScale, 1f);
    }

    public void Ability1(Ability ability)
    {
        Debug.Log("ability1");

    }

    public void Ability2(Ability ability)
    {
        Debug.Log("ability2");
    }
}
