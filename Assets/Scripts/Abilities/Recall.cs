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

    private Vector3 _initialPosition;
    private int _initialHp;

    private readonly Vector3 _baseCameraPosition = new(-47f, 4f, -3f);
    private readonly Vector3 _recallOffset = new(0.1f, -1.4f, 0f);

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

    protected override void Update()
    {
        if (_isRecalling)
        {
            _recallTimer -= Time.deltaTime;

            // Finished Recall
            if (_recallTimer < 0)
            {
                StopRecall();
                SpawnInBase();
            }
            // Still in progress
            else
            {
                float progressRatio = _recallTimer / _recallTime;
                _recallProgress.fillAmount = progressRatio;
                _recallProgressText.text = _recallTimer.ToString("F1");

                // Stop recalling if player moved or got damage
                if (_initialHp != _player.Hp || _initialPosition != _player.transform.position)
                {
                    StopRecall();
                }
            }
        }
    }

    public override void OnAbilityTouch(Vector3 fingerPosition)
    {
        float touchTransitionDuration = 0.2f;
        UIManager.Instance.ShrinkAbilityImage(_abilityImage, _abilityImage, touchTransitionDuration);
        UIManager.Instance.ShowRecallProgress();
        CastAbility(fingerPosition, _player);
    }

    public override void CastAbility(Vector3 abilityPosition, Entity caster)
    {
        if (!_isRecalling)
        {
            _isRecalling = true;
            _abilityObject.SetActive(true);
            _abilityObject.transform.position = _player.transform.position + _recallOffset;
            _anim.SetBool("Recall", true);
            _recallTimer = _recallTime;
            _initialPosition = _player.transform.position;
            _initialHp = _player.Hp;
        }
    }


    private void SpawnInBase()
    {
        Vector3 basePosition = GameObject.Find("BlueRespawnPoint").transform.position;
        _player.transform.position = basePosition;
        Camera.main.transform.position = _baseCameraPosition;
    }

    private void StopRecall()
    {
        UIManager.Instance.HideRecallProgress();
        _isRecalling = false;
        _abilityObject.SetActive(false);
        _anim.SetBool("Recall", false);
    }
}
