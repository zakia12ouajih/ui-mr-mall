using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using TMPro;
using System.Collections;
using System.Text;

[System.Serializable]
public class LoginResponse
{
    public string token;
    public string id;
    public string[] roles;
}

[System.Serializable]
public class LoginRequest
{
    public string email;
    public string password;
}


[System.Serializable]
public class RegisterRequest
{
    public string firstName;
    public string lastName;
    public string email;
    public string password;
}

public class AuthUIManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject panelWelcome;
    public GameObject panelLogin;
    public GameObject panelRegister;

    [Header("Login Inputs")]
    public TMP_InputField loginEmail;
    public TMP_InputField loginPassword;

    [Header("Register Inputs")]
    public TMP_InputField regFirstName;
    public TMP_InputField regLastName;
    public TMP_InputField regEmail;
    public TMP_InputField regPassword;


    private string BASE_URL = "http://localhost:5089";

    void Start()
    {
        ShowWelcome();
    }

    // ================= UI NAV =================
    public void ShowWelcome()
    {
        panelWelcome.SetActive(true);
        panelLogin.SetActive(false);
        panelRegister.SetActive(false);
    }

    public void ShowLogin()
    {
        panelWelcome.SetActive(false);
        panelLogin.SetActive(true);
        panelRegister.SetActive(false);
    }

    public void ShowRegister()
    {
        panelWelcome.SetActive(false);
        panelLogin.SetActive(false);
        panelRegister.SetActive(true);
    }

    // ================= LOGIN =================
    public void OnLoginClicked()
    {
        string email = loginEmail.text.Trim();
        string password = loginPassword.text;
        Debug.Log("Email: " + email);
        Debug.Log("Password: " + password);

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            Debug.LogError("Missing email or password");
            return;
        }

        StartCoroutine(LoginCoroutine(email, password));
    }

    IEnumerator LoginCoroutine(string email, string password)
    {
        string url = BASE_URL + "/api/auth/login";

        LoginRequest body = new LoginRequest
        {
            email = email,
            password = password
        };


        string json = JsonUtility.ToJson(body);

        UnityWebRequest req = new UnityWebRequest(url, "POST");
        req.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            var res = JsonUtility.FromJson<LoginResponse>(req.downloadHandler.text);

            PlayerPrefs.SetString("gb_token", res.token);
            PlayerPrefs.SetString("gb_user_id", res.id);
            PlayerPrefs.SetString("gb_role", res.roles != null && res.roles.Length > 0 ? res.roles[0] : "");
            PlayerPrefs.Save();

            Debug.Log("Login success");

            GoToExplore();
        }
        else
        {
            Debug.LogError("Login failed: " + req.error);
        }
    }

    // ================= REGISTER =================
    public void OnRegisterClicked()
    {
        StartCoroutine(RegisterCoroutine());
    }

    IEnumerator RegisterCoroutine()
    {
        string url = BASE_URL + "/api/auth/register";

        RegisterRequest body = new RegisterRequest
        {
            firstName = regFirstName.text,
            lastName = regLastName.text,
            email = regEmail.text,
            password = regPassword.text
        };


        string json = JsonUtility.ToJson(body);

        UnityWebRequest req = new UnityWebRequest(url, "POST");
        req.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Register success");
            ShowLogin();
        }
        else
        {
            Debug.LogError("Register failed: " + req.error);
        }
    }

    // ================= SCENE =================
    public void GoToExplore()
    {
        SceneManager.LoadScene("ExploreScene");
    }
}