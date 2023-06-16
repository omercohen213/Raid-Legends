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

    private Entity _activeTarget;
    private bool _isTargetActive;
    public bool IsTargetActive { get => _isTargetActive; set => _isTargetActive = value; }
    

    [SerializeField] private GameObject _entityStats;
    [SerializeField] Transform _hpBar;
    [SerializeField] Text _hpText;

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
        _isTargetActive= true;
        _activeTarget = entity;      
    }
    public void UpdateUIEntityStats()
    {
        _hpText.text = _activeTarget.Hp + " / " + _activeTarget.MaxHp;
        float hpRatio = (float)_activeTarget.Hp / _activeTarget.MaxHp;
        _hpBar.localScale = new Vector3(hpRatio, 1, 1);
        _entityStats.SetActive(true);
    }

    public void HideUIEntityStats()
    {
        _entityStats.SetActive(false);
        _isTargetActive = false;
        _activeTarget = null;
    }
}
