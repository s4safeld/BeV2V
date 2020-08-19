using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatInput : MonoBehaviour
{
    private InputField btn;

    void Start()
    {
        btn = gameObject.GetComponent<InputField>();
    }

    public void Edit(string input)
    {
        Debug.Log("Edit() has been called for: " + name);
        btn.onValueChanged.Invoke(input);
    }
}
