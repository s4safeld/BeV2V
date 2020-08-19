using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalInformation : MonoBehaviour
{
    public static bool vrReady = true;
    public static float height = 10.0f;
    public static float movementSpeed = 5;
    public static float rotationSpeed = 100;
    public static string currScene = "Menu";
    public static string username;
    public static ChatManager chatmanager;
    public static bool connected = false;
    public static GameObject XRSet;

    private void Start()
    {

        if (GameObject.FindObjectOfType<GlobalInformation>() != this)
            Destroy(this);
        else
            DontDestroyOnLoad(this);
    }

    public static void setHeight(float input)
    {
        height = input;
        Debug.Log("GlobalInformation: Height set to: " + height + "m");
    }

    public static void setVrReady(bool input)
    {
        vrReady = input;
        Debug.Log("Global Information: vrReady set to: " + vrReady);
    }
    public static void setMovementSpeed(float inputSpeed)
    {
        movementSpeed = inputSpeed;
        Debug.Log("Globalinformation: changed speed to: " + movementSpeed);
    }
    public static void setRotationSpeed(float inputSpeed)
    {
        rotationSpeed = inputSpeed;
        Debug.Log("GlobalInformation: changed rotation Speed to: " + rotationSpeed);
    }
}
