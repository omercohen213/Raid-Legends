using UnityEngine;
using UnityEngine.UI;

public class IndicatorAbility : Ability
{  
    [SerializeField] protected GameObject _indicatorPrefab;
    protected GameObject _indicator;
    protected Vector3 _initialIndicatorPosition;    

    protected GameObject _abilityCancelGo;
    private Image _abilityCancelImage;

    public GameObject IndicatorPrefab { get => _indicatorPrefab; set => _indicatorPrefab = value; }

    protected override void Awake()
    {
        base.Awake();
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

    protected override void Start()
    {
        base.Start();
        _abilityCancelGo.SetActive(false);
    }

    public override void OnAbilityTouch(Vector3 fingerPosition)
    {
        _player.UpdateAttackRange(_range);
        _abilityCancelGo.SetActive(true);
        UIManager.Instance.ShrinkAbilityImage(_abilityImage, _abilityCdImage);
        _player.ShowPlayerRange();
    }

    // Show indicator according to player's range
    public virtual void MoveIndicator(Vector3 fingerPosition)
    {
        // override
    }

    protected float GetIndicatorAngle(Vector3 fingerPosition)
    {
        Vector3 direction = fingerPosition - _initialIndicatorPosition;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        return angle;
    }

    public virtual void ReleaseIndicator(Vector3 fingerPosition)
    {
        UIManager.Instance.ResetAbilityImage(_abilityImage, _abilityCdImage);
        _player.HidePlayerRange();
        _abilityCancelGo.SetActive(false);
        _indicator.GetComponent<SpriteRenderer>().enabled = false;
    }  

    public void AbilityCancel()
    {
        _player.HidePlayerRange();
        _indicator.GetComponent<SpriteRenderer>().enabled = false;
        UIManager.Instance.ResetAbilityImage(_abilityImage, _abilityCdImage);
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
}
