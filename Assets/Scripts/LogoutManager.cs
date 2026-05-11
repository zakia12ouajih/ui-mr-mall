using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LogoutManager 
{
    public static void Logout()
    {
        PlayerPrefs.DeleteKey("gb_token");
        PlayerPrefs.DeleteKey("gb_user_id");
        PlayerPrefs.DeleteKey("gb_first_name");
        PlayerPrefs.DeleteKey("gb_last_name");
        PlayerPrefs.Save();
    }

    public static string GetFirstName() =>
        PlayerPrefs.GetString("gb_first_name", "");

    public static string GetLastName() =>
        PlayerPrefs.GetString("gb_last_name", "");

    public static string GetFullName() =>
        (GetFirstName() + " " + GetLastName()).Trim();
}