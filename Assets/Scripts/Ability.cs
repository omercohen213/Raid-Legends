using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UI;

public class Ability : MonoBehaviour
{
    protected string _abilityName;
    [SerializeField] protected float _cd;
    [SerializeField] protected float _cdTimer;
    [SerializeField] protected bool _isCd;
    [SerializeField] protected float range;
    [SerializeField] protected int _baseDamage;
    [SerializeField] protected float _damageScaling; // damage multiplyer per level
    [SerializeField] protected int _level;
    [SerializeField] protected float _animationTime;
    protected Image _abilityImage;
    protected Image _abilityCdImage;
    [SerializeField] protected GameObject _abilityObject;
    [SerializeField] protected bool isTargetNeeded;
    [SerializeField] protected bool hasIndicator;
    [SerializeField] protected GameObject indicatorPrefab;
    protected Player _player;
    [SerializeField] protected Animator _anim;


    public bool IsTargetNeeded { get => isTargetNeeded; set => isTargetNeeded = value; }
    public GameObject IndicatorPrefab { get => indicatorPrefab; set => indicatorPrefab = value; }
    public bool HasIndicator { get => hasIndicator; set => hasIndicator = value; }
    public float Range { get => range; set => range = value; }

    private void Awake()
    {
        if (!GameObject.Find("Player").TryGetComponent(out _player))
        {
            Debug.LogError("Missing Player object");
        }
        _abilityName = gameObject.name;

        string imagePath = "Abilities/"+ _abilityName+ "/Image";
        if (!GameObject.Find(imagePath).TryGetComponent(out _abilityImage))
        {
            Debug.LogError("Missing Image");
        }
        string CdImagePath = "Abilities/"+ _abilityName+ "/CDImage";
        if (!GameObject.Find(CdImagePath).TryGetComponent(out _abilityCdImage))
        {
            Debug.LogError("Missing CDImage");
        }
    }

    private void Start()
    {
        _cdTimer = _cd;
    }

    private void Update()
    {
        if (_isCd)
        {
            ApplyCooldown();
        }
    }

    public virtual void UseAbility()
    {
        // override
    }
    public virtual void UseAbility(Vector3 abilityPosition)
    {
        // override
    }
    public virtual void UseAbility(Finger finger)
    {
        // override
    }

    private void ApplyCooldown()
    {
        _cdTimer -= Time.deltaTime;

        // Cd is over
        if (_cdTimer < 0)
        {
            _isCd = false;
            _cdTimer = _cd;
            _abilityCdImage.fillAmount = 0.0f;
        }
        // Still on cd
        else
        {
            _abilityCdImage.fillAmount = _cdTimer / _cd;
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

    // Show touch on ability image
    public void ShrinkAbilityImage(Ability ability)
    {
        float shrinkScale = 0.8f;
        ability._abilityImage.transform.localScale = new Vector3(shrinkScale, shrinkScale);
        Color abilityImageColor = ability._abilityImage.color;
        abilityImageColor.a = 150f;
        ability._abilityImage.color = abilityImageColor;

        ability._abilityCdImage.transform.localScale = new Vector3(shrinkScale, shrinkScale);
        Color cdImagecolor = ability._abilityCdImage.color;
        cdImagecolor.a = 150f;
        ability._abilityCdImage.color = cdImagecolor;
    }

    // Show normal ability image
    public void ResetAbilityImage(Ability ability)
    {
        float normalScale = 1f;
        ability._abilityImage.transform.localScale = new Vector3(normalScale, normalScale);
        Color abilityImageColor = ability._abilityImage.color;
        abilityImageColor.a = 255f;
        ability._abilityImage.color = abilityImageColor;

        ability._abilityCdImage.transform.localScale = new Vector3(normalScale, normalScale);
        Color cdImagecolor = ability._abilityCdImage.color;
        cdImagecolor.a = 255f;
        ability._abilityCdImage.color = cdImagecolor;
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