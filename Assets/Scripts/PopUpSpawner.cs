using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;

public class PopUpSpawner : MonoBehaviour
{
    public static PopUpSpawner Instance;

    private ObjectPool<GameObject> _popUpPool;
    [SerializeField] private GameObject _popUpPrefab;
    private Vector3 _spawnPosition;
    private readonly float _fadeDuration = 1f;
    private readonly float _moveSpeed = 0.01f;

    private void Awake()
    {
        Instance = this;
        _popUpPool = new ObjectPool<GameObject>(CreatePopUp, OnPopUpGet, OnPopUpRelease);
    }

    public void ShowLevelUpPopUp(Vector3 spawnPosition)
    {
        Vector3 levelUpOffset = new (0f, 0.3f);
        _spawnPosition = spawnPosition + levelUpOffset;

        GameObject popUp = _popUpPool.Get();
        popUp.transform.SetPositionAndRotation(_spawnPosition, Quaternion.identity);
        TextMeshPro textMesh = popUp.GetComponent<TextMeshPro>();
        textMesh.SetText("Level Up!");
        textMesh.fontSize = 1.2f;
        textMesh.color = new Color{ r = 132f , g = 85f, b = 255f, a = 255f};
        textMesh.fontStyle = FontStyles.Bold;
        StartCoroutine(FadePopUp(popUp));
    }

    public void ShowDamagePopUp(string text, Vector3 spawnPosition, bool isCritical)
    {
        Color textColor;

        _spawnPosition = spawnPosition;

        GameObject popUp = _popUpPool.Get();
        popUp.transform.SetPositionAndRotation(spawnPosition, Quaternion.identity);
        TextMeshPro textMesh = popUp.GetComponent<TextMeshPro>();
        textMesh.SetText(text);
        if (!isCritical)
        {
            textMesh.fontSize = 7;
            textColor = new Color32(255, 150, 0, 255);

        }
        else
        {
            textMesh.fontSize = 10;
            textColor = Color.red;
        }
        textMesh.color = textColor;
        StartCoroutine(FadePopUp(popUp));
    }

    private GameObject CreatePopUp()
    {
        GameObject popUp = Instantiate(_popUpPrefab, _spawnPosition, Quaternion.identity, transform);
        popUp.SetActive(false);
        return popUp;
    }

    private void OnPopUpGet(GameObject popUp)
    {
        popUp.SetActive(true);
    }

    private void OnPopUpRelease(GameObject popUp)
    {
        popUp.SetActive(false);
    }

    private IEnumerator FadePopUp(GameObject popUp)
    {
        TextMeshPro textMeshPro = popUp.GetComponent<TextMeshPro>();
        Color originalColor = textMeshPro.color;

        float timer = 0f;
        while (timer < _fadeDuration)
        {
            popUp.transform.position += new Vector3(0, _moveSpeed);
            float alpha = Mathf.Lerp(1f, 0f, timer / _fadeDuration);
            Color fadedColor = new(originalColor.r, originalColor.g, originalColor.b, alpha);
            textMeshPro.color = fadedColor;

            timer += Time.deltaTime;
            yield return null;
        }

        _popUpPool.Release(popUp);
    }

}