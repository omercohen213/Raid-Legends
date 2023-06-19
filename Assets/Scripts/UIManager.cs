using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<UIManager>();
                if (_instance == null)
                {
                    GameObject singletonObject = new();
                    _instance = singletonObject.AddComponent<UIManager>();
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return _instance;
        }
    }

    private Player _player;
    private Entity _activeTarget;
    private bool _isTargetActive;

    [SerializeField] private GameObject _entityStats;
    [SerializeField] private Transform _hpBar;
    [SerializeField] private TextMeshProUGUI _hpText;
    [SerializeField] private TextMeshProUGUI _goldText;

    [SerializeField] private GameObject _recallProgressBar;
    public bool IsTargetActive { get => _isTargetActive; set => _isTargetActive = value; }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        if (!GameObject.Find("Player").TryGetComponent(out _player))
        {
            Debug.LogError("Missing Player object", this);
        }
    }

    private void Start()
    {
        _goldText.text = _player.StartingGold.ToString();
    }

    private void Update()
    {
        if (_isTargetActive)
        {
            UpdateUIEntityStats();
        }
    }

    public void ShowUIEntityStats(GameObject gameObject)
    {
        Entity entity = gameObject.GetComponent<Entity>();
        _activeTarget = entity;
        _isTargetActive = true;
    }
    public void UpdateUIEntityStats()
    {
        _entityStats.SetActive(true);
        _hpText.text = _activeTarget.Hp + " / " + _activeTarget.MaxHp;
        float hpRatio = (float)_activeTarget.Hp / _activeTarget.MaxHp;
        _hpBar.localScale = new Vector3(hpRatio, 1, 1);
    }

    public void HideUIEntityStats()
    {
        _entityStats.SetActive(false);
        _isTargetActive = false;
        _activeTarget = null;
    }

    public void ShowRecallProgress()
    {
        _recallProgressBar.SetActive(true);
    }
    public void HideRecallProgress()
    {
        _recallProgressBar.SetActive(false);
    }

    // Show touch on ability image
    public void ShrinkAbilityImage(Image abilityImage, Image abilityCdImage)
    {
        float shrinkScale = 0.8f;
        abilityImage.transform.localScale = new Vector3(shrinkScale, shrinkScale);
        Color abilityImageColor = abilityImage.color;
        abilityImageColor.a = 150f;
        abilityImage.color = abilityImageColor;

        abilityCdImage.transform.localScale = new Vector3(shrinkScale, shrinkScale);
        Color cdImagecolor = abilityCdImage.color;
        cdImagecolor.a = 150f;
        abilityCdImage.color = cdImagecolor;
    }
    // Show normal ability image
    public void ResetAbilityImage(Image abilityImage, Image abilityCdImage)
    {
        float normalScale = 1f;
        abilityImage.transform.localScale = new Vector3(normalScale, normalScale);
        Color abilityImageColor = abilityImage.color;
        abilityImageColor.a = 255f;
        abilityImage.color = abilityImageColor;

        abilityCdImage.transform.localScale = new Vector3(normalScale, normalScale);
        Color cdImagecolor = abilityCdImage.color;
        cdImagecolor.a = 255f;
        abilityCdImage.color = cdImagecolor;
    }
    // Shrink image for transitionDuration and then show normal image
    public void ShrinkAbilityImage(Image abilityImage, Image abilityCdImage, float transitionDuration)
    {
        ShrinkAbilityImage(abilityImage, abilityCdImage);
        StartCoroutine(ResetAbilityImage(abilityImage, abilityCdImage, transitionDuration));

    }
    private IEnumerator ResetAbilityImage(Image abilityImage, Image abilityCdImage, float transitionDuration)
    {
        yield return new WaitForSeconds(transitionDuration);
        ResetAbilityImage(abilityImage, abilityCdImage);
    }

    private void SetActiveLevelUpAbility(bool value)
    {
        GameObject abilitiesParent = GameObject.Find("Abilities");
        if (abilitiesParent != null)
        {
            foreach (Transform ability in abilitiesParent.transform)
            {
                foreach (Transform abilityProperty in ability)
                {
                    if (abilityProperty.name == "LevelUpAbility")
                    {
                        abilityProperty.gameObject.SetActive(value);
                    }
                }
            }
        }
    }

    public void ShowLevelUpAbility()
    {
        SetActiveLevelUpAbility(true);
    }

    public void HideLevelUpAbility()
    {
        SetActiveLevelUpAbility(false);
    }

    public void UpdateGoldUI()
    {
        _goldText.text = _player.Gold.ToString();
    }
}
