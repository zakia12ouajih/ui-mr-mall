using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Text;

public class ApiManager : MonoBehaviour
{
    public static ApiManager Instance;

    private string BASE_URL = "http://localhost:5089/api";

    void Awake()
    {
        Instance = this;
    }

    public IEnumerator Get(string endpoint, Action<string> onSuccess, Action<string> onError)
    {
        using (UnityWebRequest req = UnityWebRequest.Get(BASE_URL + endpoint))
        {
            string token = PlayerPrefs.GetString("gb_token", "");

            if (!string.IsNullOrEmpty(token))
                req.SetRequestHeader("Authorization", "Bearer " + token);

            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
                onSuccess?.Invoke(req.downloadHandler.text);
            else
                onError?.Invoke(req.error);
        }
    }

    public IEnumerator Post(string endpoint, string json, Action<string> onSuccess, Action<string> onError)
    {
        using (UnityWebRequest req = new UnityWebRequest(BASE_URL + endpoint, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();

            req.SetRequestHeader("Content-Type", "application/json");

            string token = PlayerPrefs.GetString("gb_token", "");
            if (!string.IsNullOrEmpty(token))
                req.SetRequestHeader("Authorization", "Bearer " + token);

            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
                onSuccess?.Invoke(req.downloadHandler.text);
            else
                onError?.Invoke(req.error);
        }
    }
}