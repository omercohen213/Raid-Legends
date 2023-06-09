using System;
using UnityEngine;
using UnityEngine.UI;

public class Ability: MonoBehaviour
{ 
    public string abilityName;
    public float cd;
    public float cdTimer = 0;
    public bool isCd = false;
    public float range;
    public float animationTime;
    public bool isAnimationActive = false;
    public Image abilityImage;
    public Image abilityCdImage;
}