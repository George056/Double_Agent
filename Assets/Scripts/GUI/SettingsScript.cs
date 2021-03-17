using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsScript : MonoBehaviour
{
    public GameObject SettingsWindow;

    public void SettingsWindowPopUp()
    {
        SettingsWindow.SetActive(true);
    }
}
