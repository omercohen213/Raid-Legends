using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMap : MonoBehaviour
{
    public RectTransform miniMapPanel;     // UI panel for the mini-map
    public GameObject allyIconPrefab;      // Prefab for allied icons
    public GameObject enemyIconPrefab;     // Prefab for enemy icons

    private List<Entity> _entities; // A reference to the entities list in GameManager 
    private List<Entity> _miniMapEntities; // List of current entities presented in the mini map

    private void Awake()
    {
        _miniMapEntities = new List<Entity>();
    }

    void Start()
    {
        _entities = GameManager.Instance.Entities;
        // Instantiate mini-map icons for each entity
        foreach (Entity entity in _entities)
        {
            GameObject icon = Instantiate(
                entity.IsAlly() ? allyIconPrefab : enemyIconPrefab,
                miniMapPanel
            );
            entity.MiniMapIcon = icon.GetComponent<RectTransform>();
        }
    }

    void Update()
    {
        UpdateMiniMap();
    }

    void UpdateMiniMap()
    {
        foreach (Entity entity in _entities)
        {
            if (!_miniMapEntities.Contains(entity))
            {
                _miniMapEntities.Add(entity);

                GameObject icon = Instantiate(
                entity.IsAlly() ? allyIconPrefab : enemyIconPrefab,
                miniMapPanel
            );
                entity.MiniMapIcon = icon.GetComponent<RectTransform>();
            }
        }

        foreach (Entity entity in _entities)
        {
            // Calculate position relative to the player
            Vector2 miniMapPosition = new Vector2(entity.transform.position.x, entity.transform.position.y);// * miniMapScale;

            // Update the mini-map icon position
            if (entity.MiniMapIcon != null)
            {
                entity.MiniMapIcon.anchoredPosition = miniMapPosition;
            }
        }
    }
}
