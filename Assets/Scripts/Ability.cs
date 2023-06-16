using System;
using System.Collections;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
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
    // protected GameObject _abilityObjectPosition;
    [SerializeField] protected bool _hasIndicator;
    [SerializeField] protected bool _isDirectionIndicator; // Show direction indicator of the ability
    [SerializeField] protected bool _isOnPointIndicator; // Show point indicator where the ability object spawns
    [SerializeField] protected GameObject _indicatorPrefab;
    protected GameObject _indicator;
    protected Vector3 _initialIndicatorPosition;
    protected readonly Vector3 _directionOffset = new(0.17f, -0.35f);

    private GameObject _abilityCancelGo;
    private Image _abilityCancelImage;

    [SerializeField] protected float _range;
    [SerializeField] protected int _baseDamage;
    [SerializeField] protected float _damageScaling; // damage multiplyer per level
    [SerializeField] protected int _level;
    [SerializeField] protected float _animationTime;

    protected Player _player;
    [SerializeField] protected Animator _anim;

    public GameObject IndicatorPrefab { get => _indicatorPrefab; set => _indicatorPrefab = value; }
    public bool HasIndicator { get => _hasIndicator; set => _hasIndicator = value; }
    public float Range { get => _range; set => _range = value; }

    private void Awake()
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

        _abilityCancelGo = GameObject.Find("AbilityCancel");
        if (_abilityCancelGo == null)
        {
            Debug.LogError("Missing Ability Cancel");
        }
        else
        {
            _abilityCancelImage = _abilityCancelGo.GetComponentInChildren<Image>();
        }
    }

    protected virtual void Start()
    {
        _cdTimer = _cd;
        _abilityCancelGo.SetActive(false);
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


    public void OnAbilityTouch(Vector3 fingerPosition)
    {
        _player.UpdateAttackRange(_range);

        // If ability has indicator, instantiate it and wait for touch release
        if (_hasIndicator)
        {
            if (_isDirectionIndicator)
            {
                _initialIndicatorPosition = _player.transform.position + _directionOffset;
                float angle = GetIndicatorAngle(fingerPosition);

                string indicatorObjectName = _indicatorPrefab.name;
                GameObject existingIndicator = GameObject.Find("AbilityObjects/" + indicatorObjectName);
                if (existingIndicator != null)
                {
                    _indicator = existingIndicator;
                    _indicator.transform.SetPositionAndRotation(_player.transform.position + _directionOffset, Quaternion.Euler(0f, 0f, angle));
                    _indicator.GetComponent<SpriteRenderer>().enabled = true;
                }
                else
                {
                    _indicator = Instantiate(_indicatorPrefab, _initialIndicatorPosition, Quaternion.Euler(0f, 0f, angle), GameObject.Find("AbilityObjects").transform);
                    _indicator.name = indicatorObjectName;
                }

                _indicator.transform.localScale = new Vector3(1f, _player.AttackRange.radius / 3.8f);
            }
            else if (_isOnPointIndicator)
            {
                _initialIndicatorPosition = GetClosestPointToPlayerRange(fingerPosition);
                _indicator = Instantiate(_indicatorPrefab, _initialIndicatorPosition, Quaternion.identity, GameObject.Find("AbilityObjects").transform);
            }

            _abilityCancelGo.SetActive(true);
            ShrinkAbilityImage();
            _player.ShowPlayerRange();
        }

        // Else use the ability immediately and show touch and range for a brief moment
        else
        {
            float transitionDuration = 0.2f;
            ShrinkAbilityImage(transitionDuration);
            _player.ShowPlayerRange(transitionDuration);
            _player.TryUseAbility(this);
        }
    }

    // Show indicator according to player's range
    public void MoveIndicator(Vector3 fingerPosition)
    {
        if (_isDirectionIndicator)
        {
            float angle = GetIndicatorAngle(fingerPosition);
            _indicator.transform.SetPositionAndRotation(_player.transform.position + _directionOffset, Quaternion.Euler(0f, 0f, angle));
        }
        else if (_isOnPointIndicator)
        {
            float distanceToPlayer = GetDistanceFromPlayer(fingerPosition);

            // Check if the finger position is within the ability range
            if (distanceToPlayer <= _range)
            {
                _indicator.transform.position = fingerPosition;
            }

            else
            {
                Vector3 indicatorPosition = GetClosestPointToPlayerRange(fingerPosition);
                _indicator.transform.position = indicatorPosition;
            }
        }
    }

    private float GetIndicatorAngle(Vector3 fingerPosition)
    {
        Vector3 direction = fingerPosition - _initialIndicatorPosition;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        return angle;
    }

    public void ReleaseIndicator(Vector3 fingerPosition)
    {
        if (_isDirectionIndicator)
        {
            UseAbility(fingerPosition);
        }
        else if (_isOnPointIndicator)
        {
            Vector3 indicatorPos = _indicator.transform.position;
            UseAbility(indicatorPos);

        }
        ResetAbilityImage();
        _player.HidePlayerRange();
        _abilityCancelGo.SetActive(false);
        _indicator.GetComponent<SpriteRenderer>().enabled = false;
    }

    private float GetDistanceFromPlayer(Vector3 fingerPosition)
    {
        Vector3 playerPosition = _player.transform.position;
        Vector3 directionToPlayer = fingerPosition - playerPosition;
        float distanceToPlayer = directionToPlayer.magnitude;
        return distanceToPlayer;
    }

    private Vector3 GetClosestPointToPlayerRange(Vector3 fingerPosition)
    {
        Vector3 playerPosition = _player.transform.position;
        Vector3 directionToPlayer = fingerPosition - playerPosition;
        Vector3 closestPoint = playerPosition + (directionToPlayer.normalized * _range);
        return closestPoint;
    }

    public void AbilityCancel()
    {
        _player.HidePlayerRange();
        _indicator.GetComponent<SpriteRenderer>().enabled = false;
        ResetAbilityImage();
        _abilityCancelGo.SetActive(false);
    }

    public void AbilityCancelHover()
    {
        Color color = new() { r = 255f, g = 0f, b = 0f, a = 0.39f };
        _abilityCancelImage.color = color;
        float expandScale = 1.4f;
        _abilityCancelImage.transform.localScale = new Vector3(expandScale, expandScale);
        _indicator.GetComponent<SpriteRenderer>().color = color;
        _player.HidePlayerRange();
    }

    public void AbilityCancelRelease()
    {
        Color color = new() { r = 0f, g = 255f, b = 255f, a = 0.39f };
        _abilityCancelImage.color = color;
        float normalScale = 1f;
        _abilityCancelImage.transform.localScale = new Vector3(normalScale, normalScale);
        _indicator.GetComponent<SpriteRenderer>().color = color;
        _player.ShowPlayerRange();
    }

    // Show touch on ability image
    public void ShrinkAbilityImage()
    {
        float shrinkScale = 0.8f;
        _abilityImage.transform.localScale = new Vector3(shrinkScale, shrinkScale);
        Color abilityImageColor = _abilityImage.color;
        abilityImageColor.a = 150f;
        _abilityImage.color = abilityImageColor;

        _abilityCdImage.transform.localScale = new Vector3(shrinkScale, shrinkScale);
        Color cdImagecolor = _abilityCdImage.color;
        cdImagecolor.a = 150f;
        _abilityCdImage.color = cdImagecolor;
    }
    // Show normal ability image
    public void ResetAbilityImage()
    {
        float normalScale = 1f;
        _abilityImage.transform.localScale = new Vector3(normalScale, normalScale);
        Color abilityImageColor = _abilityImage.color;
        abilityImageColor.a = 255f;
        _abilityImage.color = abilityImageColor;

        _abilityCdImage.transform.localScale = new Vector3(normalScale, normalScale);
        Color cdImagecolor = _abilityCdImage.color;
        cdImagecolor.a = 255f;
        _abilityCdImage.color = cdImagecolor;
    }
    // Shrink image for transitionDuration and then show normal image
    public void ShrinkAbilityImage(float transitionDuration)
    {
        ShrinkAbilityImage();
        StartCoroutine(ResetAbilityImage(transitionDuration));

    }
    private IEnumerator ResetAbilityImage(float transitionDuration)
    {
        yield return new WaitForSeconds(transitionDuration);
        ResetAbilityImage();
    }
}