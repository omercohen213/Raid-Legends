using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Reflection;
using Unity.VisualScripting.AssemblyQualifiedNameParser;
using UnityEngine;
using UnityEngine.UI;

public class AbilityManager : MonoBehaviour
{
    private static AbilityManager instance;

    private Player _player;
    [SerializeField] private Image _basicAttackImage;
    private readonly float _showRangeDuration = 0.2f;
    [SerializeField] private Animator _anim;
    [SerializeField] private GameObject ice;
    [SerializeField] private GameObject _empoweredMinionPrefab; 
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

    // Use given ability
    public void UseAbility(Ability ability, Vector3 abilityPosition)
    {
        //_player.StartMovingTowardsTarget(ability);
        if (!ability.isCd)
        {
            MethodInfo methodInfo = GetType().GetMethod(ability.abilityName);
            methodInfo?.Invoke(this, new object[] { ability, abilityPosition }); // Call ability method    
            ability.isCd = true;
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
            //ability.abilityCdText.gameObject.SetActive(false);
            ability.abilityCdImage.fillAmount = 0.0f;
        }
        // Still on cd
        else
        {
            //ability.abilityCdText.text = Mathf.RoundToInt(ability.cdTimer).ToString();
            ability.abilityCdImage.fillAmount = ability.cdTimer / ability.cd;
        }
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


    public void BasicAttack(Ability ability, Vector3 abilityPosition)
    {
        BasicAttackAnimation(ability);
        _player.ShowPlayerRange(_showRangeDuration);
        bool isCritical = Random.value < _player.CritChance;
        _player.TargetedEntity.ReceiveDamage(_player.BaseDamage, isCritical, true);
    }

    public void Ability3 (Ability ability , Vector3 abilityPosition)
    {
        ice.transform.position = abilityPosition;
        _anim.SetTrigger("Ability3");
        int abilityDamage = Mathf.RoundToInt(ability.baseDamage + ability.level * ability.baseDamage * ability.damageScaling);
        _player.TargetedEntity.ReceiveDamage(abilityDamage, false, true);
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

    public void Ability1(Ability ability, Vector3 abilityPosition)
    {
        Debug.Log("ability1");

    }

    public void Ability2(Ability ability, Vector3 abilityPosition)
    {
        GameObject empMinionGo = Instantiate(_empoweredMinionPrefab, abilityPosition, Quaternion.identity);
        Minion empMinion = empMinionGo.GetComponent<Minion>();
    }

    // Show touch on ability image
    public void ShrinkAbilityImage(Ability ability)
    {
        float shrinkScale = 0.8f;
        ability.abilityImage.transform.localScale = new Vector3(shrinkScale, shrinkScale);
        Color abilityImageColor = ability.abilityImage.color;
        abilityImageColor.a = 150f;
        ability.abilityImage.color = abilityImageColor;

        ability.abilityCdImage.transform.localScale = new Vector3(shrinkScale, shrinkScale);
        Color cdImagecolor = ability.abilityCdImage.color;
        cdImagecolor.a = 150f;
        ability.abilityCdImage.color = cdImagecolor;
    }

    // Show normal ability image
    public void ResetAbilityImage(Ability ability)
    {
        float normalScale = 1f;
        ability.abilityImage.transform.localScale = new Vector3(normalScale, normalScale);
        Color abilityImageColor = ability.abilityImage.color;
        abilityImageColor.a = 255f;
        ability.abilityImage.color = abilityImageColor;

        ability.abilityCdImage.transform.localScale = new Vector3(normalScale, normalScale);
        Color cdImagecolor = ability.abilityCdImage.color;
        cdImagecolor.a = 255f;
        ability.abilityCdImage.color = cdImagecolor;
    }

    // Shrink image for transitionDuration and then show normal image
    public void ShrinkAbilityImage(Ability ability, float transitionDuration)
    {
        ShrinkAbilityImage(ability);
        StartCoroutine(ResetAbilityImage(ability, transitionDuration));

    }
    private IEnumerator ResetAbilityImage(Ability ability, float transitionDuration)
    {
        yield return new WaitForSeconds(transitionDuration);
        ResetAbilityImage(ability);
    }
}
