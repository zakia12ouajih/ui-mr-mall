using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject panelWelcome;
    public GameObject panelLogin;
    public GameObject panelCreateAccount;

    void Start()
    {
        ShowWelcome(); // Start with the first panel
    }

    public void ShowWelcome()
    {
        panelWelcome.SetActive(true);
        panelLogin.SetActive(false);
        panelCreateAccount.SetActive(false);
    }

    public void ShowLogin()
    {
        panelWelcome.SetActive(false);
        panelLogin.SetActive(true);
        panelCreateAccount.SetActive(false);
    }

    public void ShowCreateAccount()
    {
        panelWelcome.SetActive(false);
        panelLogin.SetActive(false);
        panelCreateAccount.SetActive(true);
    }

    public void BackToGlobal(){
        panelWelcome.SetActive(true);
        panelLogin.SetActive(false);
        panelCreateAccount.SetActive(false);
    }

    public void GoToExplore()
    {
        SceneManager.LoadScene("ExploreScene"); // use the exact scene name
    }
}