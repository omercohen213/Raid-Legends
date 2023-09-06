using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Ability : MonoBehaviour
{
    protected string _abilityName;
    protected Image _abilityImage;
    protected Image _abilityCdImage;
    protected TextMeshProUGUI _abilityCdText;
    protected TextMeshProUGUI _abilityLevelText;

    [SerializeField] protected bool _isPlayerAbility;
    [SerializeField] protected float _cd;
    protected float _cdTimer;
    protected bool _isCd;

    [SerializeField] protected GameObject _abilityObject;

    [SerializeField] protected float _range;
    [SerializeField] protected int _baseDamage;
    [SerializeField] protected float _damageScaling; // damage multiplyer per level
    protected int _level;

    protected Player _player;
    protected Entity _caster;
    [SerializeField] protected Animator _anim;

    public float Range { get => _range; set => _range = value; }

    protected virtual void Awake()
    {
        if (_isPlayerAbility)
        {
            if (!GameObject.Find("Player").TryGetComponent(out _player))
            {
                Debug.LogError("Missing Player object", this);
            }

            _abilityName = gameObject.name;
            string imagePath = "Abilities/" + _abilityName + "/Image";
            if (!GameObject.Find(imagePath).TryGetComponent(out _abilityImage))
            {
                Debug.LogError("Missing Image", this);
            }
            string cdImagePath = "Abilities/" + _abilityName + "/CDImage";
            if (!GameObject.Find(cdImagePath).TryGetComponent(out _abilityCdImage))
            {
                Debug.LogError("Missing CDImage", this);
            }
            string cdTextPath = "Abilities/" + _abilityName + "/CDText";
            if (!GameObject.Find(cdTextPath).TryGetComponent(out _abilityCdText))
            {
                Debug.LogError("Missing CDText", this);
            }
            if (this is not BasicAttack)
            {
                string levelTextPath = "Abilities/" + _abilityName + "/Level/Text";
                if (!GameObject.Find(levelTextPath).TryGetComponent(out _abilityLevelText))
                {
                    Debug.LogError("Missing level text", this);
                }
            }
        }
    }

    protected virtual void Start()
    {
        _level = 1;
        _cdTimer = _cd;

        if (_isPlayerAbility)
        {
            _abilityCdText.enabled = false;
        }
    }

    protected virtual void Update()
    {
        if (_isCd)
        {
            ApplyCooldown();
        }
    }

    public virtual void CastAbility(Vector3 abilityPosition, Entity caster)
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
            if (_isPlayerAbility)
            {
                _abilityCdImage.fillAmount = 0.0f;
                _abilityCdText.enabled = false;
            }

        }
        // Still on cd
        else
        {
            if (_isPlayerAbility)
            {
                _abilityCdImage.fillAmount = _cdTimer / _cd;
                _abilityCdText.text = _cdTimer.ToString("F1");
                if (_abilityName != "BasicAttack")
                {

                    _abilityCdText.enabled = true;
                }
            }
        }
    }

    public virtual void OnAbilityTouch(Vector3 fingerPosition)
    {
        _player.UpdateAttackRange(_range);
        float touchTransitionDuration = 0.2f;
        UIManager.Instance.ShrinkAbilityImage(_abilityImage, _abilityCdImage, touchTransitionDuration);
        _player.ShowPlayerRange(touchTransitionDuration);
        _player.TryUseAbility(this);
    }

    public virtual void LevelUpAbility()
    {
        _level++;
        _abilityLevelText.text = _level.ToString();
        _baseDamage = Mathf.RoundToInt(_baseDamage * _damageScaling);
        UIManager.Instance.HideLevelUpAbility();
    }
}