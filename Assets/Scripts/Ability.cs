using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
[CreateAssetMenu(fileName = "ScriptableObjects", menuName = "ScriptableObjects/Ability")]
public class Ability : ScriptableObject
{
    public string abilityName;
    public float cd;
    public float cdTimer = 0;
    public bool isCd = false;
    public float animationTime;
    public bool isAnimationActive = false;
    public Image abilityCdImage;
    public Text abilityCdText;
}