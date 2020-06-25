using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    private bool vrReady = true;

    public TextMeshProUGUI heightText;
    public TextMeshProUGUI movText;
    public TextMeshProUGUI rotText;

    private void Start()
    {
        //Sets the Values of the Menu Options to the Global Information Values
        //This is important for when the menu is opened a second time
        GameObject.FindObjectsOfType<GameObject>().First(obj => obj.name.Equals("VrReadyToggle")).GetComponent<Toggle>().isOn = GlobalInformation.vrReady;
        GameObject.FindObjectsOfType<GameObject>().First(obj => obj.name.Equals("HeightSlider")).GetComponent<Slider>().value = GlobalInformation.height;
        GameObject.FindObjectsOfType<GameObject>().First(obj => obj.name.Equals("MovementSpeedSlider")).GetComponent<Slider>().value = GlobalInformation.movementSpeed;
        GameObject.FindObjectsOfType<GameObject>().First(obj => obj.name.Equals("RotationSpeedSlider")).GetComponent<Slider>().value = GlobalInformation.rotationSpeed;

        //Sets the Description texts to the values of the Options
        heightText.text = ""+FindObjectsOfType<GameObject>().First(obj => obj.name.Equals("HeightSlider")).GetComponent<Slider>().value/10;
        movText.text = "" + FindObjectsOfType<GameObject>().First(obj => obj.name.Equals("MovementSpeedSlider")).GetComponent<Slider>().value;
        rotText.text = "" + FindObjectsOfType<GameObject>().First(obj => obj.name.Equals("RotationSpeedSlider")).GetComponent<Slider>().value;
    }

    //Loads Showcase Scene
    public void LoadShowcase()
    {
        SceneManager.LoadScene("ShowCase");
        GlobalInformation.currScene = "Showcase";
    }
    public void LoadBigscale()
    {
        SceneManager.LoadScene("Bigscale");
        GlobalInformation.currScene = "Bigscale";
    }

    public void setVrReady(bool input)
    {
        vrReady = input;
        GlobalInformation.setVrReady(vrReady);
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
}
