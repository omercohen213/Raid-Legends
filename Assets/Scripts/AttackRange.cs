using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackRange : MonoBehaviour
{
    protected Entity targetedEntity;
    protected List<Minion> minionsInRange;
    protected List<Player> playersInRange;

    public Entity TargetedEntity { get => targetedEntity; set => targetedEntity = value; }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        targetedEntity = null;
        minionsInRange = new List<Minion>();
        playersInRange = new List<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
