using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // <--- needed for TMP_Dropdown

public class UIManagerExplore : MonoBehaviour
{
    // public GameObject categoryButtons;  
    // public GameObject scrollView;       
    public GameObject searchExplore;

    // public TMP_Dropdown floorDropdown;    // <--- added
    // public StoreItem storeItem;           // <--- reference to your StoreItem script

    void Start()
    {
        searchExplore.SetActive(true);
        // categoryButtons.SetActive(true);
        // scrollView.SetActive(false);
    }

    public void GoToWelcome()
    {
        SceneManager.LoadScene("WelcomeScene");
    }

    // public void ShowScrollView()
    // {
    //     categoryButtons.SetActive(false);
    //     scrollView.SetActive(true);
    // }

    // public void ShowSelectedFloor()
    // {
    //     int floor = floorDropdown.value + 1;

    //     if (floorDropdown.options[floorDropdown.value].text == "Category View")
    //     {
    //         scrollView.SetActive(false);
    //         categoryButtons.SetActive(true);
    //         return;
    //     }

    //     scrollView.SetActive(true);
    //     categoryButtons.SetActive(false);

    //     if (storeItem != null)
    //         storeItem.SpawnStores(floor);  // call SpawnStores from StoreItem
    // }
}