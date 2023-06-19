using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private readonly List<int> _xpTable = new ();

    private void Awake()
    {
        Instance = this;
        CreateXpTable();
    }

    private void CreateXpTable()
    {
        _xpTable.Add(0);
        int xpToLvlUp = 28;
        _xpTable.Add(xpToLvlUp); // Lvl 1
        for (int i = 0; i <= 17; i++)
        {
            xpToLvlUp += 100;
            _xpTable.Add(xpToLvlUp);
        }
    }

    public int GetXpToLevelUp(int lvl)
    {
        return _xpTable[lvl];
    }

}
