using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerAttack : MonoBehaviour
{
    private Tower tower;

    [SerializeField] private Transform focusStartingPoint;
    [SerializeField] private LineRenderer focusLine;
    [SerializeField] private float damageRamp;


    private SpriteRenderer rangeSpriteRenderer;
    private Transform currentFocusedEntity;
    private List<Collider2D> minionsInRange;
    private List<Collider2D> playersInRange;


    private void Awake()
    {
        if (!TryGetComponent(out rangeSpriteRenderer))
        {
            Debug.LogError("RangeSpriteRenderer component not found!");
        }
        tower = GetComponentInParent<Tower>();
        if (tower == null)
        {
            Debug.LogError("Tower component not found!");
        }
    }

    private void Start()
    {
        minionsInRange = new List<Collider2D>();
        playersInRange = new List<Collider2D>();
        focusLine.SetPosition(0, focusStartingPoint.position);
    }

    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.layer == LayerMask.NameToLayer("Player")) // or minion
        {
            Entity collEntity = coll.GetComponent<Entity>();
            if (tower.IsAgainst(collEntity))
            {
                if (coll.CompareTag("Player"))
                {
                    playersInRange.Add(coll);
                }
                else if (coll.CompareTag("Minion"))
                {
                    Debug.Log("Minion in range");
                }
                else Debug.LogError("Unrecognized entity", coll.gameObject);

                // If there is no currently focused entity, focus on the first entity in range
                if (currentFocusedEntity == null)
                {
                    Attack(coll.transform);
                    Debug.Log(currentFocusedEntity);
                }
            }
        }
    }


    private void OnTriggerExit2D(Collider2D coll)
    {

        if (coll.gameObject.layer == LayerMask.NameToLayer("Player")) // or minion
        {
            if (coll.CompareTag("Player"))
            {
                playersInRange.Remove(coll);
            }
            else if (coll.CompareTag("Minion"))
                minionsInRange.Remove(coll);

            // If the focused player leaves the range, switch focus to the next entity 
            if (coll.transform == currentFocusedEntity)
            {
                if (minionsInRange.Count > 0)
                {
                    Attack(minionsInRange[0].transform);
                }
                else if (playersInRange.Count > 0)
                {
                    Attack(playersInRange[0].transform);
                    Debug.Log("switching taget", playersInRange[0].transform);
                }
                else
                {
                    StopAttack();
                    Debug.Log("stopping attack");
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D coll)
    {
        if (currentFocusedEntity != null)
        {
            focusLine.SetPosition(1, currentFocusedEntity.transform.position);

        }
    }

    private void Attack(Transform entityTransform)
    {
        focusLine.enabled = true;
        rangeSpriteRenderer.enabled = true;
        currentFocusedEntity = entityTransform;
    }

    private void StopAttack()
    {
        currentFocusedEntity = null;
        focusLine.enabled = false;
        rangeSpriteRenderer.enabled = false;
    }
}

