using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UserElementControls : MonoBehaviour
{
    RegisterLoginScreen RegLoginScript;
    public GameObject toggleButton;
    public TMP_InputField min;
    public TMP_InputField sec;

    private void Start()
    {
        RegLoginScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<RegisterLoginScreen>();
    }
    public void EnbledChanged()
    {
        bool _val = toggleButton.GetComponent<Toggle>().isOn;
        RegLoginScript.UserActivationChanged(name,_val);
    }
    public void MinChanged()
    {
        int _min=int.Parse(min.GetComponent<TMP_InputField>().text);
        RegLoginScript.UserMinChanged(name, _min);
    }
    public void SecChanged()
    {
        int _sec = int.Parse(sec.GetComponent<TMP_InputField>().text);
        RegLoginScript.UserSecChanged(name, _sec);
    }
}
