using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LegendSelectManager : MonoBehaviour
{
    [SerializeField] private GameObject _unselectedImagePrefab;
    [SerializeField] private GameObject _selectedImagePrefab;
    [SerializeField] private Image _lockInButtonImage;
    private Image lastSelected;

    public static LegendSelectManager Instance;
    public string SelectedLegendName;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Persist across scenes
        }
        else
        {
            Destroy(gameObject);  // Ensure there's only one instance
        }
    }

    // On touch on a legend object
    public void OnSelect(GameObject gameObject)
    {
        // Find the legend background image
        if (gameObject.TryGetComponent<Image>(out var bgImage))
        {
            // Change visuals
            if (lastSelected == null)
                lastSelected = bgImage;
            else
                lastSelected.sprite = _unselectedImagePrefab.GetComponent<Image>().sprite;
            bgImage.sprite = _selectedImagePrefab.GetComponent<Image>().sprite;
        }
        _lockInButtonImage.color = Color.white;
        
        // if legend is available, save legend name to pass
        SelectedLegendName = gameObject.name;
        Debug.Log(gameObject.name);
    }

    // On legend lock in, load game scene with chosen legend
    public void OnLockIn()
    {
        // Load game
        SceneManager.LoadScene("LoadingScreen");
    }
}
