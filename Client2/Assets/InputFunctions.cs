using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InputFunctions : MonoBehaviour
{

    [SerializeField] TMP_Text text;

    public void ConnectToSever(string message)
    {
        text.text = message;

    }
}
