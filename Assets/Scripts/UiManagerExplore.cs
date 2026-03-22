using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class UIManagerExplore : MonoBehaviour
{
    public GameObject categoryButtons;  // parent of category buttons
    public GameObject scrollView;       // the scroll view
    public GameObject searchExplore;
    // Start is called before the first frame update
    void Start()
    {
        searchExplore.SetActive(true);

        categoryButtons.SetActive(true);
        scrollView.SetActive(false);
        
    }


    public void GoToWelcome()
    {
        SceneManager.LoadScene("WelcomeScene"); // use the exact scene name
    }

    public void ShowScrollView()
    {
        categoryButtons.SetActive(false);
        scrollView.SetActive(true);
        
    }
}
