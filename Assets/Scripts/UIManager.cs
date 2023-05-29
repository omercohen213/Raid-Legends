using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    public static UIManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UIManager>();
                if (instance == null)
                {
                    GameObject singletonObject = new GameObject();
                    instance = singletonObject.AddComponent<UIManager>();
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return instance;
        }
    }

    [SerializeField] private GameObject entityStats;
    [SerializeField] Transform hpBar;
    [SerializeField] Text hpText;

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
    }

    public void ShowUIEntityStats(GameObject gameObject)
    {
        Entity entity = gameObject.GetComponent<Entity>();
        hpText.text = entity.Hp + " / " + entity.MaxHp;
        float hpRatio = (float)entity.Hp / entity.MaxHp;
        hpBar.localScale = new Vector3(hpRatio, 1, 1);        
        entityStats.SetActive(true);
    }

    public void HideUIEntityStats()
    {
        entityStats.SetActive(false);
    }
}
