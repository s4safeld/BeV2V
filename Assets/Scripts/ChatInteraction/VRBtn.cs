using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VRBtn : MonoBehaviour
{
    private Button btn;

    void Start()
    {
        btn = gameObject.GetComponent<Button>();
    }

    public void Send()
    {
        Debug.Log("Send() has been called for: "+name);
        try{ btn.onClick.Invoke();} catch(Exception e) { Debug.Log("btn Invocation did not work: "+e); }
    }
}
