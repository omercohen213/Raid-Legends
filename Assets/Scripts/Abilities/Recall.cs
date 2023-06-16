using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Recall : Ability
{  
    [SerializeField] private Image _recallProgress;
    [SerializeField] private TextMeshProUGUI _recallProgressText;
    private readonly float _recallTime = 5f;
    private float _recallTimer;
    private bool _isRecalling;
    private readonly Vector3 _baseCameraPosition = new(-47f, 4f, -3f);
    private readonly Vector3 _recallOffset = new(0f, -1.5f, 0f);

    protected override void Awake()
    {
        if (!GameObject.Find("Player").TryGetComponent(out _player))
        {
            Debug.LogError("Missing Player object");
        }
        _abilityName = gameObject.name;

        string imagePath = "Abilities/" + _abilityName + "/Image";
        if (!GameObject.Find(imagePath).TryGetComponent(out _abilityImage))
        {
            Debug.LogError("Missing Image");
        }
    }

    protected override void Start()
    {
        _recallTimer = _recallTime;
        
    }

    public override void OnAbilityTouch(Vector3 fingerPosition)
    {
        UIManager.Instance.ShowRecallProgress();
        UseAbility(fingerPosition);
    }

    public override void UseAbility(Vector3 abilityPosition)
    {
        _isRecalling = true;
        _abilityObject.SetActive(true);
        _abilityObject.transform.position = _player.transform.position + _recallOffset;
        _anim.SetTrigger("Recall");
        _recallTimer = _recallTime; // Reset the recall timer when the ability is used
    }

    protected override void Update()
    {
        if (_isRecalling)
        {
            _recallTimer -= Time.deltaTime;

            if (_recallTimer < 0)
            {
                UIManager.Instance.HideRecallProgress();
                SpawnInBase();
                _isRecalling = false;
            }
            else
            {
                float progressRatio = _recallTimer / _recallTime;
                _recallProgress.fillAmount = progressRatio;
                _recallProgressText.text = _recallTimer.ToString("F1");
            }
        }
    }

    private void SpawnInBase()
    {
        Vector3 basePosition = GameObject.Find("BlueRespawnPoint").transform.position;
        _player.transform.position = basePosition;
        Camera.main.transform.position = _baseCameraPosition;
    }
}
