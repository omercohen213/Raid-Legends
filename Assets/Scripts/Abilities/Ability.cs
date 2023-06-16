using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Ability : MonoBehaviour
{
    protected string _abilityName;
    protected Image _abilityImage;
    protected Image _abilityCdImage;
    protected Text _abilityCdText;

    [SerializeField] protected float _cd;
    protected float _cdTimer;
    protected bool _isCd;

    [SerializeField] protected GameObject _abilityObject;
    [SerializeField] protected float _range;
    [SerializeField] protected int _baseDamage;
    [SerializeField] protected float _damageScaling; // damage multiplyer per level
    protected int _level;

    protected Player _player;
    [SerializeField] protected Animator _anim;

    public float Range { get => _range; set => _range = value; }

    protected virtual void Awake()
    {
        if (!GameObject.Find("Player").TryGetComponent(out _player))
        {
            Debug.LogError("Missing Player object");
        }
        _abilityName = gameObject.name;

        string imagePath = "Abilities/" + _abilityName + "/Image";
        if (!GameObject.Find(imagePath).TryGetComponent(out _abilityImage))
        {
            Debug.LogError("Missing Image");
        }
        string CdImagePath = "Abilities/" + _abilityName + "/CDImage";
        if (!GameObject.Find(CdImagePath).TryGetComponent(out _abilityCdImage))
        {
            Debug.LogError("Missing CDImage");
        }
        string CdTextPath = "Abilities/" + _abilityName + "/CDText";
        if (!GameObject.Find(CdTextPath).TryGetComponent(out _abilityCdText))
        {
            Debug.LogError("Missing CDText");
        }      
    }

    protected virtual void Start()
    {
        _level = 1;
        _cdTimer = _cd;       
        _abilityCdText.enabled = false;
    }

    protected virtual void Update()
    {
        if (_isCd)
        {
            ApplyCooldown();
        }
    }

    public virtual void UseAbility(Vector3 abilityPosition)
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
            _abilityCdText.enabled = false;

        }
        // Still on cd
        else
        {
            _abilityCdImage.fillAmount = _cdTimer / _cd;
            _abilityCdText.text = _cdTimer.ToString("F1");
            if (_abilityName != "BasicAttack")
            {
                _abilityCdText.enabled = true;
            }
        }
    }

    public virtual void OnAbilityTouch(Vector3 fingerPosition)
    {
        _player.UpdateAttackRange(_range);
        float transitionDuration = 0.2f;
        UIManager.Instance.ShrinkAbilityImage(_abilityImage, _abilityCdImage, transitionDuration);
        _player.ShowPlayerRange(transitionDuration);
        _player.TryUseAbility(this);
    }
}