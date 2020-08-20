using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    private bool vrReady = false;

    public TextMeshProUGUI heightText;
    public TextMeshProUGUI movText;
    public TextMeshProUGUI rotText;
    public InputField usernameInput;
    public Button JoinShowcaseButton;
    public Button JoinBigscaleButton;
    public Dropdown selectPlatform;

    private void Start()
    {
        //Sets the Values of the Menu Options to the Global Information Values
        //This is important for when the menu is opened a second time
        GameObject.FindObjectsOfType<GameObject>().First(obj => obj.name.Equals("HeightSlider")).GetComponent<Slider>().value = GlobalInformation.height;
        GameObject.FindObjectsOfType<GameObject>().First(obj => obj.name.Equals("MovementSpeedSlider")).GetComponent<Slider>().value = GlobalInformation.movementSpeed;
        GameObject.FindObjectsOfType<GameObject>().First(obj => obj.name.Equals("RotationSpeedSlider")).GetComponent<Slider>().value = GlobalInformation.rotationSpeed;

        //Sets the Description texts to the values of the Options
        heightText.text = ""+(FindObjectsOfType<GameObject>().First(obj => obj.name.Equals("HeightSlider")).GetComponent<Slider>().value/10).ToString("0.00") + "m";
        movText.text = "" + FindObjectsOfType<GameObject>().First(obj => obj.name.Equals("MovementSpeedSlider")).GetComponent<Slider>().value;
        rotText.text = "" + FindObjectsOfType<GameObject>().First(obj => obj.name.Equals("RotationSpeedSlider")).GetComponent<Slider>().value;
    }

    //Loads Showcase Scene
    public void LoadShowcase()
    {
        GlobalInformation.username = usernameInput.text;
        SceneManager.LoadScene("ShowCase");
        GlobalInformation.currScene = "Showcase";
    }
    public void LoadBigscale()
    {
        GlobalInformation.username = usernameInput.text;
        SceneManager.LoadScene("Bigscale");
        GlobalInformation.currScene = "Bigscale";
    }

    public void setPlatform(int input)
    {
        Debug.Log("test");
        if (input == 0){
            GlobalInformation.desktopReady  = true;
            GlobalInformation.vrReady       = false;
            GlobalInformation.mobileReady   = false;
        }
        if(input == 1){
            GlobalInformation.desktopReady  = false;
            GlobalInformation.vrReady       = true;
            GlobalInformation.mobileReady   = false;
        }
        if (input == 2){
            GlobalInformation.desktopReady  = false;
            GlobalInformation.vrReady       = false;
            GlobalInformation.mobileReady   = true;
        }
        Debug.Log("GlobalInformation.desktopReady: " + GlobalInformation.desktopReady);
        Debug.Log("GlobalInformation.vrReady: " + GlobalInformation.vrReady);
        Debug.Log("GlobalInformation.mobileReady: " + GlobalInformation.mobileReady);
    }

    //The division by 10 is because the Slider moves in steps does steps, thats completely optional
    public void setHeight(float input)
    {
        heightText.text = "" + (input/10).ToString("0.00") + "m";   //Trimming number
        GlobalInformation.setHeight(input/10);
    }
    public void setMovSpeed(float input)
    {
        movText.text = ""+input;
        GlobalInformation.setMovementSpeed(input);
    }
    public void setRotSpeed(float input)
    {
        rotText.text = "" + input;
        GlobalInformation.setRotationSpeed(input);
    }
    public void UsernameEntered(string name)
    {
        GlobalInformation.username = name;
        JoinShowcaseButton.interactable = true;
        JoinBigscaleButton.interactable = true;
    }
}
