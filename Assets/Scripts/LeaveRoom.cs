using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class LeaveRoom : MonoBehaviourPunCallbacks
{
    public void leaveRoom() {
        StartCoroutine(DisableScript());
        PhotonNetwork.Disconnect();
        Destroy(GameObject.FindObjectsOfType<GameObject>().First(obj => obj.name.Equals("WebXRCameraSet")));
        SceneManager.LoadScene("MainMenu");
        GlobalInformation.currScene = "Menu";
    }

    //This is just disabling the Script for a short time, so that the function wont be called multiple times in a second
    IEnumerator DisableScript()
    {
        this.enabled = false;

        yield return new WaitForSeconds(0.1f);

        this.enabled = true;
    }

}
