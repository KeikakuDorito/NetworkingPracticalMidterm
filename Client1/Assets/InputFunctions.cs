using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InputFunctions : MonoBehaviour
{

    public GameObject inputCanvas;
    public GameObject inputField;

    public void connectToServer()
    {
        string ip = inputField.GetComponent<TMP_InputField>().text;
        try
        {
            ClientPosition.instance.connectPosition(ip);
            inputCanvas.SetActive(false);
        }
        catch 
        {
            inputField.GetComponent<TMP_InputField>().text = "INVALID ADDRESS";
        }

    }
}
