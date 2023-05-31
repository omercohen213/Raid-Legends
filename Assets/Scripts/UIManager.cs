using System.Collections;
using System.Collections.Generic;
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

    public void ShowUIEntityStats(GameObject gameObject)
    {
        Entity entity = gameObject.GetComponent<Entity>();
        _hpText.text = entity.Hp + " / " + entity.MaxHp;
        float hpRatio = (float)entity.Hp / entity.MaxHp;
        _hpBar.localScale = new Vector3(hpRatio, 1, 1);        
        _entityStats.SetActive(true);
    }

    public void HideUIEntityStats()
    {
        _entityStats.SetActive(false);
    }
}
