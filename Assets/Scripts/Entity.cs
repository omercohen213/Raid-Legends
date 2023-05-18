using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public enum Team { Red, Blue }

    protected int hp;

    private Team team;
    public Team EntityTeam { get => team; set => team = value; }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
