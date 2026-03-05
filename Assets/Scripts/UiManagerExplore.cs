using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class UIManagerExplore : MonoBehaviour
{
    public GameObject searchExplore;
    // Start is called before the first frame update
    void Start()
    {
        searchExplore.SetActive(true);
    }


    public void GoToWelcome()
    {
        SceneManager.LoadScene("WelcomeScene"); // use the exact scene name
    }
}
