using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LegendSelect : MonoBehaviour
{
    [SerializeField] GameObject _unselectedImagePrefab;
    [SerializeField] GameObject _selectedImagePrefab;
    [SerializeField] Image _lockInButtonImage;
    private Image lastSelected;


 /*   // On touch on a legend object
    public void OnSelect(GameObject coll)
    {
        // Find the background image
        if (coll.TryGetComponent<Image>(out var bgImage))
        {
            // Change visuals
            if (lastSelected != null)
                lastSelected = bgImage;
            else
                lastSelected.sprite = _unselectedImagePrefab.GetComponent<Image>().sprite;
            bgImage.sprite = _selectedImagePrefab.GetComponent<Image>().sprite;
        }
        _lockInButtonImage.color = Color.white;
    }*/

    // On touch on a legend object
    public void OnSelect(GameObject gameObject)
    {
        // Find the background image
        if (gameObject.TryGetComponent<Image>(out var bgImage))
        {
            // Change visuals
            if (lastSelected != null)
                lastSelected = bgImage;
            else
                lastSelected.sprite = _unselectedImagePrefab.GetComponent<Image>().sprite;
            bgImage.sprite = _selectedImagePrefab.GetComponent<Image>().sprite;
        }
        _lockInButtonImage.color = Color.white;
    }

    // On legend lock in, load game scene with chosen legend
    public void OnLockIn(GameObject gameObject)
    {
        // LoadGame(gameObject.name)
    }
}
