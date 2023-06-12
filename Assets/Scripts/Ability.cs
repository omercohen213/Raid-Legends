using System;
using UnityEngine;
using UnityEngine.UI;

public class Ability: MonoBehaviour
{ 
    public string abilityName;
    public float cd;
    public float cdTimer;
    public bool isCd;
    public float range;
    public int baseDamage;
    public float damageScaling; // damage multiplyer per level
    public int level;
    public float animationTime;
    // public bool isAnimationActive;
    public Image abilityImage;
    public Image abilityCdImage;
    public bool isOnHoldTouch;
    public bool isTargetNeeded;
    public bool hasIndicator;
    public GameObject indicatorPrefab;
}