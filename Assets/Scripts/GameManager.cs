using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private List<Entity> _entities; // A list to track all entities in the game
    private List<int> _xpTable;

    public List<Entity> Entities { get => _entities; set => _entities = value; }

    private void Awake()
    {
        _entities = new List<Entity>();
        _xpTable = new List<int> ();
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

    // Add entity to entities list
    public void AddEntity(Entity entity)
    {
        _entities.Add(entity);
    }

    // Remove entity from entities list
    public void RemoveEntity(Entity entity)
    {
        _entities.Remove(entity);
    }
}
